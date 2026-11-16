import { useEffect, useState } from "react";
import type { RoomType } from "../../../types/roomType";
import type { Room } from "../../../types/room";
import { getRoomsByRoomType } from "../../../services/roomService";

interface RoomCardProps {
  roomType: RoomType;
  selectedRoomIds: string[];
  onRoomSelectionChange: (roomId: string, selected: boolean) => void;
}

export default function RoomCard({
  roomType,
  selectedRoomIds,
  onRoomSelectionChange,
}: RoomCardProps) {
  const [rooms, setRooms] = useState<Room[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    async function loadRooms() {
      try {
        const result = await getRoomsByRoomType(roomType.id);
        setRooms(result.filter((room) => room.isActive));
      } catch {
        setError("Failed to load rooms.");
      } finally {
        setLoading(false);
      }
    }

    loadRooms();
  }, [roomType.id]);

  return (
    <article className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
      <h3 className="text-xl font-bold text-slate-900">
        {roomType.name}
      </h3>

      <p className="mt-2 text-slate-600">
        {roomType.description}
      </p>

      <p className="mt-3 text-sm text-slate-500">
        Capacity: {roomType.capacity} guests
      </p>

      <p className="mt-4 text-xl font-bold text-orange-600">
        ${roomType.pricePerNight}
        <span className="text-sm font-normal text-slate-500">
          {" "}/ night
        </span>
      </p>

      <div className="mt-6 border-t pt-4">
        <h4 className="mb-3 font-semibold text-slate-900">
          Select rooms
        </h4>

        {loading && <p>Loading rooms...</p>}

        {error && <p className="text-red-600">{error}</p>}

        {!loading && !error && rooms.length === 0 && (
          <p className="text-sm text-slate-500">
            No active rooms available.
          </p>
        )}

        <div className="space-y-3">
          {rooms.map((room) => (
            <label
              key={room.id}
              className="flex cursor-pointer items-center justify-between rounded-lg border p-3 hover:border-orange-400"
            >
              <span>
                Room {room.roomNumber}
                <span className="ml-2 text-sm text-slate-500">
                  Floor {room.floor}
                </span>
              </span>

              <input
                type="checkbox"
                checked={selectedRoomIds.includes(room.id)}
                onChange={(e) =>
                  onRoomSelectionChange(room.id, e.target.checked)
                }
                className="h-4 w-4 accent-orange-500"
              />
            </label>
          ))}
        </div>
      </div>
    </article>
  );
}
