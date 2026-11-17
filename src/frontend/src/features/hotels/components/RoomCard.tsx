import { useEffect, useState } from "react";
import type { Room } from "../../../types/room";
import { getRoomsByRoomType } from "../../../services/roomService";

interface RoomCardProps {
    roomTypeId: string;
    selectedRoomId: string | null;
    onRoomSelect: (roomId: string) => void;
}

export default function RoomCard({
    roomTypeId,
    selectedRoomId,
    onRoomSelect,
}: RoomCardProps) {
    const [rooms, setRooms] = useState<Room[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        const loadRooms = async () => {
            try {
                setLoading(true);
                setError("");

                const result = await getRoomsByRoomType(roomTypeId);

                setRooms(result.filter((room) => room.isActive));
            } catch (err) {
                setError(
                    err instanceof Error
                        ? err.message
                        : "Failed to load rooms."
                );
            } finally {
                setLoading(false);
            }
        };

        loadRooms();
    }, [roomTypeId]);

    if (loading) {
        return <p className="text-sm text-slate-500">Loading rooms...</p>;
    }

    if (error) {
        return <p className="text-sm text-red-600">{error}</p>;
    }

    if (rooms.length === 0) {
        return <p className="text-sm text-slate-500">No rooms available.</p>;
    }

    return (
        <div className="mt-4 space-y-3">
            <h4 className="font-semibold text-slate-800">
                Select a room
            </h4>

            {rooms.map((room) => (
                <label
                    key={room.id}
                    className={`flex cursor-pointer items-center justify-between rounded-lg border p-4 transition ${
                        selectedRoomId === room.id
                            ? "border-orange-500 bg-orange-50"
                            : "border-slate-200 hover:border-orange-300"
                    }`}
                >
                    <div>
                        <p className="font-medium text-slate-900">
                            Room {room.roomNumber}
                        </p>

                        <p className="text-sm text-slate-500">
                            Floor {room.floor}
                        </p>
                    </div>

                    <input
                        type="radio"
                        name={`room-${roomTypeId}`}
                        value={room.id}
                        checked={selectedRoomId === room.id}
                        onChange={() => onRoomSelect(room.id)}
                        className="h-4 w-4 accent-orange-500"
                    />
                </label>
            ))}
        </div>
    );
}