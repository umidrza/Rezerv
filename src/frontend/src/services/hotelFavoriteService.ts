import { apiRequest } from "./api";
import type { Hotel } from "../types/hotel";

export function getMyFavorites(token: string): Promise<Hotel[]> {
    return apiRequest<Hotel[]>("/favorites", {}, token);
}

export function addFavorite(
    hotelId: string,
    token: string
): Promise<void> {
    return apiRequest<void>(
        `/favorites/${encodeURIComponent(hotelId)}`,
        { method: "POST" },
        token
    );
}

export function removeFavorite(
    hotelId: string,
    token: string
): Promise<void> {
    return apiRequest<void>(
        `/favorites/${encodeURIComponent(hotelId)}`,
        { method: "DELETE" },
        token
    );
}

export function isFavorite(
    hotelId: string,
    token: string
): Promise<boolean> {
    return apiRequest<boolean>(
        `/favorites/${encodeURIComponent(hotelId)}/status`,
        {},
        token
    );
}