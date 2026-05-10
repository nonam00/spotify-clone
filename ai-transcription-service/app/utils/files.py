import logging
import os

logger = logging.getLogger(__name__)

def cleanup_file(path):
    if path and os.path.exists(path):
        try:
            os.unlink(path)
        except OSError as e:
            logger.warning("Failed to delete %s: %s", path, e)