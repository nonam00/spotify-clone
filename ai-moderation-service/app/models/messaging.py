from uuid import UUID

from pydantic.dataclasses import dataclass


@dataclass(frozen=True)
class TranscribeSongMessage:
    song_id: UUID
    audio_path: str

@dataclass(frozen=True)
class UpdateSongInformationMessage:
    song_id: UUID
    contains_explicit_content: bool
