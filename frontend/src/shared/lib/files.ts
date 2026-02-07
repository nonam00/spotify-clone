import { CLIENT_FILES_URL } from "@/shared/config/api";

export type FileUploadType = "image" | "audio";

type PresignedUrlResponse = {
  url: string;
  file_id: string;
};


type FileConfigElementType = {
  allowedTypes: string[],
  maxSize: number
}

type FileConfigType = Record<string, FileConfigElementType>;

export const FILE_CONFIG: FileConfigType = {
  image: {
    allowedTypes: [".png", ".jpg", ".jpeg", ".webp", ".heif", ".raw"],
    maxSize: 32 * 1024 * 1024, // 32MB
  },
  audio: {
    allowedTypes: [".mp3", ".wav", ".flac", ".m4a", ".aac", ".ogg"],
    maxSize: 256 * 1024 * 1024, // 256MB
  },
};

const validateSize = (file: File, maxSize: number): string | null => {
  if (file.size > maxSize) {
    return `File too large. Maximum size: ${Math.round(maxSize / 1024 / 1024)}MB`;
  }
  return null;
};

const validateType = (file: File, allowedTypes: string[]): string | null => {
  const extension = file.name.split(".").pop()?.toLowerCase();

  if (extension === undefined) {
    return "Invalid file extension.";
  }

  if (!allowedTypes.includes("." + extension)) {
    return "Invalid file type. Allowed types: " + allowedTypes.join(" ");
  }

  return null;
}

export function validateImage(file: File): string | null {
  const sizeError = validateSize(file, FILE_CONFIG.image.maxSize);
  if (sizeError) {
    return sizeError;
  }

  const typeError = validateType(file, FILE_CONFIG.image.allowedTypes);
  if (typeError) {
    return typeError;
  }

  return null;
}

export function validateAudio(file: File): string | null {
  const sizeError = validateSize(file, FILE_CONFIG.audio.maxSize);
  if (sizeError) {
    return sizeError;
  }

  const typeError = validateType(file, FILE_CONFIG.audio.allowedTypes);
  if (typeError) {
    return typeError;
  }
  return null;
}

export async function getPresignedUrl(
  type: FileUploadType
): Promise<PresignedUrlResponse | null> {
  try {
    const uploadResponse = await fetch(`${CLIENT_FILES_URL}/upload-url`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ file_type: type }),
    });

    if (!uploadResponse.ok) {
      throw new Error("Failed to get upload URL");
    }

    return await uploadResponse.json();
  } catch (error) {
    console.error("Error getting presigned URL:", error);
    return null;
  }
}

export async function getPresignedUrls(): Promise<
  [PresignedUrlResponse, PresignedUrlResponse] | null
> {
  try {
    const [imageResponse, audioResponse] = await Promise.all([
      fetch(`${CLIENT_FILES_URL}/upload-url`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ file_type: "image" }),
      }),
      fetch(`${CLIENT_FILES_URL}/upload-url`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ file_type: "audio" }),
      }),
    ]);

    if (!imageResponse.ok || !audioResponse.ok) {
      throw new Error("Failed to get upload URLs");
    }

    const [imageData, audioData] = await Promise.all([
      imageResponse.json(),
      audioResponse.json(),
    ]);

    return [imageData, audioData];
  } catch (error) {
    console.error("Error getting presigned URLs:", error);
    return null;
  }
}

export async function uploadFileToS3(
  url: string,
  file: File,
  contentType: FileUploadType
): Promise<boolean> {
  try {
    const response = await fetch(url, {
      method: "PUT",
      headers: { "content-type": contentType },
      body: file,
    });
    return response.ok;
  } catch (error) {
    console.error("Error uploading file to S3:", error);
    return false;
  }
}