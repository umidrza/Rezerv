
import { apiRequest } from "./api";
import type { AuthUser } from "../types/auth";

export function loginUser(
    email: string,
    password: string
): Promise<AuthUser> {
    return apiRequest<AuthUser>("/auth/login", {
        method: "POST",
        body: JSON.stringify({ email, password }),
    });
}

export function registerUser(
    fullName: string,
    email: string,
    password: string
): Promise<AuthUser> {
    return apiRequest<AuthUser>("/auth/register", {
        method: "POST",
        body: JSON.stringify({ fullName, email, password }),
    });
}

export function getCurrentUser(): Promise<AuthUser> {
    return apiRequest<AuthUser>("/auth/me");
}

export function logoutUser(): Promise<void> {
    return apiRequest<void>("/auth/logout", { method: "POST" });
}
