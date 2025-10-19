"use server";

import { revalidatePath } from "next/cache";
import { cookies } from "next/headers";

import {SERVER_API_URL} from "@/helpers/api";

type updateUserData = {
  fullName: string | null,
  avatarId: string | null,
}

const updateUserInfo = async (data: updateUserData): Promise<{
  success: boolean;
  error?: string
}> => {
  try {
    const cookieStore = await cookies();

    const response = await fetch(`${SERVER_API_URL}/users`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Cookie: cookieStore.toString()
      },
      body: JSON.stringify(data),
      credentials: "include"
    });

    if (!response.ok) {
      const errorData = await response.json();
      console.log(errorData);
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

