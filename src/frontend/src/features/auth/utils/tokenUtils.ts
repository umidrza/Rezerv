import type { AuthUser } from "../../../types/auth";

export function getUserRoles(user: AuthUser | null): string[] {
    return user?.roles ?? [];
}
