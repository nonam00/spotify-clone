export const CLIENT_API_URL = "http://localhost/api/1";
export const CLIENT_FILES_URL = "http://localhost/files/api/v1";

export const SERVER_API_URL = process.env.SERVER_API_URL;
export const SERVER_FILES_URL = process.env.FILE_SERVER_URL;

const refreshClient = async (url: string): Promise<boolean> => {
  const response = await fetch(`${url}/auth/refresh`, {
    method: "POST",
    credentials: "include",
  });
  return response.ok;
};

export async function authFetchClient(input: string | URL, init: RequestInit = {}) {
  const response = await fetch(input, {
    credentials: "include",
    ...init,
  });
  
  if (!response.ok && response.status === 401) {
    const refreshed = await refreshClient(CLIENT_API_URL);
    if (refreshed) {
      return await fetch(input, {
        credentials: "include",
        ...init,
      });
    }
  }
  return response;
}