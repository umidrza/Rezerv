
import { apiRequest } from "./api";

import type {
    Hotel,
    SearchHotelsParams,
    PaginatedResult,
    UpdateHotelRequest,
} from "../types/hotel";

import type { RoomType } from "../types/roomType";

export function searchHotels(
    params: SearchHotelsParams
): Promise<PaginatedResult<Hotel>> {
    const queryParams = new URLSearchParams();

    Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined && value !== null && value !== "") {
            queryParams.append(key, String(value));
        }
    });

    const query = queryParams.toString();
    const endpoint = query
        ? `/hotels/search?${query}`
        : "/hotels/search";

    return apiRequest<PaginatedResult<Hotel>>(endpoint);
}

export function getHotelById(id: string): Promise<Hotel> {
    return apiRequest<Hotel>(
        `/hotels/${encodeURIComponent(id)}`
    );
}

export function getRoomTypesByHotel(
    hotelId: string
): Promise<RoomType[]> {
    return apiRequest<RoomType[]>(
        `/roomtypes/hotel/${encodeURIComponent(hotelId)}`
    );
}

export function getMyHotels(token: string): Promise<Hotel[]> {
    return apiRequest<Hotel[]>("/hotels/my-hotels", {}, token);
}

export function updateHotel(
    id: string,
    data: UpdateHotelRequest,
    token: string
): Promise<Hotel> {
    return apiRequest<Hotel>(
        `/hotels/${encodeURIComponent(id)}`,
        {
            method: "PUT",
            body: JSON.stringify(data),
        },
        token
    );
}

export function activateHotel(
    id: string,
    token: string
): Promise<void> {
    return apiRequest<void>(
        `/hotels/${encodeURIComponent(id)}/activate`,
        { method: "PUT" },
        token
    );
}

export function deactivateHotel(
    id: string,
    token: string
): Promise<void> {
    return apiRequest<void>(
        `/hotels/${encodeURIComponent(id)}/deactivate`,
        { method: "PUT" },
        token
    );
}
