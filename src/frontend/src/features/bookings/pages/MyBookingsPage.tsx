
import { useAuth } from "../../auth/AuthContext";

import { useMyBookings } from "../hooks/useMyBookings";
import { useBookingActions } from "../hooks/useBookingActions";

import BookingPageHeader from "../components/BookingPageHeader";
import BookingList from "../components/BookingList";
import BookingCard from "../components/BookingCard";
import BookingEmptyState from "../components/BookingEmptyState";
import LoadingState from "../../../components/LoadingState";
import ErrorMessage from "../../../components/ErrorMessage";

export default function MyBookingsPage() {
  const { token } = useAuth();

  const {
    bookings,
    setBookings,
    loading,
    error: loadingError,
  } = useMyBookings(token);

  const {
    processingBookingId,
    error: actionError,
    executeAction,
  } = useBookingActions(token);

  async function handleCancelBooking(bookingId: string) {
    if (processingBookingId !== null) return;

    const confirmed = window.confirm(
      "Are you sure you want to cancel this booking?"
    );

    if (!confirmed) return;

    const success = await executeAction(bookingId, "cancel");

    if (success) {
      setBookings((current) =>
        current.map((booking) =>
          booking.id === bookingId
            ? { ...booking, status: "Cancelled" }
            : booking
        )
      );
    }
  }

  if (loading) {
    return <LoadingState message="Loading your bookings..." />;
  }

  return (
    <div className="mx-auto max-w-5xl px-4 py-10">
      <BookingPageHeader
        title="My Bookings"
        description="View and manage your hotel reservations."
      />

      <div className="mb-6">
        <ErrorMessage message={loadingError || actionError} />
      </div>

      {bookings.length === 0 ? (
        <BookingEmptyState />
      ) : (
        <BookingList>
          {bookings.map((booking) => (
            <BookingCard
              key={booking.id}
              booking={booking}
              onCancel={handleCancelBooking}
              isCancelling={processingBookingId === booking.id}
              disabled={processingBookingId !== null}
            />
          ))}
        </BookingList>
      )}
    </div>
  );
}
