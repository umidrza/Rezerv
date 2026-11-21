
import { useState } from "react";

import {
    approveBooking,
    rejectBooking,
    cancelBooking,
} from "../../../services/bookingService";

export type BookingAction = "approve" | "reject" | "cancel";

export function useBookingActions(token: string | null) {
    const [processingBookingId, setProcessingBookingId] =
        useState<string | null>(null);

    const [processingAction, setProcessingAction] =
        useState<BookingAction | null>(null);

    const [error, setError] = useState("");

    async function executeAction(
        bookingId: string,
        action: BookingAction
    ): Promise<boolean> {
        if (!token || processingBookingId !== null) {
            setError("Unable to process this booking.");
            return false;
        }

        setProcessingBookingId(bookingId);
        setProcessingAction(action);
        setError("");

        try {
            switch (action) {
                case "approve":
                    await approveBooking(bookingId, token);
                    break;

                case "reject":
                    await rejectBooking(bookingId, token);
                    break;

                case "cancel":
                    await cancelBooking(bookingId, token);
                    break;
            }

            return true;
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : "Failed to process booking."
            );

            return false;
        } finally {
            setProcessingBookingId(null);
            setProcessingAction(null);
        }
    }

    return {
        processingBookingId,
        processingAction,
        error,
        executeAction,
    };
}
