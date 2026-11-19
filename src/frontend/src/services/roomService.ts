import { apiRequest } from "./api";
import type { Room } from "../types/room";

export async function getRoomsByRoomType(
    roomTypeId: string
): Promise<Room[]> {
    return apiRequest<Room[]>(`/rooms/roomtype/${roomTypeId}`);
}
