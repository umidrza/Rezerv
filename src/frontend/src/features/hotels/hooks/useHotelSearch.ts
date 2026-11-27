
import { useState } from "react";

import { searchHotels } from "../../../services/hotelService";
import type { Hotel } from "../../../types/hotel";

export function useHotelSearch() {
    const [city, setCity] = useState("");
    const [country, setCountry] = useState("");
    const [checkIn, setCheckIn] = useState("");
    const [checkOut, setCheckOut] = useState("");
    const [guests, setGuests] = useState("");
    const [starRating, setStarRating] = useState("");
    const [minimumAverageRating, setMinimumAverageRating] = useState("");
    const [sortBy, setSortBy] = useState("Name");

    const [hotels, setHotels] = useState<Hotel[]>([]);
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(0);

    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");

    async function loadHotels(pageNumber: number) {
        setLoading(true);
        setError("");

        try {
            const result = await searchHotels({
                city: city || undefined,
                country: country || undefined,
                checkIn: checkIn || undefined,
                checkOut: checkOut || undefined,
                numberOfGuests: guests ? Number(guests) : undefined,
                starRating: starRating ? Number(starRating) : undefined,
                minimumAverageRating: minimumAverageRating
                    ? Number(minimumAverageRating)
                    : undefined,
                sortBy,
                sortDescending: true,
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
            setTotalPages(0);
        } finally {
            setLoading(false);
        }
    }

    return {
        city,
        setCity,
        country,
        setCountry,
        checkIn,
        setCheckIn,
        checkOut,
        setCheckOut,
        guests,
        setGuests,
        starRating,
        setStarRating,
        minimumAverageRating,
        setMinimumAverageRating,
        sortBy,
        setSortBy,
        hotels,
        page,
        totalPages,
        loading,
        error,
        loadHotels,
    };
}
