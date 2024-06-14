import axios from "axios";
import https from "https";
import cookie from "react-cookies";

export const API_URL = "https://localhost:7025/1";

const $api = axios.create({
  baseURL: API_URL,
  withCredentials: true,
  xsrfCookieName: ".AspNetCore.Xsrf",
  xsrfHeaderName: "x-xsrf-token",
});

$api.defaults.headers.common['x-xsrf-token'] = cookie.load(".AspNetCore.Xsrf");

// only for development
const httpAgent = new https.Agent({
  rejectUnauthorized: false
});

$api.interceptors.request.use((config) => {
  config.httpsAgent = httpAgent;
  return config;
});

export default $api;