import { useEffect, useState } from "react";
import { Link, useParams } from "react-router";

import type { Booking } from "../../../types/booking";
import { getBookingById } from "../../../services/bookingService";

import BookingPageHeader from "../components/BookingPageHeader";
import BookingSection from "../components/BookingSection";
import BookingDates from "../components/BookingDates";
import BookingGuestInfo from "../components/BookingGuestInfo";
import BookingPriceSummary from "../components/BookingPriceSummary";
import BookingInfo from "../components/BookingInfo";
import DetailItem from "../components/DetailItem";
import LoadingState from "../../../components/LoadingState";
import ErrorMessage from "../../../components/ErrorMessage";
import BookingStatusBadge from "../components/BookingStatusBadge";

export default function BookingDetailsPage() {
    const { id } = useParams<{ id: string }>();

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

    if (loading) {
        return <LoadingState message="Loading booking details..." />;
    }

    if (error) {
        return (
            <div className="mx-auto max-w-3xl px-4 py-12">
                <ErrorMessage message={error} />
            </div>
        );
    }

    if (!booking) {
        return (
            <div className="py-12 text-center text-slate-600">
                Booking not found.
            </div>
        );
    }

    return (
        <div className="mx-auto max-w-4xl px-4 py-10">
            <Link
                to="/my-bookings"
                className="font-medium text-orange-600 hover:text-orange-700"
            >
                ← Back to my bookings
            </Link>

            <div className="mt-6 flex flex-wrap items-center justify-between gap-4">
                <BookingPageHeader
                    title="Booking Details"
                    description="Review your booking information."
                />

                <BookingStatusBadge status={booking.status} />
            </div>

            <div className="mt-8 space-y-6">
                <BookingSection title={booking.hotelName}>
                    <div className="grid gap-6 sm:grid-cols-2">
                        <DetailItem
                            label="Room type"
                            value={booking.roomTypeName}
                        />

                        <DetailItem
                            label="Room number"
                            value={booking.roomNumber}
                        />

                        <BookingGuestInfo
                            numberOfGuests={booking.numberOfGuests}
                        />
                    </div>
                </BookingSection>

                <BookingSection title="Stay Information">
                    <BookingDates
                        checkIn={booking.checkIn}
                        checkOut={booking.checkOut}
                    />
                </BookingSection>

                <BookingSection title="Price Summary">
                    <BookingPriceSummary
                        pricePerNight={booking.pricePerNight}
                        totalPrice={booking.totalPrice}
                    />
                </BookingSection>

                <BookingSection title="Booking Information">
                    <BookingInfo
                        bookingId={booking.id}
                        createdAt={booking.createdAt}
                    />
                </BookingSection>
            </div>
        </div>
    );
}
