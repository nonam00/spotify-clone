import logging
from dataclasses import asdict

import numpy as np
from faster_whisper import WhisperModel

from config import ModelConfig, TranscribeConfig
from models.transcribe_result import TranscribeResult, MiniSegment

logger = logging.getLogger(__name__)

class WhisperService:
    def __init__(self, config: ModelConfig, transcribe_config: TranscribeConfig):
        self.model_config = config
        self.transcribe_config = transcribe_config
        self.model = self._load_model()

    def _load_model(self):
        compute_type = (
            "float16"
            if self.model_config.device == "cuda" and self.model_config.use_fp16
            else "float32"
        )

        logger.info(
            "Loading model %s on %s (compute type: %s)...",
            self.model_config.model_size.upper(),
            self.model_config.device.upper(),
            compute_type
        )

        model = WhisperModel(
            self.model_config.model_size,
            device=self.model_config.device,
            compute_type=compute_type,
        )

        return model

    def run_transcribe(self, file_path: str) -> TranscribeResult:
        # Converting transcribe config object into dictionary to pass into then function
        options = asdict(self.transcribe_config)

        segments_generator, info = self.model.transcribe(
            audio=file_path,
            **options,
        )

        # Exponential sum for confidence calculation
        exp_sum = 0.0
        size = 0

        # Partial segments objects for response
        segments: list[MiniSegment] = []
        for segment in segments_generator:
            logger.info(f"[{segment.start:.2f}s -> {segment.end:.2f}s] {segment.text}")

            segments.append(MiniSegment(
                start = segment.start,
                end = segment.end,
                text = segment.text,
            ))

            # Exponential function used to convert logarithmic probability back to normal value between 0 and 1
            exp_sum += np.exp(segment.avg_logprob)
            size += 1

        confidence = exp_sum / size

        return TranscribeResult(
            language = info.language,
            segments = segments,
            confidence = confidence,
            duration = info.duration,
        )
