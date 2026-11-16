
import { useState } from "react";
import { searchHotels } from "../../../services/hotelService";
import type { Hotel } from "../../../types/hotel";
import HotelCard from "../components/HotelCard";
import DatePicker from "@/components/DatePicker";
import { parseISO } from "date-fns";

export default function HotelsPage() {
    const [city, setCity] = useState("");
    const [checkIn, setCheckIn] = useState("");
    const [checkOut, setCheckOut] = useState("");
    const [guests, setGuests] = useState("");
    const [starRating, setStarRating] = useState("");
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(0);

    const [hotels, setHotels] = useState<Hotel[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");

    async function loadHotels(pageNumber: number) {
        setLoading(true);
        setError("");

        try {
            const result = await searchHotels({
                city: city || undefined,
                checkIn: checkIn || undefined,
                checkOut: checkOut || undefined,
                numberOfGuests: guests ? Number(guests) : undefined,
                starRating: starRating ? Number(starRating) : undefined,
                page: pageNumber,
            });

            setHotels(result.items);
            setTotalPages(result.totalPages);
            setPage(pageNumber);
        } catch (err) {
            setError(
                err instanceof Error ? err.message : "Something went wrong."
            );
            setHotels([]);
        } finally {
            setLoading(false);
        }
    }

    async function handleSearch(event: React.FormEvent<HTMLFormElement>) {
        event.preventDefault();
        await loadHotels(1);
    }

    return (
        <div className="min-h-screen bg-slate-50">
            <section className="bg-orange-500 px-6 py-20 text-center text-white">
                <h1 className="text-4xl font-bold">
                    Find your perfect stay
                </h1>
                <p className="mt-4 text-lg">
                    Discover hotels and make your next trip memorable.
                </p>
            </section>

            <section className="mx-auto -mt-8 max-w-6xl px-6">
                <form
                    onSubmit={handleSearch}
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
                            onChange={(value) => {
                                setCheckIn(value);

                                if (checkOut && value && checkOut <= value) {
                                    setCheckOut("");
                                }
                            }}
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
            </section>

            <section className="mx-auto max-w-6xl px-6 py-12">
                <h2 className="mb-6 text-2xl font-bold text-slate-900">
                    {hotels.length > 0 ? "Search results" : "Discover hotels"}
                </h2>

                {error && (
                    <p className="mb-6 rounded-lg bg-red-100 p-4 text-red-700">
                        {error}
                    </p>
                )}

                {!loading && hotels.length === 0 && !error && (
                    <p className="text-slate-500">
                        Search for hotels to see available results.
                    </p>
                )}

                <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
                    {hotels.map((hotel) => (
                        <HotelCard key={hotel.id} hotel={hotel} />
                    ))}
                </div>

                {totalPages > 1 && (
                    <div className="mt-8 flex items-center justify-center gap-4">
                        <button
                            disabled={page === 1 || loading}
                            onClick={() => loadHotels(page - 1)}
                            className="rounded-lg border px-4 py-2 disabled:opacity-40"
                        >
                            Previous
                        </button>

                        <span className="text-slate-600">
                            Page {page} of {totalPages}
                        </span>

                        <button
                            disabled={page === totalPages || loading}
                            onClick={() => loadHotels(page + 1)}
                            className="rounded-lg border px-4 py-2 disabled:opacity-40"
                        >
                            Next
                        </button>
                    </div>
                )}
            </section>
        </div>
    );
}
