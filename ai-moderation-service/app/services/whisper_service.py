import logging
from dataclasses import asdict

import numpy as np
from faster_whisper import WhisperModel

from config import ModelConfig, TranscribeConfig
from models.transcription import TranscribeResult, LyricsSegment

logger = logging.getLogger(__name__)

class WhisperService:
    def __init__(self, config: ModelConfig, transcribe_config: TranscribeConfig):
        self._model_config = config
        self._transcribe_config = transcribe_config
        self._model = self._load_model()

    def _load_model(self):
        compute_type = (
            "float16"
            if self._model_config.device == "cuda" and self._model_config.use_fp16
            else "float32"
        )

        logger.info(
            "Loading model %s on %s (compute type: %s)...",
            self._model_config.model_size.upper(),
            self._model_config.device.upper(),
            compute_type
        )

        model = WhisperModel(
            self._model_config.model_size,
            device=self._model_config.device,
            compute_type=compute_type,
        )

        return model

    def run_transcribe(self, file_path: str) -> TranscribeResult:
        # Converting transcribe config object into dictionary to pass into then function
        options = asdict(self._transcribe_config)

        segments_generator, info = self._model.transcribe(
            audio=file_path,
            **options,
        )

        # Exponential sum for confidence calculation
        exp_sum = 0.0
        size = 0

        # Partial segments objects for response
        segments: list[LyricsSegment] = []
        for segment in segments_generator:
            logger.info(f"[{segment.start:.2f}s -> {segment.end:.2f}s] {segment.text}")

            segments.append(LyricsSegment(
                start = segment.start,
                end = segment.end,
                text = segment.text,
                order = size + 1,
            ))

            # Exponential function used to convert logarithmic probability back to normal value between 0 and 1
            exp_sum += np.exp(segment.avg_logprob)
            size += 1

        confidence = exp_sum / size

        return TranscribeResult(
            language = info.language,
            lyrics_segments= segments,
            confidence = confidence,
            duration = info.duration,
        )
