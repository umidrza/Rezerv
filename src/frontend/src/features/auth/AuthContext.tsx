import { createContext, useContext, useState, useEffect, useCallback } from "react";
import type { ReactNode } from "react";

interface AuthContextType {
    token: string | null;
    isAuthenticated: boolean;
    isAdmin: boolean;
    isHotelManager: boolean;
    isCustomer: boolean;
    login: (token: string) => void;
    logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

interface AuthProviderProps {
    children: ReactNode;
}


function getUserRoles(token: string | null): string[] {
    if (!token) return [];

    try {
        const payload = token.split(".")[1];

        if (!payload) return [];

        const normalizedPayload = payload
            .replace(/-/g, "+")
            .replace(/_/g, "/");

        const decoded = JSON.parse(atob(normalizedPayload));

        const roleClaims = [
            decoded.role,
            decoded.roles,
            decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"],
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
    } catch {
        return [];
    }
}


function isTokenValid(token: string | null): boolean {
    if (!token) return false;

    try {
        const payload = token.split(".")[1];
        const decoded = JSON.parse(
            atob(payload.replace(/-/g, "+").replace(/_/g, "/"))
        );

        return decoded.exp && decoded.exp > Date.now() / 1000;
    } catch {
        return false;
    }
}

export function AuthProvider({ children }: AuthProviderProps) {
    const [token, setToken] = useState<string | null>(() => {
        const savedToken = localStorage.getItem("token");

        if (!isTokenValid(savedToken)) {
            localStorage.removeItem("token");
            return null;
        }

        return savedToken;
    });

    const roles = getUserRoles(token);

    const isAuthenticated = Boolean(token);
    const isAdmin = roles.includes("Admin");
    const isHotelManager = roles.includes("HotelManager");
    const isCustomer = roles.includes("Customer");

    const login = useCallback((token: string) => {
        if (!isTokenValid(token)) {
            throw new Error("Invalid or expired token.");
        }

        localStorage.setItem("token", token);
        setToken(token);
    }, []);

    const logout = useCallback(() => {
        localStorage.removeItem("token");
        localStorage.removeItem("user");
        setToken(null);
    }, []);

    useEffect(() => {
        function handleUnauthorized() {
            logout();
        }

        window.addEventListener("auth:unauthorized", handleUnauthorized);

        return () => {
            window.removeEventListener("auth:unauthorized", handleUnauthorized);
        };
    }, [logout]);

    return (
        <AuthContext.Provider
            value={{
                token,
                isAuthenticated,
                isAdmin,
                isHotelManager,
                isCustomer,
                login,
                logout,
            }}
        >
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    const context = useContext(AuthContext);

    if (!context) {
        throw new Error("useAuth must be used inside AuthProvider.");
    }

    return context;
}
