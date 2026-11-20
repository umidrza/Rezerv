
import { Link } from "react-router";

import type { Booking } from "../../../types/booking";

import BookingStatusBadge from "./BookingStatusBadge";

import {
  canCancelBooking,
  formatBookingDate,
} from "../../../utils/bookingUtils";

interface BookingCardProps {
  booking: Booking;
  onCancel: (bookingId: string) => void;
  isCancelling: boolean;
  disabled: boolean;
}

export default function BookingCard({
  booking,
  onCancel,
  isCancelling,
  disabled,
}: BookingCardProps) {
  const status =
    typeof booking.status === "number"
      ? ["Draft", "Submitted", "Approved", "Rejected", "Cancelled"][booking.status] ?? "Unknown"
      : booking.status;

  return (
    <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
      {/* Header */}
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div>
          <p className="text-sm text-slate-500">
            Booking ID
          </p>

          <p className="font-medium text-slate-800">
            {booking.id}
          </p>
        </div>

        <BookingStatusBadge status={booking.status} />
      </div>

      {/* Booking information */}
      <div className="mt-5 grid gap-4 sm:grid-cols-3">
        <div>
          <p className="text-sm text-slate-500">Check-in</p>
          <p className="font-medium">
            {formatBookingDate(booking.checkIn)}
          </p>
        </div>

        <div>
          <p className="text-sm text-slate-500">Check-out</p>
          <p className="font-medium">
            {formatBookingDate(booking.checkOut)}
          </p>
        </div>

        <div>
          <p className="text-sm text-slate-500">Guests</p>
          <p className="font-medium">
            {booking.numberOfGuests}
          </p>
        </div>
      </div>

      {/* Price */}
      <div className="mt-5 border-t border-slate-100 pt-4">
        <p className="text-sm text-slate-500">
          Total price
        </p>

        <p className="text-xl font-bold text-orange-600">
          {booking.totalPrice.toFixed(2)}
        </p>
      </div>

      {/* Actions */}
      <div className="mt-5 flex items-center justify-between gap-4 border-t border-slate-100 pt-4">
        <Link
          to={`/bookings/${booking.id}`}
          className="font-medium text-orange-600 hover:text-orange-700"
        >
          View details →
        </Link>

        {canCancelBooking(status) && (
          <button
            type="button"
            onClick={() => onCancel(booking.id)}
            disabled={disabled}
            className="rounded-lg bg-red-600 px-4 py-2 text-sm font-medium text-white transition hover:bg-red-700 disabled:cursor-not-allowed disabled:opacity-50"
          >
            {isCancelling ? "Cancelling..." : "Cancel Booking"}
          </button>
        )}
      </div>
    </div>
  );
}
