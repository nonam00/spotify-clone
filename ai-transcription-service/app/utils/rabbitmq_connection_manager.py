import logging
from typing import Optional

import aio_pika
from aio_pika.abc import AbstractRobustChannel, AbstractRobustConnection, ExchangeType

from config import RabbitMqConfig

logger = logging.getLogger(__name__)

class RabbitMqConnectionManager:
    def __init__(self, config: RabbitMqConfig):
        self._config = config
        self._connection: Optional[AbstractRobustConnection] = None
        self._channel: Optional[AbstractRobustChannel] = None

    async def connect(self):
        self._connection = await aio_pika.connect_robust(self._config.url)
        self._channel = await self._connection.channel()
        await self._declare_topology()

    async def _declare_topology(self):
        """Declare exchange, queues, bindings and DLX setup."""
        # Declare global DLX exchange
        dlx_exchange = await self._channel.declare_exchange(
            self._config.global_dlx,
            ExchangeType.DIRECT,
            durable=True,
        )

        # Declare DLQs and bind them to DLX
        dlq_args = {"x-queue-type": "classic"}  # optional
        transcribe_dlq = await self._channel.declare_queue(
            self._config.transcribe_song_dlq,
            durable=True,
            arguments=dlq_args,
        )
        await transcribe_dlq.bind(
            dlx_exchange,
            routing_key=self._config.transcribe_song_dlq,
        )

        update_dlq = await self._channel.declare_queue(
            self._config.update_song_info_dlq,
            durable=True,
            arguments=dlq_args,
        )
        await update_dlq.bind(
            dlx_exchange,
            routing_key=self._config.update_song_info_dlq,
        )

        # Main topic exchange
        main_exchange = await self._channel.declare_exchange(
            self._config.exchange,
            ExchangeType.TOPIC,
            durable=True,
        )

        # Transcribe queue with DLX policy
        transcribe_args = {
            "x-dead-letter-exchange": self._config.global_dlx,
            "x-dead-letter-routing-key": self._config.transcribe_song_dlq,
        }
        transcribe_queue = await self._channel.declare_queue(
            self._config.transcribe_song_queue,
            durable=True,
            arguments=transcribe_args,
        )
        await transcribe_queue.bind(
            main_exchange,
            routing_key=self._config.transcribe_song_routing_key,
        )

        # Update queue with DLX policy
        update_args = {
            "x-dead-letter-exchange": self._config.global_dlx,
            "x-dead-letter-routing-key": self._config.update_song_info_dlq,
        }
        update_queue = await self._channel.declare_queue(
            self._config.update_song_info_queue,
            durable=True,
            arguments=update_args,
        )
        await update_queue.bind(
            main_exchange,
            routing_key=self._config.update_song_info_routing_key,
        )

        logger.info("RabbitMQ topology declared successfully")

    async def get_channel(self) -> AbstractRobustChannel:
        if self._channel is None or self._channel.is_closed:
            await self.connect()
        logger.info("RabbitMQ channel recreated")
        return self._channel

    async def close(self):
        if self._channel:
            await self._channel.close()
        if self._connection:
            await self._connection.close()