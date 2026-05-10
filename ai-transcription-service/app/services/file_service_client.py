import aiofiles
import httpx

from config import FilesConfig


class FileServiceClient:
    def __init__(self, files_config: FilesConfig):
        self._base_url = files_config.api_url
        self._max_size = files_config.max_file_size
        self._chunk_size = files_config.chunk_size

    async def download_audio_file(self, file_id: str, tmp_path: str):
        url = f"{self._base_url}/download-url?type=audio&is_internal=true&file_id={file_id}"
        print(url)
        async with httpx.AsyncClient() as client:
            async with client.stream(method="GET", url=url, follow_redirects=True) as response:
                response.raise_for_status()
                total_size = 0
                async with aiofiles.open(tmp_path, "wb") as f:
                    async for chunk in response.aiter_bytes(chunk_size=self._chunk_size):
                        total_size += len(chunk)

                        if total_size > self._max_size:
                            await f.close()
                            raise Exception(
                                f"File exceeds {self._max_size // (1024 * 1024)} MB limit."
                                f" Maximum {self._max_size // (1024 * 1024)} MB"
                            )

                        await f.write(chunk)