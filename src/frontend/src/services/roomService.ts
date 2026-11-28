import { apiRequest } from "./api";

import type {
    Room,
    CreateRoomRequest,
    UpdateRoomRequest,
} from "../types/room";

export function getRoomsByRoomType(
    roomTypeId: string
): Promise<Room[]> {
    return apiRequest<Room[]>(
        `/rooms/roomtype/${encodeURIComponent(roomTypeId)}`
    );
}

export function createRoom(
    data: CreateRoomRequest
): Promise<Room> {
    return apiRequest<Room>(
        "/rooms",
        {
            method: "POST",
            body: JSON.stringify(data),
        }
    );
}

export function updateRoom(
    id: string,
    data: UpdateRoomRequest
): Promise<Room> {
    return apiRequest<Room>(
        `/rooms/${encodeURIComponent(id)}`,
        {
            method: "PUT",
            body: JSON.stringify(data),
        }
    );
}

export function toggleRoomStatus(id: string): Promise<Room> {
    return apiRequest<Room>(
        `/rooms/${encodeURIComponent(id)}/status`,
        {
            method: "PATCH",
        }
    );
}