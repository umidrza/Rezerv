
import { useEffect, useState } from "react";

import {
  cancelBooking,
  getMyBookings,
} from "../../../services/bookingService";

import type { Booking } from "../../../types/booking";
import { useAuth } from "../../auth/AuthContext";

import BookingCard from "../components/BookingCard";
import BookingEmptyState from "../components/BookingEmptyState";

export default function MyBookingsPage() {
  const { token } = useAuth();

  const [bookings, setBookings] = useState<Booking[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [cancellingBookingId, setCancellingBookingId] =
    useState<string | null>(null);

  useEffect(() => {
    async function loadBookings() {
      if (!token) {
        setError("Please log in to view your bookings.");
        setLoading(false);
        return;
      }

      try {
        setLoading(true);
        setError("");

        const result = await getMyBookings(token);
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
    }

    loadBookings();
  }, [token]);

  async function handleCancelBooking(bookingId: string) {
    if (!token || cancellingBookingId !== null) return;

    const confirmed = window.confirm(
      "Are you sure you want to cancel this booking?"
    );

    if (!confirmed) return;

    setCancellingBookingId(bookingId);

    try {
      await cancelBooking(bookingId, token);

      setBookings((current) =>
        current.map((booking) =>
          booking.id === bookingId
            ? { ...booking, status: "Cancelled" }
            : booking
        )
      );
    } catch (err) {
      alert(
        err instanceof Error
          ? err.message
          : "Failed to cancel booking."
      );
    } finally {
      setCancellingBookingId(null);
    }
  }

  if (loading) {
    return (
      <div className="py-12 text-center text-slate-500">
        Loading your bookings...
      </div>
    );
  }

  if (error) {
    return (
      <div className="py-12 text-center text-red-600">
        {error}
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-5xl px-4 py-10">
      <h1 className="mb-8 text-3xl font-bold text-slate-900">
        My Bookings
      </h1>

      {bookings.length === 0 ? (
        <BookingEmptyState />
      ) : (
        <div className="space-y-4">
          {bookings.map((booking) => (
            <BookingCard
              key={booking.id}
              booking={booking}
              onCancel={handleCancelBooking}
              isCancelling={cancellingBookingId === booking.id}
              disabled={cancellingBookingId !== null}
            />
          ))}
        </div>
      )}
    </div>
  );
}
