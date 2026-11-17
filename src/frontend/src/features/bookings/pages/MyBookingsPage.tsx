import { useEffect, useState } from "react";
import { getMyBookings } from "../../../services/bookingService";
import type { Booking } from "../../../types/booking";
import { Link } from "react-router";

const bookingStatuses = [
  "Draft",
  "Submitted",
  "Approved",
  "Rejected",
  "Cancelled",
];

function getStatusLabel(status: string | number): string {
  if (typeof status === "number") {
    return bookingStatuses[status] ?? "Unknown";
  }

  return status.charAt(0).toUpperCase() + status.slice(1);
}

function getStatusStyle(status: string) {
  switch (status.toLowerCase()) {
    case "approved":
      return "bg-green-100 text-green-700";
    case "submitted":
      return "bg-orange-100 text-orange-700";
    case "rejected":
    case "cancelled":
      return "bg-red-100 text-red-700";
    default:
      return "bg-slate-100 text-slate-700";
  }
}

function formatDate(date: string) {
  return new Date(date).toLocaleDateString();
}

export default function MyBookingsPage() {
  const [bookings, setBookings] = useState<Booking[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    async function loadBookings() {
      const token = localStorage.getItem("token");

      if (!token) {
        setError("Please log in to view your bookings.");
        setLoading(false);
        return;
      }

      try {
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
  }, []);

  if (loading) {
    return <div className="py-12 text-center">Loading your bookings...</div>;
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
        <div className="rounded-xl border border-slate-200 bg-white p-8 text-center">
          <h2 className="text-lg font-semibold text-slate-800">
            No bookings yet
          </h2>
          <p className="mt-2 text-slate-500">
            Your bookings will appear here after you submit a booking request.
          </p>
        </div>
      ) : (
        <div className="space-y-4">
          {bookings.map((booking) => {
            const status = getStatusLabel(booking.status);

            return (
              <div
                key={booking.id}
                className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm"
              >
                <div className="flex flex-wrap items-center justify-between gap-3">
                  <div>
                    <p className="text-sm text-slate-500">Booking ID</p>
                    <p className="font-medium text-slate-800">
                      {booking.id}
                    </p>
                  </div>

                  <span
                    className={`rounded-full px-3 py-1 text-sm font-medium ${getStatusStyle(status)}`}
                  >
                    {status}
                  </span>
                </div>

                <div className="mt-5 grid gap-4 sm:grid-cols-3">
                  <div>
                    <p className="text-sm text-slate-500">Check-in</p>
                    <p className="font-medium">
                      {formatDate(booking.checkIn)}
                    </p>
                  </div>

                  <div>
                    <p className="text-sm text-slate-500">Check-out</p>
                    <p className="font-medium">
                      {formatDate(booking.checkOut)}
                    </p>
                  </div>

                  <div>
                    <p className="text-sm text-slate-500">Guests</p>
                    <p className="font-medium">{booking.numberOfGuests}</p>
                  </div>
                </div>

                <div className="mt-5 border-t border-slate-100 pt-4">
                  <p className="text-sm text-slate-500">Total price</p>
                  <p className="text-xl font-bold text-orange-600">
                    {booking.totalPrice.toFixed(2)}
                  </p>
                </div>

                <Link
                  to={`/bookings/${booking.id}`}
                  className="mt-4 inline-block font-medium text-orange-600 hover:text-orange-700"
                >
                  View details →
                </Link>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}