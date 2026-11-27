
import { useCallback, useEffect, useState } from "react";

import type {
    HotelReview,
    CreateHotelReviewRequest,
} from "../../../types/hotelReview";

import {
    getHotelReviews,
    createHotelReview,
} from "../../../services/hotelReviewService";

export function useHotelReviews(
    hotelId: string | undefined,
    token: string | null
) {
    const [reviews, setReviews] = useState<HotelReview[]>([]);
    const [loading, setLoading] = useState(true);
    const [submitting, setSubmitting] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const fetchReviews = useCallback(async () => {
        if (!hotelId) return;

        try {
            setLoading(true);
            setError(null);

            const data = await getHotelReviews(hotelId);
            setReviews(data);
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : "Failed to load reviews."
            );
        } finally {
            setLoading(false);
        }
    }, [hotelId]);

    useEffect(() => {
        fetchReviews();
    }, [fetchReviews]);

    const submitReview = async (data: CreateHotelReviewRequest) => {
        if (!hotelId || !token) {
            throw new Error("You must be logged in to submit a review.");
        }

        try {
            setSubmitting(true);
            setError(null);

            const review = await createHotelReview(hotelId, data, token);

            setReviews((current) => [review, ...current]);

            return review;
        } catch (err) {
            const message =
                err instanceof Error
                    ? err.message
                    : "Failed to submit review.";

            setError(message);
            throw err;
        } finally {
            setSubmitting(false);
        }
    };

    return {
        reviews,
        loading,
        submitting,
        error,
        submitReview,
        refreshReviews: fetchReviews,
    };
}
