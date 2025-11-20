export const CLIENT_API_URL = "http://localhost:8080/1";
export const CLIENT_FILES_URL = "http://localhost/files/api/v1";

const refreshClient = async (url: string): Promise<boolean> => {
  const response = await fetch(`${url}/auth/refresh`, {
    method: "POST",
    credentials: "include",
  });
  return response.ok;
};

export async function authFetchClient(input: string | URL, init: RequestInit) {
  const response = await fetch(input, init);
  if (!response.ok && response.status === 401) {
    const refreshed = await refreshClient(CLIENT_API_URL);
    if (refreshed) {
      return await fetch(input, init);
    }
  }
  return response;
}

