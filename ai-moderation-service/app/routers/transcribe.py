import asyncio
import json
import logging
import tempfile
import time
from concurrent.futures.thread import ThreadPoolExecutor

from aio_pika.abc import AbstractRobustChannel, AbstractIncomingMessage
from fastapi import APIRouter

from config import Config
from dependencies import get_whisper_service, get_executor, get_file_service_client
from models.messaging import TranscribeSongMessage
from services.file_service_client import FileServiceClient
from services.moderation_service import check_text_for_explicit_content
from services.transcription_publisher import TranscriptionPublisher
from services.whisper_service import WhisperService
from utils.files import cleanup_file

logger = logging.getLogger(__name__)

router = APIRouter()


async def start_transcribe_consumer(
    channel: AbstractRobustChannel,
    config: Config,
):
    logger.info("Starting Transcribe Consumer")

    await channel.set_qos(prefetch_count=config.max_concurrent_transcriptions)

    semaphore = asyncio.Semaphore(config.max_concurrent_transcriptions)

    queue = await channel.declare_queue(
        config.rabbit_mq_config.transcribe_song_queue,
        durable=True,
        passive=True
    )

    file_service_client = get_file_service_client()
    whisper_service = get_whisper_service()
    transcription_publisher = TranscriptionPublisher(channel, config.rabbit_mq_config)
    executor = get_executor()

    async def process_transcribe_message(message: AbstractIncomingMessage):
        async with semaphore:
            logger.info("Processing Transcribe Message")
            async with message.process():
                body = json.loads(message.body.decode())
                msg = TranscribeSongMessage(**body)

                await handle_transcribe_message(
                    message=msg,
                    file_service_client=file_service_client,
                    whisper_service=whisper_service,
                    transcription_publisher=transcription_publisher,
                    executor=executor,
                    config=config
                )

    await queue.consume(process_transcribe_message)

async def handle_transcribe_message(
    message: TranscribeSongMessage,
    file_service_client: FileServiceClient,
    whisper_service: WhisperService,
    transcription_publisher: TranscriptionPublisher,
    executor: ThreadPoolExecutor,
    config: Config,
):
    tmp_path = None
    try:
        with tempfile.NamedTemporaryFile(
            delete=False,
            dir=config.files_config.temp_dir
        ) as tmp:
            tmp_path = tmp.name

        await file_service_client.download_audio_file(message.audio_path, tmp_path)

        start = time.time()

        # Run whisper in thread pool
        result = await asyncio.get_running_loop().run_in_executor(
            executor,
            whisper_service.run_transcribe,
            tmp.name,
        )

        # Getting full text from segments
        full_text = " ".join(segment.text for segment in result.segments)

        is_explicit = check_text_for_explicit_content(full_text)

        elapsed = time.time() - start
        logger.info("Done in %.1f s", elapsed)

        await transcription_publisher.publish_update_song_info(
            song_id=message.song_id,
            contains_explicit=is_explicit
        )
    finally:
        asyncio.get_running_loop().run_in_executor(
            executor,
            cleanup_file,
            tmp_path,
        )
