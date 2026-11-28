import { apiRequest } from "./api";
import type { Hotel } from "../types/hotel";

export function getMyFavorites(): Promise<Hotel[]> {
    return apiRequest<Hotel[]>("/favorites");
}

export function addFavorite(hotelId: string): Promise<void> {
    return apiRequest<void>(
        `/favorites/${encodeURIComponent(hotelId)}`,
        { method: "POST" }
    );
}

export function removeFavorite(hotelId: string): Promise<void> {
    return apiRequest<void>(
        `/favorites/${encodeURIComponent(hotelId)}`,
        { method: "DELETE" }
    );
}

export function isFavorite(hotelId: string): Promise<boolean> {
    return apiRequest<boolean>(
        `/favorites/${encodeURIComponent(hotelId)}/status`
    );
}