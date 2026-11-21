
import { Link } from "react-router";

import type { Booking } from "../../../types/booking";

import BookingStatusBadge from "./BookingStatusBadge";
import BookingActions from "./BookingActions";

import {
    canCancelBooking,
    formatBookingDate,
} from "../utils/bookingUtils";

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
    return (
        <div className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
            <div className="flex flex-wrap items-center justify-between gap-3">
                <div>
                    <p className="text-sm text-slate-500">Hotel</p>
                    <p className="font-medium text-slate-800">
                        {booking.hotelName}
                    </p>
                </div>

                <BookingStatusBadge status={booking.status} />
            </div>

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

            <div className="mt-5 border-t border-slate-100 pt-4">
                <p className="text-sm text-slate-500">Total price</p>
                <p className="text-xl font-bold text-orange-600">
                    ${booking.totalPrice.toFixed(2)}
                </p>
            </div>

            <div className="mt-5 flex flex-wrap items-center justify-between gap-4 border-t border-slate-100 pt-4">
                <Link
                    to={`/bookings/${booking.id}`}
                    className="font-medium text-orange-600 hover:text-orange-700"
                >
                    View details →
                </Link>

                {canCancelBooking(booking.status) && (
                    <BookingActions
                        onCancel={() => onCancel(booking.id)}
                        showCancel
                        disabled={disabled}
                        loading={isCancelling}
                    />
                )}
            </div>
        </div>
    );
}
