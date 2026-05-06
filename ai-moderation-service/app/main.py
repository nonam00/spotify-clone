import asyncio
import logging
from contextlib import asynccontextmanager

from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

from dependencies import get_config, get_rabbitmq_connection_manager
from routers import transcribe, health
from routers.transcribe import start_transcribe_consumer

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s - %(name)s - %(levelname)s - %(message)s",
)

logger = logging.getLogger(__name__)

config = get_config()

@asynccontextmanager
async def lifespan(_: FastAPI):
    rabbitmq_connection_manager = get_rabbitmq_connection_manager()
    await rabbitmq_connection_manager.connect()
    channel = await rabbitmq_connection_manager.get_channel()

    consumer_task = asyncio.create_task(start_transcribe_consumer(channel, config))
    yield
    consumer_task.cancel()
    try:
        await consumer_task
    except asyncio.CancelledError:
        pass
    await rabbitmq_connection_manager.close()

def create_app() -> FastAPI:
    app = FastAPI(title="Whisper ASR microservice", lifespan=lifespan)

    app.add_middleware(
        CORSMiddleware,
        allow_origins=["*"],
        allow_credentials=True,
        allow_methods=["*"],
        allow_headers=["*"],
    )

    app.include_router(transcribe.router)
    app.include_router(health.router)

    return app

_app = create_app()

if __name__ == "__main__":
    import uvicorn
    logger.info("Temp files: %s", config.files_config.temp_dir)
    logger.info("File service connection: %s", config.files_config.api_url)
    uvicorn.run("main:_app", host="0.0.0.0", port=8000, log_level="info", timeout_keep_alive=300)
