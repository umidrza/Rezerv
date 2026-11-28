
const API_BASE_URL =
    import.meta.env.VITE_API_BASE_URL || "http://localhost:5218/api";

export class ApiError extends Error {
    public readonly status: number;

    constructor(message: string, status: number) {
        super(message);
        this.name = "ApiError";
        this.status = status;
    }
}

export async function apiRequest<T>(
    endpoint: string,
    options: RequestInit = {}
): Promise<T> {
    const headers = new Headers(options.headers);

    if (
        typeof options.body === "string" &&
        !headers.has("Content-Type")
    ) {
        headers.set("Content-Type", "application/json");
    }

    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
        ...options,
        credentials: "include",
        headers,
    });

    if (response.status === 401) {
        if (
            endpoint !== "/auth/login" &&
            endpoint !== "/auth/register" &&
            endpoint !== "/auth/me"
        ) {
            window.dispatchEvent(new Event("auth:unauthorized"));
        }

        const error = await response.json().catch(() => null);
        throw new ApiError(
            error?.message || "Your session has expired. Please log in again.",
            response.status
        );
    }

    if (!response.ok) {
        const error = await response.json().catch(() => null);
        const validationMessage = error?.errors
            ? Object.values(error.errors).flat()[0]
            : undefined;

        throw new ApiError(
            error?.message ||
                error?.detail ||
                validationMessage ||
                `Request failed with status ${response.status}.`,
            response.status
        );
    }

    if (response.status === 204) {
        return undefined as T;
    }

    return response.json();
}

export default API_BASE_URL;
