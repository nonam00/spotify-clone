import axios from "axios";
import https from "https";

export const API_URL = "https://localhost:7025/1";

const $api = axios.create({
  withCredentials: true,
  baseURL: API_URL
});

// only for development
const httpAgent = new https.Agent({
  rejectUnauthorized: false
});

$api.interceptors.request.use((config) => {
  config.httpsAgent = httpAgent;
  return config;
});

export default $api;