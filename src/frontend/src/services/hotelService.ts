import { apiRequest } from "./api";

import type {
    Hotel,
    SearchHotelsParams,
    PaginatedResult,
    UpdateHotelRequest,
} from "../types/hotel";

import type { RoomType } from "../types/roomType";

export async function searchHotels(
    params: SearchHotelsParams
): Promise<PaginatedResult<Hotel>> {
    const queryParams = new URLSearchParams();

    Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined && value !== null && value !== "") {
            queryParams.append(key, String(value));
        }
    });

    return apiRequest<PaginatedResult<Hotel>>(
        `/hotels/search?${queryParams.toString()}`
    );
}

export async function getHotelById(id: string): Promise<Hotel> {
    return apiRequest<Hotel>(`/hotels/${id}`);
}

export async function getRoomTypesByHotel(
    hotelId: string
): Promise<RoomType[]> {
    return apiRequest<RoomType[]>(`/roomtypes/hotel/${hotelId}`);
}

export async function getMyHotels(token: string): Promise<Hotel[]> {
    return apiRequest<Hotel[]>("/hotels/my-hotels", {}, token);
}

export async function updateHotel(
    id: string,
    data: UpdateHotelRequest,
    token: string
): Promise<Hotel> {
    return apiRequest<Hotel>(
        `/hotels/${id}`,
        {
            method: "PUT",
            body: JSON.stringify(data),
        },
        token
    );
}

export async function activateHotel(
    id: string,
    token: string
): Promise<void> {
    return apiRequest<void>(
        `/hotels/${id}/activate`,
        { method: "PUT" },
        token
    );
}

export async function deactivateHotel(
    id: string,
    token: string
): Promise<void> {
    return apiRequest<void>(
        `/hotels/${id}/deactivate`,
        { method: "PUT" },
        token
    );
}
