import { createContext, useContext, useEffect, useState } from "react";
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
        const decoded = JSON.parse(
            atob(payload.replace(/-/g, "+").replace(/_/g, "/"))
        );

        const roleClaim =
            decoded.role ??
            decoded.roles ??
            decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

        if (Array.isArray(roleClaim)) {
            return roleClaim;
        }

        return typeof roleClaim === "string" ? [roleClaim] : [];
    } catch {
        return [];
    }
}

export function AuthProvider({ children }: AuthProviderProps) {
    const [token, setToken] = useState<string | null>(
        () => localStorage.getItem("token")
    );

    const roles = getUserRoles(token);

    const isAuthenticated = Boolean(token);
    const isAdmin = roles.includes("Admin");
    const isHotelManager = roles.includes("HotelManager");
    const isCustomer = roles.includes("Customer");

    function login(token: string) {
        localStorage.setItem("token", token);
        setToken(token);
    }

    function logout() {
        localStorage.removeItem("token");
        localStorage.removeItem("user");
        setToken(null);
    }

    useEffect(() => {
        function handleUnauthorized() {
            logout();
        }

        window.addEventListener("auth:unauthorized", handleUnauthorized);

        return () => {
            window.removeEventListener("auth:unauthorized", handleUnauthorized);
        };
    }, []);

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
