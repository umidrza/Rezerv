import { useCallback, useEffect, useState } from "react";

import { getMyBookings } from "../../../services/bookingService";
import type { Booking } from "../../../types/booking";

export function useMyBookings() {
    const [bookings, setBookings] = useState<Booking[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const loadBookings = useCallback(async () => {
        try {
            setLoading(true);
            setError("");

            const result = await getMyBookings();
            setBookings(result);
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : "Failed to load your bookings."
            );
        } finally {
            setLoading(false);
        }
    }, []);

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
