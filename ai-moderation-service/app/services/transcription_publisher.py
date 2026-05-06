import json
import logging
from uuid import UUID

from aio_pika import Message, DeliveryMode
from aio_pika.abc import AbstractRobustChannel

from config import RabbitMqConfig
from models.messaging import UpdateSongInformationMessage
from utils.json_encoder import UUIDEncoder

logger = logging.getLogger(__name__)


class TranscriptionPublisher:
    def __init__(self, channel: AbstractRobustChannel, config: RabbitMqConfig):
        self._channel = channel
        self._config = config

    async def _publish(self, routing_key: str, message: object):
        exchange = await self._channel.get_exchange(self._config.exchange)

        body = json.dumps(message.__dict__, cls=UUIDEncoder).encode("utf-8")
        msg = Message(
            body,
            delivery_mode=DeliveryMode.PERSISTENT,
            content_type="application/json",
        )
        await exchange.publish(msg, routing_key=routing_key)

    async def publish_update_song_info(self, song_id: UUID, contains_explicit: bool):
        logger.info(f"Publishing update info for {song_id}, contains_explicit: {contains_explicit}")
        msg = UpdateSongInformationMessage(
            song_id=song_id,
            contains_explicit_content=contains_explicit
        )
        await self._publish(self._config.update_song_info_routing_key, msg)
        logger.info(f"Published update info for song {song_id}")