from typing import Optional
from dataclasses import dataclass

@dataclass(frozen=True)
class GpuInfo:
    name: str
    memory_total_gb: float
    memory_used_gb: float
    memory_free_gb: float

@dataclass(frozen=True)
class HealthCheckResponse:
    healthy: bool
    model: str
    device: str
    fp16: bool
    cuda_available: bool
    cuda_version: Optional[str]
    gpu_info: Optional[GpuInfo]
