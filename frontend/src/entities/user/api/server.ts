"use server";

import { cookies } from "next/headers";
import { revalidatePath } from "next/cache";

import { SERVER_API_URL } from "@/shared/config/api";
import { UserDetails } from "../model";

export async function getUserInfoServer(): Promise<UserDetails | null> {
  try {
    const cookieStore = await cookies();
    const response = await fetch(`${SERVER_API_URL}/users/info`, {
      method: "GET",
      headers: {
        Cookie: cookieStore.toString()
      },
      credentials: "include"
    });

    if (response.status === 401) {
      return null;
    }

    const data = await response.json();

    if (!response.ok) {
      console.error(data);
      return null;
    }

    return data;
  } catch (error) {
    console.error(error);
    return null;
  }
}

type UpdateUserData = {
  fullName: string | null,
  avatarId: string | null,
}

export async function updateUserInfo(data: UpdateUserData): Promise<{
  success: boolean;
  error?: string;
}> {
  "use server";

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
}

export async function changePassword(
  currentPassword: string,
  newPassword: string
): Promise<{
  success: boolean;
  error?: string
}> {
  "use server";

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
}