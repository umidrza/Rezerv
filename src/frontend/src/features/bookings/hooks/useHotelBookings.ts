
import { useCallback, useEffect, useState } from "react";

import { getHotelBookings } from "../../../services/bookingService";
import type { Booking } from "../../../types/booking";

export function useHotelBookings(
    hotelId: string | undefined
) {
    const [bookings, setBookings] = useState<Booking[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const loadBookings = useCallback(async () => {
        if (!hotelId) {
            setError("Unable to load hotel bookings.");
            setLoading(false);
            return;
        }

        try {
            setLoading(true);
            setError("");

            const result = await getHotelBookings(hotelId);
            setBookings(result);
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : "Failed to load hotel bookings."
            );
        } finally {
            setLoading(false);
        }
    }, [hotelId]);

    useEffect(() => {
        loadBookings();
    }, [loadBookings]);

    return {
        bookings,
        setBookings,
        loading,
        error,
        loadBookings,
    };
}
