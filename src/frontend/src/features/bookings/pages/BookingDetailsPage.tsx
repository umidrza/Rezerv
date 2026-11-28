
import { Link, useParams } from "react-router";

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

import { useBookingDetails } from "../hooks/useBookingDetails";

export default function BookingDetailsPage() {
    const { id } = useParams<{ id: string }>();
    const { booking, loading, error } = useBookingDetails(id);

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
