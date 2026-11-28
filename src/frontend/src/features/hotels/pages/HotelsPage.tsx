import { useCallback, useEffect, useRef, useState } from "react";

import HotelCard from "../components/HotelCard";
import HotelSearchForm from "../components/HotelSearchForm";
import { useHotelSearch } from "../hooks/useHotelSearch";
import { useAuth } from "../../auth/AuthContext";
import {
  getMyFavorites,
  addFavorite,
  removeFavorite,
} from "../../../services/hotelFavoriteService";

export default function HotelsPage() {
  const search = useHotelSearch();
  const { loadHotels } = search;
  const { isCustomer } = useAuth();

  const [favoriteIds, setFavoriteIds] = useState<Set<string>>(new Set());
  const [favoriteLoadingId, setFavoriteLoadingId] = useState<string | null>(
    null,
  );
  const [favoriteError, setFavoriteError] = useState("");

  const hotelsSectionRef = useRef<HTMLElement>(null);
  const searchControllerRef = useRef<AbortController | null>(null);

  const runSearch = useCallback((pageNumber: number) => {
    searchControllerRef.current?.abort();

    const controller = new AbortController();
    searchControllerRef.current = controller;

    void loadHotels(pageNumber, controller.signal).finally(() => {
      if (searchControllerRef.current === controller) {
        searchControllerRef.current = null;
      }
    });
  }, [loadHotels]);

  useEffect(() => {
    searchControllerRef.current?.abort();
  }, [
    search.city,
    search.country,
    search.checkIn,
    search.checkOut,
    search.guests,
    search.starRating,
    search.minimumAverageRating,
    search.sortBy,
  ]);

  useEffect(() => {
    runSearch(1);
    return () => searchControllerRef.current?.abort();
  }, [runSearch]);

  useEffect(() => {
    if (!isCustomer) {
      return;
    }

    getMyFavorites()
      .then((favorites) => {
        setFavoriteIds(new Set(favorites.map((hotel) => hotel.id)));
      })
      .catch((err) => {
        setFavoriteError(
          err instanceof Error ? err.message : "Failed to load favorites.",
        );
      });
  }, [isCustomer]);

  async function handleToggleFavorite(hotelId: string) {
    if (favoriteLoadingId) return;

    const currentlyFavorite = favoriteIds.has(hotelId);

    setFavoriteLoadingId(hotelId);
    setFavoriteError("");

    try {
      if (currentlyFavorite) {
        await removeFavorite(hotelId);
      } else {
        await addFavorite(hotelId);
      }

      setFavoriteIds((previous) => {
        const updated = new Set(previous);

        if (currentlyFavorite) {
          updated.delete(hotelId);
        } else {
          updated.add(hotelId);
        }

        return updated;
      });
    } catch (err) {
      setFavoriteError(
        err instanceof Error ? err.message : "Failed to update favorites.",
      );
    } finally {
      setFavoriteLoadingId(null);
    }
  }

  function handleSearch(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    runSearch(1);

    setTimeout(() => {
      hotelsSectionRef.current?.scrollIntoView({
        behavior: "smooth",
        block: "start",
      });
    }, 100);
  }

  return (
    <main className="min-h-screen bg-slate-50">
      <section className="bg-orange-500 px-6 py-20 text-center text-white">
        <h1 className="text-4xl font-bold">Find your perfect stay</h1>
        <p className="mt-4 text-lg">
          Discover hotels and make your next trip memorable.
        </p>
      </section>

      <section className="mx-auto -mt-8 max-w-6xl px-6">
        <HotelSearchForm
          city={search.city}
          setCity={search.setCity}
          country={search.country}
          setCountry={search.setCountry}
          checkIn={search.checkIn}
          setCheckIn={search.setCheckIn}
          checkOut={search.checkOut}
          setCheckOut={search.setCheckOut}
          guests={search.guests}
          setGuests={search.setGuests}
          starRating={search.starRating}
          setStarRating={search.setStarRating}
          minimumAverageRating={search.minimumAverageRating}
          setMinimumAverageRating={search.setMinimumAverageRating}
          sortBy={search.sortBy}
          setSortBy={search.setSortBy}
          loading={search.loading}
          onSearch={handleSearch}
        />
      </section>

      <section className="mx-auto max-w-6xl px-6 py-12" ref={hotelsSectionRef}>
        <h2 className="mb-6 text-2xl font-bold text-slate-900">
          Discover hotels
        </h2>

        {(search.error || favoriteError) && (
          <p className="mb-6 rounded-lg bg-red-100 p-4 text-red-700">
            {search.error || favoriteError}
          </p>
        )}

        {!search.loading && search.hotels.length === 0 && !search.error && (
          <p className="text-slate-500">No hotels found.</p>
        )}

        <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
          {search.hotels.map((hotel) => (
            <HotelCard
              key={hotel.id}
              hotel={hotel}
              isFavorite={isCustomer && favoriteIds.has(hotel.id)}
              onToggleFavorite={
                isCustomer ? () => handleToggleFavorite(hotel.id) : undefined
              }
              favoriteLoading={favoriteLoadingId === hotel.id}
            />
          ))}
        </div>

        {search.totalPages > 1 && (
          <div className="mt-8 flex items-center justify-center gap-4">
            <button
              disabled={search.page === 1 || search.loading}
              onClick={() => runSearch(search.page - 1)}
              className="rounded-lg border px-4 py-2 disabled:opacity-40"
            >
              Previous
            </button>

            <span className="text-slate-600">
              Page {search.page} of {search.totalPages}
            </span>

            <button
              disabled={search.page === search.totalPages || search.loading}
              onClick={() => runSearch(search.page + 1)}
              className="rounded-lg border px-4 py-2 disabled:opacity-40"
            >
              Next
            </button>
          </div>
        )}
      </section>
    </main>
  );
}
