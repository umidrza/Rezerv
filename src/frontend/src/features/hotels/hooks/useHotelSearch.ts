
import { useCallback, useRef, useState } from "react";

import { searchHotels } from "../../../services/hotelService";
import type { Hotel } from "../../../types/hotel";

export function useHotelSearch() {
    const [city, setCityState] = useState("");
    const [country, setCountryState] = useState("");
    const [checkIn, setCheckInState] = useState("");
    const [checkOut, setCheckOutState] = useState("");
    const [guests, setGuestsState] = useState("");
    const [starRating, setStarRatingState] = useState("");
    const [minimumAverageRating, setMinimumAverageRatingState] = useState("");
    const [sortBy, setSortByState] = useState("Name");

    const [hotels, setHotels] = useState<Hotel[]>([]);
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(0);

    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");

    const requestIdRef = useRef(0);
    const filtersRef = useRef({
        city,
        country,
        checkIn,
        checkOut,
        guests,
        starRating,
        minimumAverageRating,
        sortBy,
    });

    const setCity = (value: string) => {
        filtersRef.current.city = value;
        setCityState(value);
    };
    const setCountry = (value: string) => {
        filtersRef.current.country = value;
        setCountryState(value);
    };
    const setCheckIn = (value: string) => {
        filtersRef.current.checkIn = value;
        setCheckInState(value);
    };
    const setCheckOut = (value: string) => {
        filtersRef.current.checkOut = value;
        setCheckOutState(value);
    };
    const setGuests = (value: string) => {
        filtersRef.current.guests = value;
        setGuestsState(value);
    };
    const setStarRating = (value: string) => {
        filtersRef.current.starRating = value;
        setStarRatingState(value);
    };
    const setMinimumAverageRating = (value: string) => {
        filtersRef.current.minimumAverageRating = value;
        setMinimumAverageRatingState(value);
    };
    const setSortBy = (value: string) => {
        filtersRef.current.sortBy = value;
        setSortByState(value);
    };

    const loadHotels = useCallback(async (
        pageNumber: number,
        signal?: AbortSignal
    ) => {
        const requestId = ++requestIdRef.current;
        const filters = filtersRef.current;
        setLoading(true);
        setError("");

        try {
            const result = await searchHotels({
                city: filters.city || undefined,
                country: filters.country || undefined,
                checkIn: filters.checkIn || undefined,
                checkOut: filters.checkOut || undefined,
                numberOfGuests: filters.guests
                    ? Number(filters.guests)
                    : undefined,
                starRating: filters.starRating
                    ? Number(filters.starRating)
                    : undefined,
                minimumAverageRating: filters.minimumAverageRating
                    ? Number(filters.minimumAverageRating)
                    : undefined,
                sortBy: filters.sortBy,
                sortDescending: true,
                page: pageNumber,
            }, signal);

            if (signal?.aborted || requestId !== requestIdRef.current) return;
            setHotels(result.items);
            setTotalPages(result.totalPages);
            setPage(pageNumber);
        } catch (err) {
            if (
                signal?.aborted ||
                (err instanceof Error && err.name === "AbortError") ||
                requestId !== requestIdRef.current
            ) {
                return;
            }

            setError(
                err instanceof Error ? err.message : "Something went wrong."
            );
            setHotels([]);
            setTotalPages(0);
        } finally {
            if (requestId === requestIdRef.current) {
                setLoading(false);
            }
        }
    }, []);

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
