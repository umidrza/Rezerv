import type { Hotel } from "../../../types/hotel";
import { Link } from "react-router";

interface HotelCardProps {
    hotel: Hotel;
}

export default function HotelCard({ hotel }: HotelCardProps) {
    return (
        <Link to={`/hotels/${hotel.id}`} className="block">
            <article className="overflow-hidden rounded-xl bg-white shadow transition hover:-translate-y-1 hover:shadow-lg">
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
                    <h3 className="text-xl font-bold text-slate-900">
                        {hotel.name}
                    </h3>

                    <p className="mt-2 text-sm text-slate-500">
                        {hotel.city}, {hotel.country}
                    </p>

                    <p className="mt-2 text-orange-500">
                        {"★".repeat(hotel.starRating)}
                    </p>

                    <p className="mt-3 text-slate-600">
                        {hotel.description}
                    </p>
                </div>
            </article>
        </Link>
    );
}
