export const CLIENT_API_URL = "http://localhost:8080/1";
export const MODERATORS_API_URL = `${CLIENT_API_URL}/moderators`;
export const AUTH_API_URL = `${CLIENT_API_URL}/auth`;
export const CLIENT_FILES_URL = "http://localhost/files/api/v1";

export async function authFetchClient(input: string | URL, init: RequestInit = {}) {
  return fetch(input, {
    credentials: "include",
    ...init,
  });
}