from dataclasses import dataclass

@dataclass(frozen=True)
class LyricsSegment:
    start: float
    end: float
    text: str
    order: int

@dataclass(frozen=True)
class TranscribeResult:
    language: str | None
    lyrics_segments: list[LyricsSegment]
    confidence: float
    duration: float