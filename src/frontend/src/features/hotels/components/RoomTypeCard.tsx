
import type { RoomType } from "../../../types/roomType";
import RoomCard from "./RoomCard";

interface RoomTypeCardProps {
    roomType: RoomType;
    selectedRoomId: string | null;
    onRoomSelect: (roomId: string) => void;
}

export default function RoomTypeCard({
    roomType,
    selectedRoomId,
    onRoomSelect,
}: RoomTypeCardProps) {
    return (
        <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
            <div className="flex flex-wrap justify-between gap-3">
                <div>
                    <h3 className="text-xl font-semibold text-slate-900">
                        {roomType.name}
                    </h3>

                    <p className="mt-2 text-sm text-slate-500">
                        {roomType.description}
                    </p>

                    <p className="mt-2 text-sm text-slate-600">
                        Capacity: {roomType.capacity} guests
                    </p>
                </div>

                <div className="text-right">
                    <p className="text-xl font-bold text-orange-600">
                        ${roomType.pricePerNight.toFixed(2)}
                    </p>
                    <p className="text-sm text-slate-500">per night</p>
                </div>
            </div>

            <RoomCard
                roomTypeId={roomType.id}
                selectedRoomId={selectedRoomId}
                onRoomSelect={onRoomSelect}
            />
        </div>
    );
}
