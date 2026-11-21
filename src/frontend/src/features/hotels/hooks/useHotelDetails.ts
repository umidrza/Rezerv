
import { useEffect, useState } from "react";
import { getHotelById, getRoomTypesByHotel } from "../../../services/hotelService";
import type { Hotel } from "../../../types/hotel";
import type { RoomType } from "../../../types/roomType";

export function useHotelDetails(id: string | undefined) {
    const [hotel, setHotel] = useState<Hotel | null>(null);
    const [roomTypes, setRoomTypes] = useState<RoomType[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        if (!id) {
            setError("Hotel not found.");
            setLoading(false);
            return;
        }

        const hotelId = id;
        let isActive = true;

        async function loadHotel() {
            try {
                setLoading(true);
                setError("");

                const [hotelResult, roomTypesResult] = await Promise.all([
                    getHotelById(hotelId),
                    getRoomTypesByHotel(hotelId),
                ]);

                if (!isActive) return;

                setHotel(hotelResult);
                setRoomTypes(roomTypesResult);
            } catch (err) {
                if (!isActive) return;

                setError(
                    err instanceof Error
                        ? err.message
                        : "Failed to load hotel details."
                );
            } finally {
                if (isActive) setLoading(false);
            }
        }

        loadHotel();

        return () => {
            isActive = false;
        };
    }, [id]);

    return { hotel, roomTypes, loading, error };
}
