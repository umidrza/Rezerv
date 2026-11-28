import { useCallback, useEffect, useState } from "react";

import {
  addFavorite,
  isFavorite as checkIsFavorite,
  removeFavorite,
} from "../../../services/hotelFavoriteService";
import { useAuth } from "../../auth/AuthContext";

export function useHotelFavorite(hotelId: string) {
  const { isCustomer } = useAuth();

  const [isFavorite, setIsFavorite] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const loadFavoriteStatus = useCallback(async () => {
    if (!isCustomer) {
      setIsFavorite(false);
      return;
    }

    try {
      const result = await checkIsFavorite(hotelId);
      setIsFavorite(result);
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Failed to check favorite status.",
      );
    }
  }, [hotelId, isCustomer]);

  useEffect(() => {
    loadFavoriteStatus();
  }, [loadFavoriteStatus]);

  async function toggleFavorite() {
    if (!isCustomer || loading) return;

    setLoading(true);
    setError("");

    try {
      if (isFavorite) {
        await removeFavorite(hotelId);
        setIsFavorite(false);
      } else {
        await addFavorite(hotelId);
        setIsFavorite(true);
      }
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Failed to update favorites.",
      );
    } finally {
      setLoading(false);
    }
  }

  return {
    isFavorite,
    loading,
    error,
    toggleFavorite,
  };
}
