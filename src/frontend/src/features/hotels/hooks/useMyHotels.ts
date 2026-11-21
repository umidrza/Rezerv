import { useCallback, useEffect, useState } from "react";

import {
    getMyHotels,
    updateHotel,
    activateHotel,
    deactivateHotel,
} from "../../../services/hotelService";

import type { Hotel, UpdateHotelRequest } from "../../../types/hotel";

interface UseMyHotelsResult {
    hotels: Hotel[];
    loading: boolean;
    saving: boolean;
    error: string;
    success: string;
    setError: (value: string) => void;
    setSuccess: (value: string) => void;
    updateHotelDetails: (
        hotelId: string,
        form: UpdateHotelRequest
    ) => Promise<void>;
    toggleHotelStatus: (hotel: Hotel) => Promise<void>;
}

export function useMyHotels(token: string | null): UseMyHotelsResult {
    const [hotels, setHotels] = useState<Hotel[]>([]);
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");

    const loadHotels = useCallback(async () => {
        if (!token) {
            setError("Please log in to continue.");
            setLoading(false);
            return;
        }

        try {
            setError("");
            const result = await getMyHotels(token);
            setHotels(result);
        } catch (err) {
            setError(
                err instanceof Error ? err.message : "Failed to load hotels."
            );
        } finally {
            setLoading(false);
        }
    }, [token]);

    useEffect(() => {
        loadHotels();
    }, [loadHotels]);

    async function updateHotelDetails(
        hotelId: string,
        form: UpdateHotelRequest
    ) {
        if (!token) return;

        setSaving(true);
        setError("");
        setSuccess("");

        try {
            const updatedHotel = await updateHotel(hotelId, form, token);

            setHotels((current) =>
                current.map((hotel) =>
                    hotel.id === hotelId ? updatedHotel : hotel
                )
            );

            setSuccess("Hotel updated successfully.");
        } catch (err) {
            setError(
                err instanceof Error ? err.message : "Failed to update hotel."
            );
            throw err;
        } finally {
            setSaving(false);
        }
    }

    async function toggleHotelStatus(hotel: Hotel) {
        if (!token) return;

        const action = hotel.isActive ? "deactivate" : "activate";

        if (!window.confirm(`Are you sure you want to ${action} this hotel?`)) {
            return;
        }

        setError("");
        setSuccess("");

        try {
            if (hotel.isActive) {
                await deactivateHotel(hotel.id, token);
            } else {
                await activateHotel(hotel.id, token);
            }

            setHotels((current) =>
                current.map((item) =>
                    item.id === hotel.id
                        ? { ...item, isActive: !item.isActive }
                        : item
                )
            );

            setSuccess(
                `Hotel ${hotel.isActive ? "deactivated" : "activated"} successfully.`
            );
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : "Failed to change hotel status."
            );
        }
    }

    return {
        hotels,
        loading,
        saving,
        error,
        success,
        setError,
        setSuccess,
        updateHotelDetails,
        toggleHotelStatus,
    };
}
