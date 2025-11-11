import {CLIENT_FILES_URL} from "@/helpers/api";

export type fileUploadType = "image" | "audio";

type PresignedUrlResponse = {
    url: string;
    file_id: string;
}

export const FILE_CONFIG = {
    image: {
        maxSize: 32 * 1024 * 1024, // 32MB
        //allowedTypes: ["image/jpeg", "image/png", "image/webp", "image/gif"],
    },
    audio: {
        maxSize: 256 * 1024 * 1024, // 256MB
        //allowedTypes: ["audio/mpeg", "audio/wav", "audio/flac", "audio/mp4", "audio/aac", "audio/ogg"],
    }
};

const validateFile = (
  file: File,
  maxSize: number,
  //allowedTypes: string[]
): string | null => {
    if (file.size > maxSize) {
        return `File too large. Maximum size: ${Math.round(maxSize / 1024 / 1024)}MB`;
    }
    return null;
};

export const validateImage = (file: File) => validateFile(
  file,
  FILE_CONFIG.image.maxSize,
  //FILE_CONFIG.image.allowedTypes
);

export const validateAudio = (file: File) => validateFile(
  file,
  FILE_CONFIG.audio.maxSize,
  //FILE_CONFIG.audio.allowedTypes
);

export async function getPresignedUrl(type: fileUploadType): Promise<PresignedUrlResponse | null> {
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

export async function getPresignedUrls(): Promise<[PresignedUrlResponse, PresignedUrlResponse] | null> {
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
            })
        ]);

        if (!imageResponse.ok || !audioResponse.ok) {
            throw new Error("Failed to get upload URLs");
        }

        const [imageData, audioData] = await Promise.all([
            imageResponse.json(),
            audioResponse.json()
        ]);

        return [imageData, audioData];
    } catch (error) {
        console.error("Error getting presigned URLs:", error);
        return null;
    }
}

export async function uploadFileToS3(url: string, file: File, contentType: fileUploadType): Promise<boolean> {
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