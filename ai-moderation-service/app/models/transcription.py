from dataclasses import dataclass

@dataclass(frozen=True)
class MiniSegment:
    start: float
    end: float
    text: str

@dataclass(frozen=True)
class TranscribeResult:
    language: str | None
    segments: list[MiniSegment]
    confidence: float
    duration: float