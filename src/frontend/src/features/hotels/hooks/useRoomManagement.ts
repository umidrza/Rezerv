import { useCallback, useEffect, useState } from "react";

import {
    getRoomsByRoomType,
    createRoom,
    updateRoom,
    toggleRoomStatus,
} from "../../../services/roomService";

import type {
    Room,
    CreateRoomRequest,
    UpdateRoomRequest,
} from "../../../types/room";

export function useRoomManagement(
    roomTypeId: string,
    token: string | null
) {
    const [rooms, setRooms] = useState<Room[]>([]);
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");

    const loadRooms = useCallback(async () => {
        try {
            setLoading(true);
            setError("");

            const result = await getRoomsByRoomType(roomTypeId);
            setRooms(result);
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : "Failed to load rooms."
            );
        } finally {
            setLoading(false);
        }
    }, [roomTypeId]);

    useEffect(() => {
        loadRooms();
    }, [loadRooms]);

    async function addRoom(data: CreateRoomRequest) {
        if (!token) return;

        try {
            setSaving(true);
            setError("");
            setSuccess("");

            const room = await createRoom(data, token);

            setRooms((current) => [...current, room]);
            setSuccess("Room created successfully.");
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : "Failed to create room."
            );
            throw err;
        } finally {
            setSaving(false);
        }
    }

    async function editRoom(
        id: string,
        data: UpdateRoomRequest
    ) {
        if (!token) return;

        try {
            setSaving(true);
            setError("");
            setSuccess("");

            const updatedRoom = await updateRoom(id, data, token);

            setRooms((current) =>
                current.map((room) =>
                    room.id === id ? updatedRoom : room
                )
            );

            setSuccess("Room updated successfully.");
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : "Failed to update room."
            );
            throw err;
        } finally {
            setSaving(false);
        }
    }

    async function changeRoomStatus(id: string) {
        if (!token) return;

        try {
            setSaving(true);
            setError("");
            setSuccess("");

            const updatedRoom = await toggleRoomStatus(id, token);

            setRooms((current) =>
                current.map((room) =>
                    room.id === id ? updatedRoom : room
                )
            );

            setSuccess("Room status updated successfully.");
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : "Failed to update room status."
            );
            throw err;
        } finally {
            setSaving(false);
        }
    }

    return {
        rooms,
        loading,
        saving,
        error,
        success,
        setError,
        setSuccess,
        loadRooms,
        addRoom,
        editRoom,
        changeRoomStatus,
    };
}