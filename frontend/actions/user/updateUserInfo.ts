"use server";

import { revalidatePath } from "next/cache";
import { cookies } from "next/headers";
import { SERVER_API_URL } from "@/helpers/api";

type updateUserData = {
  fullName?: string,
  avatar?: File
}

const updateUserInfo = async (data: updateUserData): Promise<{
  success: boolean;
  error?: string
}> => {
  try {
    const cookieStore = await cookies();
    
    const formData = new FormData();
    if (data.fullName) {
      formData.append("FullName", data.fullName);
    }
    if (data.avatar) {
      formData.append("Avatar", data.avatar);
    }

    const response = await fetch(`${SERVER_API_URL}/users`, {
      method: "PUT",
      headers: {
        Cookie: cookieStore.toString()
      },
      body: formData,
      credentials: "include"
    });

    if (!response.ok) {
      const errorData = await response.json();
      return { success: false, error: errorData.detail || "Failed to update user info" };
    }

    revalidatePath("/account");
    return { success: true };
  } catch (error) {
    console.error("Error updating user info:", error);
    return { success: false, error: "Internal server error" };
  }
};

export default updateUserInfo;
