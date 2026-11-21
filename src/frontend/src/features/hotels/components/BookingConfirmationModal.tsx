
interface BookingConfirmationModalProps {
    isOpen: boolean;
    hotelName: string;
    roomId: string;
    checkIn: string;
    checkOut: string;
    numberOfGuests: number;
    loading: boolean;
    onClose: () => void;
    onConfirm: () => void;
}

export default function BookingConfirmationModal({
    isOpen,
    hotelName,
    roomId,
    checkIn,
    checkOut,
    numberOfGuests,
    loading,
    onClose,
    onConfirm,
}: BookingConfirmationModalProps) {
    if (!isOpen) return null;

    return (
        <div
            className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4"
            role="dialog"
            aria-modal="true"
            aria-labelledby="booking-confirmation-title"
        >
            <div className="w-full max-w-md rounded-xl bg-white p-6 shadow-xl">
                <h2
                    id="booking-confirmation-title"
                    className="text-xl font-bold text-slate-900"
                >
                    Confirm your booking
                </h2>

                <div className="mt-5 space-y-3 text-sm">
                    <div className="flex justify-between gap-4">
                        <span className="text-slate-500">Hotel</span>
                        <span className="text-right font-medium">
                            {hotelName}
                        </span>
                    </div>

                    <div className="flex justify-between gap-4">
                        <span className="text-slate-500">Room</span>
                        <span className="font-medium">{roomId}</span>
                    </div>

                    <div className="flex justify-between gap-4">
                        <span className="text-slate-500">Check-in</span>
                        <span className="font-medium">{checkIn}</span>
                    </div>

                    <div className="flex justify-between gap-4">
                        <span className="text-slate-500">Check-out</span>
                        <span className="font-medium">{checkOut}</span>
                    </div>

                    <div className="flex justify-between gap-4">
                        <span className="text-slate-500">Guests</span>
                        <span className="font-medium">{numberOfGuests}</span>
                    </div>
                </div>

                <p className="mt-5 text-sm text-slate-500">
                    Your request will be sent to the hotel for approval.
                </p>

                <div className="mt-6 flex justify-end gap-3">
                    <button
                        type="button"
                        onClick={onClose}
                        disabled={loading}
                        className="rounded-lg border border-slate-300 px-4 py-2 text-slate-700 hover:bg-slate-50 disabled:opacity-50"
                    >
                        Go back
                    </button>

                    <button
                        type="button"
                        onClick={onConfirm}
                        disabled={loading}
                        className="rounded-lg bg-orange-500 px-4 py-2 font-semibold text-white hover:bg-orange-600 disabled:opacity-50"
                    >
                        {loading ? "Submitting..." : "Confirm booking"}
                    </button>
                </div>
            </div>
        </div>
    );
}
