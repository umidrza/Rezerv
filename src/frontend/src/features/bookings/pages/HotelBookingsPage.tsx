import { useEffect, useState } from "react";
import { useParams } from "react-router";
import { getHotelBookings } from "../../../services/bookingService";
import { approveBooking, rejectBooking } from "../../../services/bookingService";
import type { Booking } from "../../../types/booking";
import { useAuth } from "../../auth/AuthContext";

export default function HotelBookingsPage() {
    const { hotelId } = useParams<{ hotelId: string }>();
    const { token } = useAuth();

    const [bookings, setBookings] = useState<Booking[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const loadBookings = async () => {
        if (!hotelId || !token) return;

        try {
            setLoading(true);
            setError("");

            const result = await getHotelBookings(hotelId, token);
            setBookings(result);
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : "Failed to load bookings."
            );
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadBookings();
    }, [hotelId, token]);

    const handleAction = async (
        bookingId: string,
        action: "approve" | "reject"
    ) => {
        if (!token) return;

        try {
            if (action === "approve") {
                await approveBooking(bookingId, token);
            } else {
                await rejectBooking(bookingId, token);
            }

            await loadBookings();
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : "Failed to update booking."
            );
        }
    };

    const getStatus = (status: string | number) => {
        const statuses = [
            "Draft",
            "Submitted",
            "Approved",
            "Rejected",
            "Cancelled",
        ];

        return typeof status === "number"
            ? statuses[status] ?? "Unknown"
            : status;
    };

    if (loading) return <p className="p-6">Loading bookings...</p>;

    return (
        <div className="mx-auto max-w-6xl p-6">
            <h1 className="text-3xl font-bold">Hotel Bookings</h1>

            {error && (
                <p className="mt-4 rounded-lg bg-red-100 p-3 text-red-700">
                    {error}
                </p>
            )}

            {bookings.length === 0 ? (
                <p className="mt-6 text-gray-500">
                    No booking requests found.
                </p>
            ) : (
                <div className="mt-6 space-y-4">
                    {bookings.map((booking) => {
                        const status = getStatus(booking.status);

                        return (
                            <div
                                key={booking.id}
                                className="rounded-xl border border-gray-200 p-5"
                            >
                                <div className="flex flex-wrap items-center justify-between gap-3">
                                    <div>
                                        <h2 className="text-lg font-semibold">
                                            {booking.hotelName}
                                        </h2>
                                        <p className="text-sm text-gray-500">
                                            Room {booking.roomNumber} ·{" "}
                                            {booking.roomTypeName}
                                        </p>
                                    </div>

                                    <span className="rounded-full bg-orange-100 px-3 py-1 text-sm text-orange-700">
                                        {status}
                                    </span>
                                </div>

                                <div className="mt-4 grid gap-3 sm:grid-cols-3">
                                    <div>
                                        <p className="text-sm text-gray-500">
                                            Check-in
                                        </p>
                                        <p>
                                            {new Date(
                                                booking.checkIn
                                            ).toLocaleDateString()}
                                        </p>
                                    </div>

                                    <div>
                                        <p className="text-sm text-gray-500">
                                            Check-out
                                        </p>
                                        <p>
                                            {new Date(
                                                booking.checkOut
                                            ).toLocaleDateString()}
                                        </p>
                                    </div>

                                    <div>
                                        <p className="text-sm text-gray-500">
                                            Guests
                                        </p>
                                        <p>{booking.numberOfGuests}</p>
                                    </div>
                                </div>

                                <div className="mt-4 flex flex-wrap items-center justify-between gap-3">
                                    <p className="text-lg font-bold text-orange-600">
                                        ${booking.totalPrice.toFixed(2)}
                                    </p>

                                    {status === "Submitted" && (
                                        <div className="flex gap-2">
                                            <button
                                                onClick={() =>
                                                    handleAction(
                                                        booking.id,
                                                        "approve"
                                                    )
                                                }
                                                className="rounded-lg bg-green-600 px-4 py-2 text-white hover:bg-green-700"
                                            >
                                                Approve
                                            </button>

                                            <button
                                                onClick={() =>
                                                    handleAction(
                                                        booking.id,
                                                        "reject"
                                                    )
                                                }
                                                className="rounded-lg bg-red-600 px-4 py-2 text-white hover:bg-red-700"
                                            >
                                                Reject
                                            </button>
                                        </div>
                                    )}
                                </div>
                            </div>
                        );
                    })}
                </div>
            )}
        </div>
    );
}