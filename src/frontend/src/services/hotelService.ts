import API_BASE_URL from "./api";
import type {
  Hotel,
  SearchHotelsParams,
  PaginatedResult,
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

  const response = await fetch(
    `${API_BASE_URL}/hotels/search?${queryParams.toString()}`
  );

  if (!response.ok) {
    throw new Error("Failed to search hotels.");
  }

  return response.json();
}

export async function getHotelById(id: string): Promise<Hotel> {
  const response = await fetch(`${API_BASE_URL}/hotels/${id}`);

  if (!response.ok) {
    throw new Error("Failed to load hotel details.");
  }

  return response.json();
}

export async function getRoomTypesByHotel(
  hotelId: string
): Promise<RoomType[]> {
  const response = await fetch(
    `${API_BASE_URL}/roomtypes/hotel/${hotelId}`
  );

  if (!response.ok) {
    throw new Error("Failed to load room types.");
  }

  return response.json();
}