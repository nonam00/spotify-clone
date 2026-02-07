import { z } from 'zod';
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

function getFileExtension(fileName: string): string {
  const parts = fileName.split(".");
  return parts.length > 1 ? `.${parts.pop()?.toLowerCase()}` : "";
}

// Base Zod schema for file validation
function createFileSchema(type: FileUploadType) {
  const config = FILE_CONFIG[type];
  const maxSizeMB = Math.round(config.maxSize / 1024 / 1024);

  return z
    .instanceof(File)
    .refine(
      (file) => {
        const extension = getFileExtension(file.name);
        return config.allowedTypes.includes(extension);
      },
      {
        message: `Invalid file type. Allowed types: ${config.allowedTypes.join(", ")}`,
      }
    )
    .refine(
      (file) => file.size > 0,
      {
        message: "File is empty."
      }
    )
    .refine(
      (file) => file.size <= config.maxSize,
      {
        message: `File too large. Maximum size: ${maxSizeMB}MB`,
      }
    );
}

// Pre-defined schemas for each file type
export const imageFileSchema = createFileSchema("image");
export const audioFileSchema = createFileSchema("audio");

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