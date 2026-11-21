
import DatePicker from "../../../components/DatePicker";
import { parseISO } from "date-fns";

interface HotelSearchFormProps {
    city: string;
    setCity: (value: string) => void;
    checkIn: string;
    setCheckIn: (value: string) => void;
    checkOut: string;
    setCheckOut: (value: string) => void;
    guests: string;
    setGuests: (value: string) => void;
    starRating: string;
    setStarRating: (value: string) => void;
    loading: boolean;
    onSearch: (event: React.FormEvent<HTMLFormElement>) => void;
}

export default function HotelSearchForm({
    city,
    setCity,
    checkIn,
    setCheckIn,
    checkOut,
    setCheckOut,
    guests,
    setGuests,
    starRating,
    setStarRating,
    loading,
    onSearch,
}: HotelSearchFormProps) {
    function handleCheckInChange(value: string) {
        setCheckIn(value);

        if (checkOut && value && checkOut <= value) {
            setCheckOut("");
        }
    }

    return (
        <form
            onSubmit={onSearch}
            className="grid gap-4 rounded-xl bg-white p-6 shadow-lg md:grid-cols-3"
        >
            <div>
                <label className="mb-2 block text-sm font-medium">
                    City
                </label>
                <input
                    value={city}
                    onChange={(e) => setCity(e.target.value)}
                    placeholder="Where are you going?"
                    className="h-12 w-full rounded-lg border border-slate-300 bg-white px-3 text-slate-700 placeholder:text-slate-400 outline-none transition focus:border-orange-500 focus:ring-2 focus:ring-orange-100"
                />
            </div>

            <div>
                <label className="mb-2 block text-sm font-medium">
                    Check-in
                </label>
                <DatePicker
                    value={checkIn}
                    onChange={handleCheckInChange}
                    placeholder="Select check-in"
                />
            </div>

            <div>
                <label className="mb-2 block text-sm font-medium">
                    Check-out
                </label>
                <DatePicker
                    value={checkOut}
                    onChange={setCheckOut}
                    placeholder="Select check-out"
                    minDate={checkIn ? parseISO(checkIn) : undefined}
                />
            </div>

            <div>
                <label className="mb-2 block text-sm font-medium">
                    Guests
                </label>
                <input
                    type="number"
                    min="1"
                    value={guests}
                    onChange={(e) => setGuests(e.target.value)}
                    placeholder="Number of guests"
                    className="h-12 w-full rounded-lg border border-slate-300 bg-white px-3 text-slate-700 placeholder:text-slate-400 outline-none transition focus:border-orange-500 focus:ring-2 focus:ring-orange-100"
                />
            </div>

            <div>
                <label className="mb-2 block text-sm font-medium">
                    Star rating
                </label>
                <select
                    value={starRating}
                    onChange={(e) => setStarRating(e.target.value)}
                    className="h-12 w-full rounded-lg border border-slate-300 bg-white px-3 text-slate-700 outline-none transition focus:border-orange-500 focus:ring-2 focus:ring-orange-100"
                >
                    <option value="">Any rating</option>
                    <option value="3">3 stars</option>
                    <option value="4">4 stars</option>
                    <option value="5">5 stars</option>
                </select>
            </div>

            <div className="flex items-end">
                <button
                    type="submit"
                    disabled={loading}
                    className="w-full rounded-lg bg-orange-500 p-3 font-semibold text-white hover:bg-orange-600 disabled:opacity-50"
                >
                    {loading ? "Searching..." : "Search hotels"}
                </button>
            </div>
        </form>
    );
}
