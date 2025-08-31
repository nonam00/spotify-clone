import {authFetchClient, CLIENT_API_URL} from "@/helpers/api";

export async function uploadSong (form: FormData) {
  return await authFetchClient(`${CLIENT_API_URL}/songs`, {
    method: "POST",
    credentials: "include",
    body: form
  });
}
