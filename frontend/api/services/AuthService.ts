import { AuthResponse } from "../AuthResponse";
import { AxiosResponse } from 'axios';
import Cookie from "js-cookie";

import $api from "../http";

export default class AuthService {
  static async login(email: string, password: string): Promise<AxiosResponse<AuthResponse>> {
    return $api.post<AuthResponse>('/users/login', {email, password})
  }

  static async registration(email: string, password: string) {
    return $api.post('/users/register', {email, password})
  }

  static async getUserInfo(): Promise<AxiosResponse<AuthResponse>> {
    return $api.get('/users/info', {
      headers: {
        Authorization: `Bearer ${Cookie.get("token")}`
      }
    });
  }
}
