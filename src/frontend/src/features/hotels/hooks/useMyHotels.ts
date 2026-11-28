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

export function useMyHotels(): UseMyHotelsResult {
    const [hotels, setHotels] = useState<Hotel[]>([]);
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");

    const loadHotels = useCallback(async () => {
        try {
            setError("");
            const result = await getMyHotels();
            setHotels(result);
        } catch (err) {
            setError(
                err instanceof Error ? err.message : "Failed to load hotels."
            );
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        loadHotels();
    }, [loadHotels]);

    async function updateHotelDetails(
        hotelId: string,
        form: UpdateHotelRequest
    ) {
        setSaving(true);
        setError("");
        setSuccess("");

        try {
            const updatedHotel = await updateHotel(hotelId, form);

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
        const action = hotel.isActive ? "deactivate" : "activate";

        if (!window.confirm(`Are you sure you want to ${action} this hotel?`)) {
            return;
        }

        setError("");
        setSuccess("");

        try {
            if (hotel.isActive) {
                await deactivateHotel(hotel.id);
            } else {
                await activateHotel(hotel.id);
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
