import logging
import sys
import platform
from typing import Optional

from pynvml import (
    nvmlInit,
    nvmlDeviceGetCount,
    nvmlDeviceGetHandleByIndex,
    nvmlDeviceGetMemoryInfo,
    nvmlDeviceGetName,
    nvmlShutdown,
    NVMLError,
)

from models.healthcheck import GpuInfo

logger = logging.getLogger(__name__)

def setup_device() -> tuple[str, str, bool]:
    # Check for Nvidia GPU with pynvml
    try:
        nvmlInit()
        device_count = nvmlDeviceGetCount()
        if device_count > 0:
            handle = nvmlDeviceGetHandleByIndex(0)
            mem_info = nvmlDeviceGetMemoryInfo(handle)
            gpu_name = nvmlDeviceGetName(handle)
            gpu_memory_gb = mem_info.total / 1e9

            logger.info("GPU: %s (%.1f GB)", gpu_name, gpu_memory_gb)

            # Determine model size based on VRAM
            if gpu_memory_gb >= 6:
                model_size = "large-v3"
            elif gpu_memory_gb >= 4:
                model_size = "large-v3-turbo"
            else:
                model_size = "medium"

            nvmlShutdown()
            return "cuda", model_size, True
        nvmlShutdown()
    except NVMLError as e:
        logger.debug("NVIDIA GPU not detected via pynvml: %s", e)

    # Check for Apple MPS (Apple Silicon macOS)
    if sys.platform == "darwin" and platform.machine() == "arm64":
        logger.info("Device: Apple MPS")
        return "mps", "large-v3-turbo", False

    # Fallback to CPU
    logger.info("Device: CPU")
    return "cpu", "base", False

def get_cpu_info() -> Optional[GpuInfo]:
    try:
        nvmlInit()
        handle = nvmlDeviceGetHandleByIndex(0)
        name = nvmlDeviceGetName(handle)
        mem = nvmlDeviceGetMemoryInfo(handle)
        return GpuInfo(
            name=name,
            memory_total_gb=mem.total,
            memory_used_gb=mem.used,
            memory_free_gb=mem.free,
        )
    except NVMLError:
        return None
    finally:
        try:
            nvmlShutdown()
        except NVMLError:
            pass

def get_cuda_version() -> Optional[str]:
    try:
        nvmlInit()
        from pynvml import nvmlSystemGetDriverVersion
        version = nvmlSystemGetDriverVersion()
        nvmlShutdown()
        return version.decode() if isinstance(version, bytes) else version
    except:
        return None