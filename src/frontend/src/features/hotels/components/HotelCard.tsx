import type { Hotel } from "../../../types/hotel";
import { Link } from "react-router";
import { Heart } from "lucide-react";

interface HotelCardProps {
  hotel: Hotel;
  isFavorite?: boolean;
  onToggleFavorite?: () => void;
  favoriteLoading?: boolean;
}

export default function HotelCard({
  hotel,
  isFavorite = false,
  onToggleFavorite,
  favoriteLoading = false,
}: HotelCardProps) {
  return (
    <article className="relative overflow-hidden rounded-xl bg-white shadow transition hover:-translate-y-1 hover:shadow-lg">
      {onToggleFavorite && (
        <button
          type="button"
          onClick={onToggleFavorite}
          disabled={favoriteLoading}
          aria-label={isFavorite ? "Remove from favorites" : "Add to favorites"}
          className="absolute right-3 top-3 z-10 rounded-full bg-white p-2 shadow transition hover:scale-110 disabled:opacity-50"
        >
          <Heart
            size={22}
            className={
              isFavorite ? "fill-red-500 text-red-500" : "text-slate-600"
            }
          />
        </button>
      )}

      <Link to={`/hotels/${hotel.id}`} className="block">
        {hotel.primaryImageUrl ? (
          <img
            src={hotel.primaryImageUrl}
            alt={hotel.name}
            className="h-52 w-full object-cover"
          />
        ) : (
          <div className="flex h-52 items-center justify-center bg-slate-200 text-slate-500">
            No image available
          </div>
        )}

        <div className="p-5">
          <h3 className="text-xl font-bold text-slate-900">{hotel.name}</h3>

          <p className="mt-2 text-sm text-slate-500">
            {hotel.city}, {hotel.country}
          </p>

          <p className="mt-2 text-orange-500">{"★".repeat(hotel.starRating)}</p>

          <p className="mt-2 text-sm text-slate-600">
            {hotel.averageRating > 0
              ? `${hotel.averageRating.toFixed(1)} / 5 (${hotel.reviewCount} reviews)`
              : "No guest reviews yet"}
          </p>

          <p className="mt-3 text-slate-600">{hotel.description}</p>
        </div>
      </Link>
    </article>
  );
}
