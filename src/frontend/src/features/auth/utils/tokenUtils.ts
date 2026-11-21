
interface JwtPayload {
    exp?: number;
    role?: string | string[];
    roles?: string | string[];
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"?: string | string[];
}

function decodeToken(token: string): JwtPayload | null {
    try {
        const payload = token.split(".")[1];

        if (!payload) return null;

        const normalizedPayload = payload
            .replace(/-/g, "+")
            .replace(/_/g, "/");

        return JSON.parse(atob(normalizedPayload));
    } catch {
        return null;
    }
}

export function isTokenValid(token: string | null): boolean {
    if (!token) return false;

    const payload = decodeToken(token);

    return Boolean(payload?.exp && payload.exp > Date.now() / 1000);
}

export function getUserRoles(token: string | null): string[] {
    if (!token) return [];

    const payload = decodeToken(token);

    if (!payload) return [];

    const roleClaims = [
        payload.role,
        payload.roles,
        payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"],
    ];

    return [
        ...new Set(
            roleClaims.flatMap((claim) =>
                Array.isArray(claim)
                    ? claim
                    : typeof claim === "string"
                        ? [claim]
                        : []
            )
        ),
    ];
}
