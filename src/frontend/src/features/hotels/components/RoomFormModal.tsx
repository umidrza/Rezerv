import { useEffect, useState } from "react";

import type {
    Room,
    CreateRoomRequest,
    UpdateRoomRequest,
} from "../../../types/room";

interface RoomFormModalProps {
    roomTypeId: string;
    room: Room | null;
    saving: boolean;
    onClose: () => void;
    onCreate: (data: CreateRoomRequest) => Promise<void>;
    onUpdate: (
        roomId: string,
        data: UpdateRoomRequest
    ) => Promise<void>;
}

export default function RoomFormModal({
    roomTypeId,
    room,
    saving,
    onClose,
    onCreate,
    onUpdate,
}: RoomFormModalProps) {
    const [roomNumber, setRoomNumber] = useState("");
    const [floor, setFloor] = useState(0);
    const [error, setError] = useState("");

    const isEditing = room !== null;

    useEffect(() => {
        setRoomNumber(room?.roomNumber ?? "");
        setFloor(room?.floor ?? 0);
        setError("");
    }, [room]);

    async function handleSubmit(
        event: React.FormEvent<HTMLFormElement>
    ) {
        event.preventDefault();

        if (!roomNumber.trim()) {
            setError("Room number is required.");
            return;
        }

        if (floor < 0) {
            setError("Floor cannot be negative.");
            return;
        }

        try {
            setError("");

            if (room) {
                await onUpdate(room.id, {
                    roomNumber: roomNumber.trim(),
                    floor,
                });
            } else {
                await onCreate({
                    roomTypeId,
                    roomNumber: roomNumber.trim(),
                    floor,
                });
            }

            onClose();
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : "Something went wrong."
            );
        }
    }

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
            <div className="w-full max-w-md rounded-xl bg-white p-6 shadow-xl">
                <h2 className="text-xl font-semibold text-slate-900">
                    {isEditing ? "Edit Room" : "Add Room"}
                </h2>

                <form onSubmit={handleSubmit} className="mt-6 space-y-4">
                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">
                            Room Number
                        </label>

                        <input
                            type="text"
                            value={roomNumber}
                            onChange={(e) =>
                                setRoomNumber(e.target.value)
                            }
                            required
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 outline-none focus:border-orange-500"
                            placeholder="e.g. 101"
                        />
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">
                            Floor
                        </label>

                        <input
                            type="number"
                            min="0"
                            value={floor}
                            onChange={(e) =>
                                setFloor(Number(e.target.value))
                            }
                            required
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 outline-none focus:border-orange-500"
                        />
                    </div>

                    {error && (
                        <p className="text-sm text-red-600">{error}</p>
                    )}

                    <div className="flex justify-end gap-3 pt-2">
                        <button
                            type="button"
                            onClick={onClose}
                            disabled={saving}
                            className="rounded-lg border border-slate-300 px-4 py-2 text-sm text-slate-700 hover:bg-slate-50"
                        >
                            Cancel
                        </button>

                        <button
                            type="submit"
                            disabled={saving}
                            className="rounded-lg bg-orange-600 px-4 py-2 text-sm font-medium text-white hover:bg-orange-700 disabled:opacity-50"
                        >
                            {saving
                                ? "Saving..."
                                : isEditing
                                  ? "Save Changes"
                                  : "Create Room"}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}