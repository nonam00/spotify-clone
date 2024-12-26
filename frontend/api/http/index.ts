const env_var = process.env.NEXT_PUBLIC_API_URL;
export const API_URL =  env_var ?? "http://localhost:5000/1";
export const SERVER_API = env_var ?? "http://host.docker.internal:5000/1"
console.log(SERVER_API);