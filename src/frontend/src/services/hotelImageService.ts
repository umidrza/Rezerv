import { apiRequest } from "./api";
import type { HotelImage } from "../types/hotel";

export async function uploadHotelImage(
    hotelId: string,
    file: File
): Promise<HotelImage> {
    const formData = new FormData();

    formData.append("HotelId", hotelId);
    formData.append("File", file);

    return apiRequest<HotelImage>(
        "/hotel-images",
        {
            method: "POST",
            body: formData,
        }
    );
}

export function getHotelImages(
    hotelId: string
): Promise<HotelImage[]> {
    return apiRequest<HotelImage[]>(
        `/hotel-images/hotel/${encodeURIComponent(hotelId)}`
    );
}

export function setPrimaryHotelImage(
    hotelId: string,
    imageId: string
): Promise<void> {
    return apiRequest<void>(
        `/hotel-images/${encodeURIComponent(hotelId)}/${encodeURIComponent(imageId)}/primary`,
        { method: "PUT" }
    );
}

export function deleteHotelImage(
    hotelId: string,
    imageId: string
): Promise<void> {
    return apiRequest<void>(
        `/hotel-images/${encodeURIComponent(hotelId)}/${encodeURIComponent(imageId)}`,
        { method: "DELETE" }
    );
}