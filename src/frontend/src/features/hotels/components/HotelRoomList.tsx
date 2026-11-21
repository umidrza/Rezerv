
import type { RoomType } from "../../../types/roomType";
import RoomTypeCard from "./RoomTypeCard";

interface HotelRoomListProps {
    roomTypes: RoomType[];
    selectedRoomId: string | null;
    onRoomSelect: (roomId: string) => void;
}

export default function HotelRoomList({
    roomTypes,
    selectedRoomId,
    onRoomSelect,
}: HotelRoomListProps) {
    return (
        <section className="space-y-6 lg:col-span-2">
            <h2 className="text-2xl font-bold text-slate-900">
                Available room types
            </h2>

            {roomTypes.length === 0 ? (
                <div className="rounded-xl bg-white p-6 text-slate-500">
                    No room types found for this hotel.
                </div>
            ) : (
                roomTypes.map((roomType) => (
                    <RoomTypeCard
                        key={roomType.id}
                        roomType={roomType}
                        selectedRoomId={selectedRoomId}
                        onRoomSelect={onRoomSelect}
                    />
                ))
            )}
        </section>
    );
}
