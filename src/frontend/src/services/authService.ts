
import { apiRequest } from "./api";

export interface AuthResponse {
    userId: string;
    fullName: string;
    email: string;
    token: string;
}

export function loginUser(
    email: string,
    password: string
): Promise<AuthResponse> {
    return apiRequest<AuthResponse>("/auth/login", {
        method: "POST",
        body: JSON.stringify({ email, password }),
    });
}

export function registerUser(
    fullName: string,
    email: string,
    password: string
): Promise<AuthResponse> {
    return apiRequest<AuthResponse>("/auth/register", {
        method: "POST",
        body: JSON.stringify({ fullName, email, password }),
    });
}
