import { useEffect, useState } from "react";
import { Link } from "react-router";

import HotelCard from "../components/HotelCard";
import LoadingState from "../../../components/LoadingState";
import { useAuth } from "../../auth/AuthContext";
import {
  getMyFavorites,
  removeFavorite,
} from "../../../services/hotelFavoriteService";
import type { Hotel } from "../../../types/hotel";

export default function MyFavoritesPage() {
  const { token } = useAuth();

  const [favorites, setFavorites] = useState<Hotel[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!token) return;

    getMyFavorites(token)
      .then(setFavorites)
      .catch((err) => {
        setError(
          err instanceof Error ? err.message : "Failed to load favorites.",
        );
      })
      .finally(() => setLoading(false));
  }, [token]);

  async function handleRemoveFavorite(hotelId: string) {
    if (!token) return;

    try {
      await removeFavorite(hotelId, token);
      setFavorites((previous) =>
        previous.filter((hotel) => hotel.id !== hotelId),
      );
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Failed to remove favorite.",
      );
    }
  }

  if (loading) return <LoadingState />;

  return (
    <main className="min-h-screen bg-slate-50 px-6 py-12">
      <div className="mx-auto max-w-6xl">
        <div className="mb-8 flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold text-slate-900">My Favorites</h1>
            <p className="mt-2 text-slate-500">
              Hotels you've saved for later.
            </p>
          </div>

          <Link
            to="/hotels"
            className="rounded-lg bg-orange-500 px-4 py-2 font-semibold text-white hover:bg-orange-600"
          >
            Explore hotels
          </Link>
        </div>

        {error && (
          <p className="mb-6 rounded-lg bg-red-100 p-4 text-red-700">{error}</p>
        )}

        {favorites.length === 0 && !error && (
          <p className="rounded-xl bg-white p-8 text-center text-slate-500 shadow">
            You haven't saved any hotels yet.
          </p>
        )}

        <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
          {favorites.map((hotel) => (
            <HotelCard
              key={hotel.id}
              hotel={hotel}
              isFavorite
              onToggleFavorite={() => handleRemoveFavorite(hotel.id)}
            />
          ))}
        </div>
      </div>
    </main>
  );
}
