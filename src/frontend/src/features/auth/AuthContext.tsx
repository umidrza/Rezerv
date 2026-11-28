import { createContext, useContext, useState, useEffect, useCallback, useRef } from "react";
import type { ReactNode } from "react";
import { getUserRoles } from "./utils/tokenUtils";
import { getCurrentUser, logoutUser } from "../../services/authService";
import { ApiError } from "../../services/api";
import type { AuthUser } from "../../types/auth";

interface AuthContextType {
    user: AuthUser | null;
    isAuthenticated: boolean;
    isLoading: boolean;
    isAdmin: boolean;
    isHotelManager: boolean;
    isCustomer: boolean;
    login: (user: AuthUser) => void;
    logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

interface AuthProviderProps {
    children: ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
    const [user, setUser] = useState<AuthUser | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const sessionGeneration = useRef(0);

    const roles = getUserRoles(user);

    const isAuthenticated = Boolean(user);
    const isAdmin = roles.includes("Admin");
    const isHotelManager = roles.includes("HotelManager");
    const isCustomer = roles.includes("Customer");

    const login = useCallback((authenticatedUser: AuthUser) => {
        sessionGeneration.current += 1;
        setUser(authenticatedUser);
        setIsLoading(false);
    }, []);

    const logout = useCallback(async () => {
        const generation = ++sessionGeneration.current;
        await logoutUser();
        if (generation === sessionGeneration.current) {
            setUser(null);
            setIsLoading(false);
        }
    }, []);

    useEffect(() => {
        let isMounted = true;
        const generation = sessionGeneration.current;

        async function hydrateUser() {
            try {
                const currentUser = await getCurrentUser();
                if (isMounted && generation === sessionGeneration.current) {
                    setUser(currentUser);
                }
            } catch (error) {
                if (
                    isMounted &&
                    generation === sessionGeneration.current &&
                    !(error instanceof ApiError && error.status === 401)
                ) {
                    console.error("Failed to restore the current session.", error);
                }
            } finally {
                if (isMounted && generation === sessionGeneration.current) {
                    setIsLoading(false);
                }
            }
        }

        void hydrateUser();
        return () => {
            isMounted = false;
        };
    }, []);

    useEffect(() => {
        function handleUnauthorized() {
            sessionGeneration.current += 1;
            setUser(null);
            setIsLoading(false);
        }

        window.addEventListener("auth:unauthorized", handleUnauthorized);

        return () => {
            window.removeEventListener("auth:unauthorized", handleUnauthorized);
        };
    }, []);

    return (
        <AuthContext.Provider
            value={{
                user,
                isAuthenticated,
                isLoading,
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
