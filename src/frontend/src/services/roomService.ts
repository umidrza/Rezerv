import API_BASE_URL from "./api";
import type { Room } from "../types/room";

export async function getRoomsByRoomType(
  roomTypeId: string
): Promise<Room[]> {
  const response = await fetch(
    `${API_BASE_URL}/rooms/roomtype/${roomTypeId}`
  );

  if (!response.ok) {
    throw new Error("Failed to load rooms.");
  }

  return response.json();
}