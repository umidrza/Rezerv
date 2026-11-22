import { useState } from "react";
import { useParams } from "react-router";

import { useAuth } from "../../auth/AuthContext";
import { useRoomManagement } from "../hooks/useRoomManagement";

import RoomManagementCard from "../components/RoomManagementCard";
import RoomFormModal from "../components/RoomFormModal";

import LoadingState from "../../../components/LoadingState";
import ErrorMessage from "../../../components/ErrorMessage";

import type { Room } from "../../../types/room";

export default function RoomManagementPage() {
    const { roomTypeId } = useParams();
    const { token } = useAuth();

    const {
        rooms,
        loading,
        saving,
        error,
        success,
        addRoom,
        editRoom,
        changeRoomStatus,
    } = useRoomManagement(roomTypeId ?? "", token);

    const [selectedRoom, setSelectedRoom] = useState<Room | null>(null);
    const [isModalOpen, setIsModalOpen] = useState(false);

    function handleAddRoom() {
        setSelectedRoom(null);
        setIsModalOpen(true);
    }

    function handleEditRoom(room: Room) {
        setSelectedRoom(room);
        setIsModalOpen(true);
    }

    function handleCloseModal() {
        setIsModalOpen(false);
        setSelectedRoom(null);
    }

    async function handleToggleStatus(roomId: string) {
        try {
            await changeRoomStatus(roomId);
        } catch {
            // The hook already updates the error state.
        }
    }

    if (!roomTypeId) {
        return (
            <ErrorMessage message="Room type ID is missing." />
        );
    }

    if (loading) {
        return <LoadingState message="Loading rooms..." />;
    }

    return (
        <div className="mx-auto max-w-7xl px-4 py-10">
            <div className="mb-8 flex items-center justify-between">
                <div>
                    <h1 className="text-3xl font-bold text-slate-900">
                        Room Management
                    </h1>
                    <p className="mt-2 text-slate-600">
                        Manage rooms for this room type.
                    </p>
                </div>

                <button
                    type="button"
                    onClick={handleAddRoom}
                    className="rounded-lg bg-orange-600 px-5 py-3 text-sm font-medium text-white hover:bg-orange-700"
                >
                    Add Room
                </button>
            </div>

            {error && (
                <div className="mb-4">
                    <ErrorMessage message={error} />
                </div>
            )}

            {success && (
                <div className="mb-4 rounded-lg bg-green-50 px-4 py-3 text-sm text-green-700">
                    {success}
                </div>
            )}

            {rooms.length === 0 ? (
                <div className="rounded-xl bg-white p-10 text-center shadow-sm">
                    <p className="text-slate-600">
                        No rooms have been added to this room type yet.
                    </p>
                </div>
            ) : (
                <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
                    {rooms.map((room) => (
                        <RoomManagementCard
                            key={room.id}
                            room={room}
                            saving={saving}
                            onEdit={handleEditRoom}
                            onToggleStatus={handleToggleStatus}
                        />
                    ))}
                </div>
            )}

            {isModalOpen && (
                <RoomFormModal
                    roomTypeId={roomTypeId}
                    room={selectedRoom}
                    saving={saving}
                    onClose={handleCloseModal}
                    onCreate={addRoom}
                    onUpdate={editRoom}
                />
            )}
        </div>
    );
}