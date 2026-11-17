import { useEffect, useState } from "react";
import { Link, useParams } from "react-router";
import type { Booking } from "../../../types/booking";
import { getBookingById } from "../../../services/bookingService";

export default function BookingDetailsPage() {
    const { id } = useParams<{ id: string }>();

    const [booking, setBooking] = useState<Booking | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        const loadBooking = async () => {
            if (!id) {
                setError("Booking ID is missing.");
                setLoading(false);
                return;
            }

            try {
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
        };

        loadBooking();
    }, [id]);

    if (loading) return <p className="p-6">Loading booking...</p>;

    if (error) {
        return <p className="p-6 text-red-600">{error}</p>;
    }

    if (!booking) return <p className="p-6">Booking not found.</p>;

    const statusLabels: Record<string, string> = {
        "0": "Draft",
        "1": "Submitted",
        "2": "Approved",
        "3": "Rejected",
        "4": "Cancelled",
        Draft: "Draft",
        Submitted: "Submitted",
        Approved: "Approved",
        Rejected: "Rejected",
        Cancelled: "Cancelled",
    };

    const status = statusLabels[String(booking.status)] ?? "Unknown";

    return (
        <div className="mx-auto max-w-3xl p-6">
            <Link
                to="/my-bookings"
                className="text-orange-600 hover:underline"
            >
                ← Back to my bookings
            </Link>

            <h1 className="mt-6 text-3xl font-bold">
                Booking Details
            </h1>

            <div className="mt-6 rounded-xl border border-gray-200 p-6">
                <div className="flex items-center justify-between">
                    <h2 className="text-xl font-semibold">
                        {booking.hotelName}
                    </h2>

                    <span className="rounded-full bg-orange-100 px-3 py-1 text-sm text-orange-700">
                        {status}
                    </span>
                </div>

                <div className="mt-6 grid gap-4 sm:grid-cols-2">
                    <div>
                        <p className="text-sm text-gray-500">Room</p>
                        <p className="font-medium">
                            {booking.roomTypeName} - Room {booking.roomNumber}
                        </p>
                    </div>

                    <div>
                        <p className="text-sm text-gray-500">Guests</p>
                        <p className="font-medium">
                            {booking.numberOfGuests}
                        </p>
                    </div>

                    <div>
                        <p className="text-sm text-gray-500">Check-in</p>
                        <p className="font-medium">
                            {new Date(booking.checkIn).toLocaleDateString()}
                        </p>
                    </div>

                    <div>
                        <p className="text-sm text-gray-500">Check-out</p>
                        <p className="font-medium">
                            {new Date(booking.checkOut).toLocaleDateString()}
                        </p>
                    </div>

                    <div>
                        <p className="text-sm text-gray-500">Price per night</p>
                        <p className="font-medium">
                            ${booking.pricePerNight.toFixed(2)}
                        </p>
                    </div>

                    <div>
                        <p className="text-sm text-gray-500">Total price</p>
                        <p className="text-xl font-bold text-orange-600">
                            ${booking.totalPrice.toFixed(2)}
                        </p>
                    </div>
                </div>

                <div className="mt-6 border-t pt-4">
                    <p className="text-sm text-gray-500">Booking ID</p>
                    <p className="break-all text-sm">{booking.id}</p>
                </div>
            </div>
        </div>
    );
}