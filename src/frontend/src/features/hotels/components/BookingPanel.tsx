
import DatePicker from "../../../components/DatePicker";

interface BookingPanelProps {
    checkIn: string;
    setCheckIn: (value: string) => void;
    checkOut: string;
    setCheckOut: (value: string) => void;
    numberOfGuests: number;
    setNumberOfGuests: (value: number) => void;
    availabilityLoading: boolean;
    availabilityConfirmed: boolean;
    availabilityMessage: string;
    bookingLoading: boolean;
    error: string;
    onCheckAvailability: () => void;
    onRequestBooking: () => void;
}

export default function BookingPanel({
    checkIn,
    setCheckIn,
    checkOut,
    setCheckOut,
    numberOfGuests,
    setNumberOfGuests,
    availabilityLoading,
    availabilityConfirmed,
    availabilityMessage,
    bookingLoading,
    error,
    onCheckAvailability,
    onRequestBooking,
}: BookingPanelProps) {
    function handleCheckInChange(value: string) {
        setCheckIn(value);

        if (checkOut && checkOut <= value) {
            setCheckOut("");
        }
    }

    return (
        <aside className="h-fit rounded-xl bg-white p-6 shadow-sm">
            <h2 className="text-xl font-bold text-slate-900">
                Request a booking
            </h2>

            <div className="mt-5 space-y-4">
                <div>
                    <label className="mb-2 block text-sm font-medium text-slate-700">
                        Check-in
                    </label>

                    <DatePicker
                        value={checkIn}
                        onChange={handleCheckInChange}
                        placeholder="Select check-in date"
                    />
                </div>

                <div>
                    <label className="mb-2 block text-sm font-medium text-slate-700">
                        Check-out
                    </label>

                    <DatePicker
                        value={checkOut}
                        onChange={setCheckOut}
                        placeholder="Select check-in date"
                        minDate={
                            checkIn
                                ? new Date(`${checkIn}T00:00:00`)
                                : undefined
                        }
                    />
                </div>

                <div>
                    <label className="mb-2 block text-sm font-medium text-slate-700">
                        Guests
                    </label>

                    <input
                        type="number"
                        min="1"
                        value={numberOfGuests}
                        onChange={(event) =>
                            setNumberOfGuests(
                                Math.max(1, Number(event.target.value))
                            )
                        }
                        className="w-full rounded-lg border border-slate-300 px-3 py-2"
                    />
                </div>

                <button
                    type="button"
                    onClick={onCheckAvailability}
                    disabled={availabilityLoading}
                    className="w-full rounded-lg border border-orange-500 px-4 py-3 font-semibold text-orange-600 transition hover:bg-orange-50 disabled:opacity-50"
                >
                    {availabilityLoading
                        ? "Checking..."
                        : "Check availability"}
                </button>

                {availabilityMessage && (
                    <p
                        className={`text-sm ${
                            availabilityConfirmed
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
                    onClick={onRequestBooking}
                    disabled={bookingLoading}
                    className="w-full rounded-lg bg-orange-500 px-4 py-3 font-semibold text-white transition hover:bg-orange-600 disabled:opacity-50"
                >
                    {bookingLoading ? "Submitting..." : "Request booking"}
                </button>

                <p className="text-xs text-slate-500">
                    Your booking request will be submitted for hotel approval.
                </p>
            </div>
        </aside>
    );
}
