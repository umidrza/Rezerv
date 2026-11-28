import { useEffect, useState } from "react";
import { getBookingById } from "../../../services/bookingService";
import type { Booking } from "../../../types/booking";

export function useBookingDetails(id: string | undefined) {
    const [booking, setBooking] = useState<Booking | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        async function loadBooking() {
            if (!id) {
                setError("Booking ID is missing.");
                setLoading(false);
                return;
            }

            try {
                setLoading(true);
                setError("");

                const result = await getBookingById(id);
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
    }, [id]);

    return { booking, loading, error };
}