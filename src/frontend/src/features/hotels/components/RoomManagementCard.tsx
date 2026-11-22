import type { Room } from "../../../types/room";

interface RoomManagementCardProps {
    room: Room;
    saving: boolean;
    onEdit: (room: Room) => void;
    onToggleStatus: (roomId: string) => void;
}

export default function RoomManagementCard({
    room,
    saving,
    onEdit,
    onToggleStatus,
}: RoomManagementCardProps) {
    return (
        <div className="rounded-xl border border-slate-200 bg-white p-5 shadow-sm">
            <div className="flex items-start justify-between">
                <div>
                    <h3 className="text-lg font-semibold text-slate-900">
                        Room {room.roomNumber}
                    </h3>

                    <p className="mt-1 text-sm text-slate-500">
                        Floor {room.floor}
                    </p>
                </div>

                <span
                    className={`rounded-full px-3 py-1 text-xs font-medium ${
                        room.isActive
                            ? "bg-green-100 text-green-700"
                            : "bg-slate-100 text-slate-600"
                    }`}
                >
                    {room.isActive ? "Active" : "Inactive"}
                </span>
            </div>

            <div className="mt-5 flex gap-3">
                <button
                    type="button"
                    onClick={() => onEdit(room)}
                    disabled={saving}
                    className="flex-1 rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-50"
                >
                    Edit
                </button>

                <button
                    type="button"
                    onClick={() => onToggleStatus(room.id)}
                    disabled={saving}
                    className={`flex-1 rounded-lg px-4 py-2 text-sm font-medium disabled:opacity-50 ${
                        room.isActive
                            ? "bg-red-50 text-red-600 hover:bg-red-100"
                            : "bg-green-50 text-green-700 hover:bg-green-100"
                    }`}
                >
                    {room.isActive ? "Deactivate" : "Activate"}
                </button>
            </div>
        </div>
    );
}