
import { useParams } from "react-router";

import { useHotelBookings } from "../hooks/useHotelBookings";
import { useBookingActions } from "../hooks/useBookingActions";

import BookingPageHeader from "../components/BookingPageHeader";
import BookingList from "../components/BookingList";
import BookingSection from "../components/BookingSection";
import BookingStatusBadge from "../components/BookingStatusBadge";
import BookingDates from "../components/BookingDates";
import BookingGuestInfo from "../components/BookingGuestInfo";
import BookingPriceSummary from "../components/BookingPriceSummary";
import BookingActions from "../components/BookingActions";
import LoadingState from "../../../components/LoadingState";
import ErrorMessage from "../../../components/ErrorMessage";

export default function HotelBookingsPage() {
    const { hotelId } = useParams<{ hotelId: string }>();
    const {
        bookings,
        setBookings,
        loading,
        error: loadingError,
    } = useHotelBookings(hotelId);

    const {
        processingBookingId,
        processingAction,
        error: actionError,
        executeAction,
    } = useBookingActions();

    async function handleAction(
        bookingId: string,
        action: "approve" | "reject"
    ) {
        if (processingBookingId !== null) return;

        const success = await executeAction(bookingId, action);

        if (success) {
            setBookings((current) =>
                current.map((booking) =>
                    booking.id === bookingId
                        ? {
                            ...booking,
                            status: action === "approve"
                                ? "Approved"
                                : "Rejected",
                        }
                        : booking
                )
            );
        }
    }

    if (loading) {
        return <LoadingState message="Loading hotel bookings..." />;
    }

    return (
        <div className="mx-auto max-w-6xl px-4 py-10">
            <BookingPageHeader
                title="Hotel Bookings"
                description="Review and manage booking requests for this hotel."
            />

            <div className="mb-6">
                <ErrorMessage message={loadingError || actionError} />
            </div>

            {bookings.length === 0 ? (
                <p className="mt-6 text-slate-500">
                    No booking requests found.
                </p>
            ) : (
                <BookingList>
                    {bookings.map((booking) => (
                        <BookingSection
                            key={booking.id}
                            title={booking.hotelName}
                        >
                            <div className="flex flex-wrap items-center justify-between gap-3">
                                <p className="text-sm text-slate-500">
                                    Room {booking.roomNumber} · {booking.roomTypeName}
                                </p>

                                <BookingStatusBadge status={booking.status} />
                            </div>

                            <div className="mt-5">
                                <BookingDates
                                    checkIn={booking.checkIn}
                                    checkOut={booking.checkOut}
                                />
                            </div>

                            <div className="mt-5">
                                <BookingGuestInfo
                                    numberOfGuests={booking.numberOfGuests}
                                />
                            </div>

                            <div className="mt-5 flex flex-wrap items-center justify-between gap-4 border-t border-slate-100 pt-4">
                                <BookingPriceSummary
                                    totalPrice={booking.totalPrice}
                                />

                                {booking.status === "Submitted" && (
                                    <BookingActions
                                        onApprove={() => handleAction(booking.id, "approve")}
                                        onReject={() => handleAction(booking.id, "reject")}
                                        showApproveReject
                                        disabled={processingBookingId !== null}
                                        loading={processingBookingId === booking.id}
                                        loadingAction={
                                            processingBookingId === booking.id
                                                ? processingAction ?? undefined
                                                : undefined
                                        }
                                    />
                                )}
                            </div>
                        </BookingSection>
                    ))}
                </BookingList>
            )}
        </div>
    );
}
