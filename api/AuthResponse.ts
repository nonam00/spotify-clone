import { UserDetails } from "@/types/types";

export interface AuthResponse {
  accessToken: string
  user: UserDetails;
}