import os
import tempfile
from concurrent.futures import ThreadPoolExecutor

from config import Config, FilesConfig, TranscribeConfig, ModelConfig, RabbitMqConfig
from services.file_service_client import FileServiceClient
from services.whisper_service import WhisperService
from utils.device import setup_device
from utils.rabbitmq_connection_manager import RabbitMqConnectionManager

model_config = ModelConfig(*setup_device())
transcribe_config = TranscribeConfig()

rabbitmq_url = os.getenv(
    "RABBITMQ_URL",
    default="amqp://myuser:mypassword@localhost:5672//"
)
rabbit_mq_config = RabbitMqConfig(url=rabbitmq_url)

files_api_url = os.getenv(
    "FILE_SERVICE_URL",
    default="http://localhost/files/api/v1"
)
files_config = FilesConfig(api_url=files_api_url, temp_dir=tempfile.mkdtemp())

num_workers = 2 if model_config.device == "gpu" else 1
config = Config(
    model_config=model_config,
    transcribe_config=transcribe_config,
    files_config=files_config,
    rabbit_mq_config=rabbit_mq_config,
    num_workers=num_workers
)

def get_config() -> Config:
    return config

whisper_service = WhisperService(config.model_config, config.transcribe_config)
executor = ThreadPoolExecutor(max_workers=config.num_workers)
file_service_client = FileServiceClient(files_config)
rabbitmq_connection_manager = RabbitMqConnectionManager(rabbit_mq_config)

def get_whisper_service() -> WhisperService:
    return whisper_service

def get_file_service_client() -> FileServiceClient:
    return file_service_client

def get_rabbitmq_connection_manager() -> RabbitMqConnectionManager:
    return rabbitmq_connection_manager

def get_executor() -> ThreadPoolExecutor:
    return executor