import { useEffect, useState } from "react";
import {
    checkRoomAvailability,
    createBooking,
} from "../../../services/bookingService";
import type { Hotel } from "../../../types/hotel";
import type {
    BookingDraft,
    CreateBookingRequest,
} from "../../../types/booking";

interface UseHotelBookingProps {
    hotel: Hotel | null;
    isAuthenticated: boolean;
    onLoginRequired: (draft: BookingDraft) => void;
    onBookingCreated: (bookingId: string) => void;
}

export function useHotelBooking({
    hotel,
    isAuthenticated,
    onLoginRequired,
    onBookingCreated,
}: UseHotelBookingProps) {
    const [selectedRoomId, setSelectedRoomId] = useState<string | null>(null);
    const [checkIn, setCheckIn] = useState("");
    const [checkOut, setCheckOut] = useState("");
    const [numberOfGuests, setNumberOfGuests] = useState(1);

    const [availabilityLoading, setAvailabilityLoading] = useState(false);
    const [availabilityConfirmed, setAvailabilityConfirmed] = useState(false);
    const [availabilityMessage, setAvailabilityMessage] = useState("");

    const [bookingLoading, setBookingLoading] = useState(false);
    const [error, setError] = useState("");
    const [showConfirmation, setShowConfirmation] = useState(false);

    // Restore booking draft after login
    useEffect(() => {
        if (!hotel || !isAuthenticated) return;

        const savedDraft = sessionStorage.getItem("bookingDraft");
        if (!savedDraft) return;

        try {
            const draft = JSON.parse(savedDraft) as BookingDraft;

            if (draft.hotelId === hotel.id) {
                setSelectedRoomId(draft.roomId);
                setCheckIn(draft.checkIn);
                setCheckOut(draft.checkOut);
                setNumberOfGuests(draft.numberOfGuests);
            }
        } catch {
            console.error("Invalid booking draft.");
        } finally {
            sessionStorage.removeItem("bookingDraft");
        }
    }, [hotel, isAuthenticated]);

    // Reset availability when booking details change
    useEffect(() => {
        setAvailabilityConfirmed(false);
        setAvailabilityMessage("");
    }, [selectedRoomId, checkIn, checkOut, numberOfGuests]);

    async function handleCheckAvailability() {
        if (!selectedRoomId) {
            setError("Please select a room.");
            return;
        }

        if (!checkIn || !checkOut || checkOut <= checkIn) {
            setError("Please select valid check-in and check-out dates.");
            return;
        }

        try {
            setError("");
            setAvailabilityMessage("");
            setAvailabilityLoading(true);

            const result = await checkRoomAvailability({
                roomId: selectedRoomId,
                checkIn,
                checkOut,
            });

            setAvailabilityConfirmed(result.isAvailable);
            setAvailabilityMessage(
                result.isAvailable
                    ? "This room is available for your selected dates."
                    : "This room is not available for your selected dates."
            );
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : "Failed to check availability."
            );
        } finally {
            setAvailabilityLoading(false);
        }
    }

    function handleBookingClick() {
        setError("");

        if (!selectedRoomId) {
            setError("Please select a room.");
            return;
        }

        if (!checkIn || !checkOut || checkOut <= checkIn) {
            setError("Please select valid check-in and check-out dates.");
            return;
        }

        if (numberOfGuests < 1) {
            setError("Please select at least one guest.");
            return;
        }

        if (!availabilityConfirmed) {
            setError("Please check room availability first.");
            return;
        }

        setShowConfirmation(true);
    }

    async function handleConfirmBooking() {
        if (!hotel || !selectedRoomId) return;

        if (!isAuthenticated) {
            onLoginRequired({
                hotelId: hotel.id,
                roomId: selectedRoomId,
                checkIn,
                checkOut,
                numberOfGuests,
            });
            return;
        }

        const request: CreateBookingRequest = {
            hotelId: hotel.id,
            roomId: selectedRoomId,
            checkIn,
            checkOut,
            numberOfGuests,
        };

        try {
            setBookingLoading(true);
            setError("");

            const booking = await createBooking(request);

            setShowConfirmation(false);
            onBookingCreated(booking.id);
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : "Failed to submit booking."
            );
            setShowConfirmation(false);
        } finally {
            setBookingLoading(false);
        }
    }

    return {
        selectedRoomId,
        setSelectedRoomId,
        checkIn,
        setCheckIn,
        checkOut,
        setCheckOut,
        numberOfGuests,
        setNumberOfGuests,
        availabilityLoading,
        availabilityConfirmed,
        availabilityMessage,
        bookingLoading,
        error,
        showConfirmation,
        setShowConfirmation,
        handleCheckAvailability,
        handleBookingClick,
        handleConfirmBooking,
    };
}
