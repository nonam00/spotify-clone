from dataclasses import dataclass
from typing import List, Optional

from models.transcribe_result import MiniSegment

@dataclass(frozen=True)
class TranscribeResponse:
    text: str
    language: Optional[str]
    segments: Optional[List[MiniSegment]]
    is_explicit: bool
    confidence: float
    done_in: float
