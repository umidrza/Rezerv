import { useEffect, useState } from "react";
import { Link, useParams } from "react-router";

import { getHotelById, getRoomTypesByHotel } from "../../../services/hotelService";

import type { Hotel } from "../../../types/hotel";
import type { RoomType } from "../../../types/roomType";

import RoomCard from "../components/RoomCard";
import DatePicker from "../../../components/DatePicker";
import { checkRoomAvailability } from "../../../services/bookingService";
import { createBooking } from "../../../services/bookingService";

export default function HotelDetailsPage() {
    const { id } = useParams();

    const [hotel, setHotel] = useState<Hotel | null>(null);
    const [roomTypes, setRoomTypes] = useState<RoomType[]>([]);
    const [selectedRoomIds, setSelectedRoomIds] = useState<string[]>([]);

    const [checkIn, setCheckIn] = useState("");
    const [checkOut, setCheckOut] = useState("");

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const [availabilityLoading, setAvailabilityLoading] = useState(false);
    const [availabilityMessage, setAvailabilityMessage] = useState("");
    const [availabilityError, setAvailabilityError] = useState("");

    const [guests, setGuests] = useState("1");
    const [availabilityConfirmed, setAvailabilityConfirmed] = useState(false);
    const [bookingLoading, setBookingLoading] = useState(false);
    const [bookingMessage, setBookingMessage] = useState("");

    useEffect(() => {
        async function loadHotel() {
            if (!id) {
                setError("Hotel ID is missing.");
                setLoading(false);
                return;
            }

            try {
                const [hotelResult, roomTypesResult] = await Promise.all([
                    getHotelById(id),
                    getRoomTypesByHotel(id),
                ]);

                setHotel(hotelResult);
                setRoomTypes(roomTypesResult);
            } catch (err) {
                setError(
                    err instanceof Error
                        ? err.message
                        : "Something went wrong."
                );
            } finally {
                setLoading(false);
            }
        }

        loadHotel();
    }, [id]);

    function handleRoomSelection(roomId: string, selected: boolean) {
        setSelectedRoomIds((current) =>
            selected
                ? [...current, roomId]
                : current.filter((id) => id !== roomId)
        );

        setAvailabilityConfirmed(false);
        setBookingMessage("");
    }

    async function handleCheckAvailability() {
        if (!checkIn || !checkOut) {
            setAvailabilityError("Please select check-in and check-out dates.");
            return;
        }

        if (checkOut <= checkIn) {
            setAvailabilityError("Check-out must be after check-in.");
            return;
        }

        if (selectedRoomIds.length === 0) {
            setAvailabilityError("Please select at least one room.");
            return;
        }

        setAvailabilityLoading(true);
        setAvailabilityError("");
        setAvailabilityMessage("");
        setAvailabilityConfirmed(false);
        setBookingMessage("");

        try {
            const result = await checkRoomAvailability(
                {
                    roomIds: selectedRoomIds,
                    checkIn,
                    checkOut,
                }
            );

            if (result.isAvailable) {
                setAvailabilityConfirmed(true);
                setAvailabilityMessage("Great! All selected rooms are available.");
            } else {
                setAvailabilityConfirmed(false);
                setAvailabilityError("Some selected rooms are not available for these dates.");
            }
        } catch (err) {
            setAvailabilityError(
                err instanceof Error
                    ? err.message
                    : "Failed to check availability."
            );
        } finally {
            setAvailabilityLoading(false);
        }
    }

    async function handleSubmitBooking() {
        if (!id || !availabilityConfirmed) {
            return;
        }

        if (!guests || Number(guests) < 1) {
            setAvailabilityError("Please enter a valid number of guests.");
            return;
        }

        const token = localStorage.getItem("token");

        if (!token) {
            setAvailabilityError("Please log in to submit a booking.");
            return;
        }

        setBookingLoading(true);
        setBookingMessage("");
        setAvailabilityError("");

        try {
            const booking = await createBooking(
                {
                    hotelId: id,
                    checkIn,
                    checkOut,
                    numberOfGuests: Number(guests),
                    roomIds: selectedRoomIds,
                },
                token
            );

            setBookingMessage(
                `Booking submitted successfully! Booking ID: ${booking.id}. Status: ${booking.status}.`
            );

            setAvailabilityConfirmed(false);
        } catch (err) {
            setAvailabilityError(
                err instanceof Error
                    ? err.message
                    : "Failed to submit booking."
            );
        } finally {
            setBookingLoading(false);
        }
    }

    if (loading) {
        return (
            <p className="p-10 text-center">
                Loading hotel...
            </p>
        );
    }

    if (error || !hotel) {
        return (
            <div className="p-10 text-center">
                <p className="text-red-600">
                    {error || "Hotel not found."}
                </p>

                <Link
                    to="/"
                    className="mt-4 inline-block text-orange-600"
                >
                    Back to home
                </Link>
            </div>
        );
    }

    return (
        <div className="mx-auto max-w-6xl px-6 py-10">
            <Link
                to="/"
                className="text-orange-600 hover:underline"
            >
                ← Back to hotels
            </Link>

            {hotel.primaryImageUrl ? (
                <img
                    src={hotel.primaryImageUrl}
                    alt={hotel.name}
                    className="mt-6 h-96 w-full rounded-xl object-cover"
                />
            ) : (
                <div className="mt-6 flex h-96 items-center justify-center rounded-xl bg-slate-200 text-slate-500">
                    No image available
                </div>
            )}

            {/* Hotel Information */}
            <div className="mt-8">
                <h1 className="text-3xl font-bold text-slate-900">
                    {hotel.name}
                </h1>

                <p className="mt-2 text-slate-500">
                    {hotel.address}, {hotel.city}, {hotel.country}
                </p>

                <p className="mt-3 text-xl text-orange-500">
                    {"★".repeat(hotel.starRating)}
                </p>

                <p className="mt-6 leading-7 text-slate-700">
                    {hotel.description}
                </p>
            </div>

            {/* Stay Dates */}
            <section className="mt-10 rounded-xl bg-white p-6 shadow">
                <h2 className="mb-5 text-2xl font-bold text-slate-900">
                    Your stay
                </h2>

                <div className="grid gap-4 md:grid-cols-2">
                    <div>
                        <label className="mb-2 block text-sm font-medium">
                            Check-in
                        </label>

                        <DatePicker
                            value={checkIn}
                            onChange={(value) => {
                                setCheckIn(value);

                                if (checkOut && value && checkOut <= value) {
                                    setCheckOut("");
                                }
                            }}
                            placeholder="Select check-in date"
                        />
                    </div>

                    <div>
                        <label className="mb-2 block text-sm font-medium">
                            Check-out
                        </label>

                        <DatePicker
                            value={checkOut}
                            onChange={setCheckOut}
                            placeholder="Select check-out date"
                            minDate={
                                checkIn
                                    ? new Date(`${checkIn}T00:00:00`)
                                    : undefined
                            }
                        />
                    </div>
                </div>
            </section>

            {/* Room Types */}
            <section className="mt-12">
                <h2 className="mb-6 text-2xl font-bold text-slate-900">
                    Available room types
                </h2>

                {roomTypes.length === 0 ? (
                    <p className="text-slate-500">
                        No room types available for this hotel.
                    </p>
                ) : (
                    <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
                        {roomTypes.map((roomType) => (
                            <RoomCard
                                key={roomType.id}
                                roomType={roomType}
                                selectedRoomIds={selectedRoomIds}
                                onRoomSelectionChange={handleRoomSelection}
                            />
                        ))}
                    </div>
                )}
            </section>

            {/* Selection Summary */}
            <section className="mt-8 rounded-xl bg-orange-50 p-6">
                <h2 className="text-xl font-bold text-slate-900">
                    Your selection
                </h2>

                <div className="mt-3 space-y-2 text-slate-600">
                    <p>Selected rooms: {selectedRoomIds.length}</p>
                    <p>Check-in: {checkIn || "Not selected"}</p>
                    <p>Check-out: {checkOut || "Not selected"}</p>
                </div>

                <div className="mt-4">
                    <label className="mb-2 block text-sm font-medium text-slate-700">
                        Number of guests
                    </label>

                    <input
                        type="number"
                        min="1"
                        value={guests}
                        onChange={(e) => {
                            setGuests(e.target.value);
                            setAvailabilityConfirmed(false);
                        }}
                        className="w-full rounded-lg border border-slate-300 p-3"
                    />
                </div>

                <button
                    type="button"
                    onClick={handleCheckAvailability}
                    disabled={availabilityLoading}
                    className="mt-6 rounded-lg bg-orange-500 px-6 py-3 font-semibold text-white hover:bg-orange-600 disabled:opacity-50"
                >
                    {availabilityLoading ? "Checking..." : "Check availability"}
                </button>

                {availabilityConfirmed && (
                    <button
                        type="button"
                        onClick={handleSubmitBooking}
                        disabled={bookingLoading}
                        className="ml-3 rounded-lg bg-green-600 px-6 py-3 font-semibold text-white hover:bg-green-700 disabled:opacity-50"
                    >
                        {bookingLoading ? "Submitting..." : "Submit booking"}
                    </button>
                )}

                {bookingMessage && (
                    <p className="mt-4 text-green-700">
                        {bookingMessage}
                    </p>
                )}

                {availabilityMessage && (
                    <p className="mt-4 text-green-700">
                        {availabilityMessage}
                    </p>
                )}

                {availabilityError && (
                    <p className="mt-4 text-red-600">
                        {availabilityError}
                    </p>
                )}
            </section>
        </div>
    );
}
