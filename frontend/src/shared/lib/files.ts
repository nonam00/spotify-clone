import { CLIENT_FILES_URL } from "@/shared/config/api";

export type FileUploadType = "image" | "audio";

type PresignedUrlResponse = {
  url: string;
  file_id: string;
};

export const FILE_CONFIG = {
  image: {
    maxSize: 32 * 1024 * 1024, // 32MB
  },
  audio: {
    maxSize: 256 * 1024 * 1024, // 256MB
  },
};

const validateFile = (file: File, maxSize: number): string | null => {
  if (file.size > maxSize) {
    return `File too large. Maximum size: ${Math.round(maxSize / 1024 / 1024)}MB`;
  }
  return null;
};

export const validateImage = (file: File) =>
  validateFile(file, FILE_CONFIG.image.maxSize);

export const validateAudio = (file: File) =>
  validateFile(file, FILE_CONFIG.audio.maxSize);

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