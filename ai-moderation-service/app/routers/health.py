from fastapi import APIRouter, Depends

from config import Config
from dependencies import get_config
from models.healthcheck_response import HealthCheckResponse
from utils.device import get_cpu_info, get_cuda_version

router = APIRouter()

@router.get("/health")
async def health(config: Config = Depends(get_config)) -> HealthCheckResponse:
    gpu_info = get_cpu_info()
    cuda_available = gpu_info is not None
    cuda_version = get_cuda_version() if cuda_available else None
    return HealthCheckResponse(
        healthy = True,
        model = config.model_config.model_size,
        device = config.model_config.device.upper(),
        fp16 = config.model_config.use_fp16,
        cuda_available = cuda_available,
        cuda_version=cuda_version,
        gpu_info = gpu_info,
    )
