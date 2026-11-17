
import { useEffect, useState } from "react";
import { Link, useLocation, useNavigate, useParams } from "react-router";

import { getHotelById, getRoomTypesByHotel } from "../../../services/hotelService";
import { checkRoomAvailability, createBooking } from "../../../services/bookingService";

import type { Hotel } from "../../../types/hotel";
import type { RoomType } from "../../../types/roomType";
import type { BookingDraft, CreateBookingRequest } from "../../../types/booking";

import RoomCard from "../components/RoomCard";
import DatePicker from "../../../components/DatePicker";
import { useAuth } from "../../auth/AuthContext";

export default function HotelDetailsPage() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const location = useLocation();
    const { token, isAuthenticated } = useAuth();

    const [hotel, setHotel] = useState<Hotel | null>(null);
    const [roomTypes, setRoomTypes] = useState<RoomType[]>([]);

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const [selectedRoomId, setSelectedRoomId] = useState<string | null>(null);
    const [checkIn, setCheckIn] = useState("");
    const [checkOut, setCheckOut] = useState("");
    const [numberOfGuests, setNumberOfGuests] = useState(1);

    const [availabilityLoading, setAvailabilityLoading] = useState(false);
    const [availabilityConfirmed, setAvailabilityConfirmed] = useState(false);
    const [availabilityMessage, setAvailabilityMessage] = useState("");

    const [bookingLoading, setBookingLoading] = useState(false);
    const [bookingMessage, setBookingMessage] = useState("");
    const [showConfirmation, setShowConfirmation] = useState(false);

    // Load hotel and room types
    useEffect(() => {
        if (!id) return;

        const loadHotel = async () => {
            try {
                setLoading(true);
                setError("");

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
                        : "Failed to load hotel details."
                );
            } finally {
                setLoading(false);
            }
        };

        loadHotel();
    }, [id]);

    // Restore a booking draft after returning from login
    useEffect(() => {
        if (!hotel || !isAuthenticated) return;

        const savedDraft = sessionStorage.getItem("bookingDraft");

        if (!savedDraft) return;

        try {
            const draft = JSON.parse(savedDraft) as BookingDraft;

            if (draft.hotelId === hotel.id) {
                setSelectedRoomId(draft.roomId);
                setCheckIn(draft.checkIn);
                setCheckOut(draft.checkOut);
                setNumberOfGuests(draft.numberOfGuests);
            }
        } catch {
            console.error("Invalid booking draft.");
        } finally {
            sessionStorage.removeItem("bookingDraft");
        }
    }, [hotel, isAuthenticated]);

    // Reset availability whenever booking details change
    useEffect(() => {
        setAvailabilityConfirmed(false);
        setAvailabilityMessage("");
        setBookingMessage("");
    }, [selectedRoomId, checkIn, checkOut, numberOfGuests]);

    const handleCheckAvailability = async () => {
        if (!selectedRoomId) {
            setError("Please select a room.");
            return;
        }

        if (!checkIn || !checkOut) {
            setError("Please select check-in and check-out dates.");
            return;
        }

        if (checkOut <= checkIn) {
            setError("Check-out must be after check-in.");
            return;
        }

        try {
            setError("");
            setAvailabilityMessage("");
            setAvailabilityLoading(true);

            const result = await checkRoomAvailability({
                roomId: selectedRoomId,
                checkIn,
                checkOut,
            });

            setAvailabilityConfirmed(result.isAvailable);

            setAvailabilityMessage(
                result.isAvailable
                    ? "This room is available for your selected dates."
                    : "This room is not available for your selected dates."
            );
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : "Failed to check availability."
            );
        } finally {
            setAvailabilityLoading(false);
        }
    };

    const handleBookingClick = () => {
        setError("");
        setBookingMessage("");

        if (!selectedRoomId) {
            setError("Please select a room.");
            return;
        }

        if (!checkIn || !checkOut || checkOut <= checkIn) {
            setError("Please select valid check-in and check-out dates.");
            return;
        }

        if (numberOfGuests < 1) {
            setError("Please select at least one guest.");
            return;
        }

        if (!availabilityConfirmed) {
            setError("Please check room availability first.");
            return;
        }

        setShowConfirmation(true);
    };

    const handleConfirmBooking = async () => {
        if (!hotel || !selectedRoomId) return;

        if (!isAuthenticated || !token) {
            const draft: BookingDraft = {
                hotelId: hotel.id,
                roomId: selectedRoomId,
                checkIn,
                checkOut,
                numberOfGuests,
            };

            sessionStorage.setItem("bookingDraft", JSON.stringify(draft));

            navigate("/login", {
                state: { from: location },
            });

            return;
        }

        const bookingRequest: CreateBookingRequest = {
            hotelId: hotel.id,
            roomId: selectedRoomId,
            checkIn,
            checkOut,
            numberOfGuests,
        };

        try {
            setBookingLoading(true);
            setError("");

            const booking = await createBooking(bookingRequest, token);

            setShowConfirmation(false);

            navigate("/my-bookings", {
                state: {
                    message: `Booking request submitted successfully. Booking ID: ${booking.id}`,
                },
            });
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : "Failed to submit booking."
            );
            setShowConfirmation(false);
        } finally {
            setBookingLoading(false);
        }
    };

    if (loading) {
        return (
            <div className="mx-auto max-w-7xl px-4 py-12 text-center">
                Loading hotel details...
            </div>
        );
    }

    if (error && !hotel) {
        return (
            <div className="mx-auto max-w-7xl px-4 py-12">
                <p className="text-red-600">{error}</p>
                <Link to="/" className="mt-4 inline-block text-orange-600">
                    Back to hotels
                </Link>
            </div>
        );
    }

    if (!hotel) {
        return (
            <div className="mx-auto max-w-7xl px-4 py-12">
                <p>Hotel not found.</p>
                <Link to="/" className="mt-4 inline-block text-orange-600">
                    Back to hotels
                </Link>
            </div>
        );
    }

    return (
        <div className="mx-auto max-w-7xl px-4 py-8">
            <Link
                to="/"
                className="mb-6 inline-block text-sm font-medium text-orange-600 hover:text-orange-700"
            >
                ← Back to hotels
            </Link>

            {/* Hotel information */}
            <div className="overflow-hidden rounded-xl bg-white shadow-sm">
                {hotel.primaryImageUrl && (
                    <img
                        src={hotel.primaryImageUrl}
                        alt={hotel.name}
                        className="h-72 w-full object-cover"
                    />
                )}

                <div className="p-6">
                    <div className="flex flex-wrap items-center justify-between gap-3">
                        <h1 className="text-3xl font-bold text-slate-900">
                            {hotel.name}
                        </h1>

                        <span className="rounded-full bg-orange-100 px-3 py-1 text-sm font-semibold text-orange-700">
                            {hotel.starRating} Stars
                        </span>
                    </div>

                    <p className="mt-2 text-slate-500">
                        {hotel.address}, {hotel.city}, {hotel.country}
                    </p>

                    <p className="mt-5 leading-7 text-slate-700">
                        {hotel.description}
                    </p>
                </div>
            </div>

            {/* Room types and booking */}
            <div className="mt-8 grid grid-cols-1 gap-8 lg:grid-cols-3">
                <div className="space-y-6 lg:col-span-2">
                    <h2 className="text-2xl font-bold text-slate-900">
                        Available room types
                    </h2>

                    {roomTypes.length === 0 ? (
                        <div className="rounded-xl bg-white p-6 text-slate-500">
                            No room types found for this hotel.
                        </div>
                    ) : (
                        roomTypes.map((roomType) => (
                            <div
                                key={roomType.id}
                                className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm"
                            >
                                <div className="flex flex-wrap justify-between gap-3">
                                    <div>
                                        <h3 className="text-xl font-semibold text-slate-900">
                                            {roomType.name}
                                        </h3>

                                        <p className="mt-2 text-sm text-slate-500">
                                            {roomType.description}
                                        </p>

                                        <p className="mt-2 text-sm text-slate-600">
                                            Capacity: {roomType.capacity} guests
                                        </p>
                                    </div>

                                    <div className="text-right">
                                        <p className="text-xl font-bold text-orange-600">
                                            {roomType.pricePerNight.toFixed(2)}
                                        </p>
                                        <p className="text-sm text-slate-500">
                                            per night
                                        </p>
                                    </div>
                                </div>

                                <RoomCard
                                    roomTypeId={roomType.id}
                                    selectedRoomId={selectedRoomId}
                                    onRoomSelect={setSelectedRoomId}
                                />
                            </div>
                        ))
                    )}
                </div>

                {/* Booking panel */}
                <aside className="h-fit rounded-xl bg-white p-6 shadow-sm">
                    <h2 className="text-xl font-bold text-slate-900">
                        Your booking
                    </h2>

                    <div className="mt-5 space-y-4">
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

                        <div>
                            <label
                                htmlFor="numberOfGuests"
                                className="mb-2 block text-sm font-medium text-slate-700"
                            >
                                Guests
                            </label>

                            <input
                                id="numberOfGuests"
                                type="number"
                                min="1"
                                value={numberOfGuests}
                                onChange={(event) =>
                                    setNumberOfGuests(
                                        Math.max(1, Number(event.target.value))
                                    )
                                }
                                className="w-full rounded-lg border border-slate-300 px-3 py-2 outline-none focus:border-orange-500 focus:ring-2 focus:ring-orange-100"
                            />
                        </div>

                        <button
                            type="button"
                            onClick={handleCheckAvailability}
                            disabled={availabilityLoading}
                            className="w-full rounded-lg border border-orange-500 px-4 py-3 font-semibold text-orange-600 transition hover:bg-orange-50 disabled:cursor-not-allowed disabled:opacity-50"
                        >
                            {availabilityLoading
                                ? "Checking..."
                                : "Check availability"}
                        </button>

                        {availabilityMessage && (
                            <p
                                className={`text-sm ${availabilityConfirmed
                                        ? "text-green-600"
                                        : "text-red-600"
                                    }`}
                            >
                                {availabilityMessage}
                            </p>
                        )}

                        {error && (
                            <p className="text-sm text-red-600">{error}</p>
                        )}

                        <button
                            type="button"
                            onClick={handleBookingClick}
                            disabled={bookingLoading}
                            className="w-full rounded-lg bg-orange-500 px-4 py-3 font-semibold text-white transition hover:bg-orange-600 disabled:cursor-not-allowed disabled:opacity-50"
                        >
                            Request booking
                        </button>

                        <p className="text-xs leading-5 text-slate-500">
                            Your booking request will be sent to the hotel.
                            Payment is not required at this stage.
                        </p>
                    </div>
                </aside>
            </div>

            {/* Booking confirmation modal */}
            {showConfirmation && (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
                    <div className="w-full max-w-md rounded-xl bg-white p-6 shadow-xl">
                        <h2 className="text-xl font-bold text-slate-900">
                            Confirm booking request
                        </h2>

                        <p className="mt-2 text-sm text-slate-500">
                            Please review your booking details before submitting.
                        </p>

                        <div className="mt-5 space-y-3 rounded-lg bg-slate-50 p-4 text-sm">
                            <div className="flex justify-between gap-3">
                                <span className="text-slate-500">Hotel</span>
                                <span className="font-medium text-slate-900">
                                    {hotel.name}
                                </span>
                            </div>

                            <div className="flex justify-between gap-3">
                                <span className="text-slate-500">Room</span>
                                <span className="font-medium text-slate-900">
                                    {selectedRoomId}
                                </span>
                            </div>

                            <div className="flex justify-between gap-3">
                                <span className="text-slate-500">Check-in</span>
                                <span className="font-medium text-slate-900">
                                    {checkIn}
                                </span>
                            </div>

                            <div className="flex justify-between gap-3">
                                <span className="text-slate-500">Check-out</span>
                                <span className="font-medium text-slate-900">
                                    {checkOut}
                                </span>
                            </div>

                            <div className="flex justify-between gap-3">
                                <span className="text-slate-500">Guests</span>
                                <span className="font-medium text-slate-900">
                                    {numberOfGuests}
                                </span>
                            </div>
                        </div>

                        <div className="mt-6 flex gap-3">
                            <button
                                type="button"
                                onClick={() => setShowConfirmation(false)}
                                disabled={bookingLoading}
                                className="flex-1 rounded-lg border border-slate-300 px-4 py-3 font-medium text-slate-700 hover:bg-slate-50"
                            >
                                Go back
                            </button>

                            <button
                                type="button"
                                onClick={handleConfirmBooking}
                                disabled={bookingLoading}
                                className="flex-1 rounded-lg bg-orange-500 px-4 py-3 font-semibold text-white hover:bg-orange-600 disabled:opacity-50"
                            >
                                {bookingLoading ? "Submitting..." : "Confirm"}
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}
