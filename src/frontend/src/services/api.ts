
const API_BASE_URL = "http://localhost:5218/api";

function isTokenExpired(token: string): boolean {
    try {
        const payload = token.split(".")[1];

        if (!payload) return true;

        const decoded = JSON.parse(
            atob(payload.replace(/-/g, "+").replace(/_/g, "/"))
        );

        return !decoded.exp || decoded.exp <= Date.now() / 1000;
    } catch {
        return true;
    }
}

export async function apiRequest<T>(
    endpoint: string,
    options: RequestInit = {},
    token?: string | null
): Promise<T> {
    const headers = new Headers(options.headers);

    if (options.body && !headers.has("Content-Type")) {
        headers.set("Content-Type", "application/json");
    }

    if (token) {
        headers.set("Authorization", `Bearer ${token}`);
    }

    if (token && isTokenExpired(token)) {
        window.dispatchEvent(new Event("auth:unauthorized"));
        throw new Error("Your session has expired. Please log in again.");
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
