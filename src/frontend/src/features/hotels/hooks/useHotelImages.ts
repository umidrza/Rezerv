import { useCallback, useEffect, useState } from "react";

import {
    getHotelImages,
    uploadHotelImage,
    setPrimaryHotelImage,
    deleteHotelImage,
} from "../../../services/hotelImageService";

import type { HotelImage } from "../../../types/hotel";

export function useHotelImages(
    hotelId: string,
    initialPrimaryImageId: string | null
) {
    const [images, setImages] = useState<HotelImage[]>([]);
    const [primaryImageId, setPrimaryImageId] = useState<string | null>(
        initialPrimaryImageId
    );
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");

    const loadImages = useCallback(async () => {
        try {
            setLoading(true);
            setError("");

            const result = await getHotelImages(hotelId);
            setImages(result);
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : "Failed to load hotel images."
            );
        } finally {
            setLoading(false);
        }
    }, [hotelId]);

    useEffect(() => {
        setPrimaryImageId(initialPrimaryImageId);
    }, [initialPrimaryImageId]);

    useEffect(() => {
        if (hotelId) {
            loadImages();
        } else {
            setLoading(false);
        }
    }, [hotelId, loadImages]);

    async function uploadImage(file: File) {
        try {
            setSaving(true);
            setError("");
            setSuccess("");

            const image = await uploadHotelImage(hotelId, file);

            setImages((current) => [...current, image]);

            if (!primaryImageId) {
                setPrimaryImageId(image.id);
            }

            setSuccess("Image uploaded successfully.");
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : "Failed to upload image."
            );
        } finally {
            setSaving(false);
        }
    }

    async function selectPrimaryImage(imageId: string) {
        try {
            setSaving(true);
            setError("");
            setSuccess("");

            await setPrimaryHotelImage(hotelId, imageId);

            setPrimaryImageId(imageId);
            setSuccess("Primary image updated successfully.");
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : "Failed to update primary image."
            );
        } finally {
            setSaving(false);
        }
    }

    async function removeImage(imageId: string) {
        try {
            setSaving(true);
            setError("");
            setSuccess("");

            await deleteHotelImage(hotelId, imageId);

            setImages((current) =>
                current.filter((image) => image.id !== imageId)
            );

            if (primaryImageId === imageId) {
                setPrimaryImageId(null);
            }

            setSuccess("Image deleted successfully.");
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : "Failed to delete image."
            );
        } finally {
            setSaving(false);
        }
    }

    return {
        images,
        primaryImageId,
        loading,
        saving,
        error,
        success,
        loadImages,
        uploadImage,
        selectPrimaryImage,
        removeImage,
    };
}