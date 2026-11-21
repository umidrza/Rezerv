import { useEffect, useState } from "react";
import { getBookingById } from "../../../services/bookingService";
import type { Booking } from "../../../types/booking";

export function useBookingDetails(
    id: string | undefined,
    token: string | null
) {
    const [booking, setBooking] = useState<Booking | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        async function loadBooking() {
            if (!id || !token) {
                setError(
                    !id ? "Booking ID is missing." : "Authentication required."
                );
                setLoading(false);
                return;
            }

            try {
                setLoading(true);
                setError("");

                const result = await getBookingById(id, token);
                setBooking(result);
            } catch (err) {
                setError(
                    err instanceof Error
                        ? err.message
                        : "Failed to load booking."
                );
            } finally {
                setLoading(false);
            }
        }

        loadBooking();
    }, [id, token]);

    return { booking, loading, error };
}