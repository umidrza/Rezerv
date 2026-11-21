
const API_BASE_URL =
    import.meta.env.VITE_API_BASE_URL || "http://localhost:5218/api";

export async function apiRequest<T>(
    endpoint: string,
    options: RequestInit = {},
    token?: string | null
): Promise<T> {
    const headers = new Headers(options.headers);

    if (
        typeof options.body === "string" &&
        !headers.has("Content-Type")
    ) {
        headers.set("Content-Type", "application/json");
    }

    if (token) {
        headers.set("Authorization", `Bearer ${token}`);
    }

    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
        ...options,
        headers,
    });

    if (response.status === 401) {
        window.dispatchEvent(new Event("auth:unauthorized"));
        throw new Error("Your session has expired. Please log in again.");
    }

    if (!response.ok) {
        const error = await response.json().catch(() => null);

        throw new Error(
            error?.message || `Request failed with status ${response.status}.`
        );
    }

    if (response.status === 204) {
        return undefined as T;
    }

    return response.json();
}

export default API_BASE_URL;
