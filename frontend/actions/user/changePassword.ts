"use server";

import { cookies } from "next/headers";

import {SERVER_API_URL} from "@/helpers/api";

const changePassword = async (
  currentPassword: string,
  newPassword: string
): Promise<{
  success: boolean;
  error?: string
}> => {
  try {
    const cookieStore = await cookies();

    const response = await fetch(`${SERVER_API_URL}/users/password`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Cookie: cookieStore.toString()
      },
      body: JSON.stringify({
        currentPassword: currentPassword,
        newPassword: newPassword,
      }),
      credentials: "include"
    });

    if (!response.ok) {
      const errorData = await response.json();
      return { success: false, error: errorData.detail || "Failed to change password" };
    }

    return { success: true };
  } catch (error) {
    console.error("Error changing password:", error);
    return { success: false, error: "Internal server error" };
  }
};

export default changePassword;

