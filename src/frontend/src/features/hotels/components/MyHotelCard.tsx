
import { Link } from "react-router";
import type { Hotel } from "../../../types/hotel";

interface MyHotelCardProps {
    hotel: Hotel;
    onEdit: (hotel: Hotel) => void;
    onToggleStatus: (hotel: Hotel) => void;
}

export default function MyHotelCard({
    hotel,
    onEdit,
    onToggleStatus,
}: MyHotelCardProps) {
    return (
        <article className="overflow-hidden rounded-xl bg-white shadow-sm">
            {hotel.primaryImageUrl ? (
                <img
                    src={hotel.primaryImageUrl}
                    alt={hotel.name}
                    className="h-48 w-full object-cover"
                />
            ) : (
                <div className="flex h-48 items-center justify-center bg-slate-200 text-slate-500">
                    No image available
                </div>
            )}

            <div className="p-5">
                <div className="flex items-center justify-between gap-3">
                    <h2 className="text-xl font-semibold text-slate-900">
                        {hotel.name}
                    </h2>

                    <span
                        className={`rounded-full px-3 py-1 text-xs font-semibold ${
                            hotel.isActive
                                ? "bg-green-100 text-green-700"
                                : "bg-red-100 text-red-700"
                        }`}
                    >
                        {hotel.isActive ? "Active" : "Inactive"}
                    </span>
                </div>

                <p className="mt-2 text-sm text-slate-500">
                    {hotel.city}, {hotel.country}
                </p>

                <p className="mt-2 text-sm text-slate-600">
                    {"★".repeat(hotel.starRating)}
                </p>

                <button
                    type="button"
                    onClick={() => onEdit(hotel)}
                    className="mt-5 w-full rounded-lg border border-orange-500 px-4 py-3 font-semibold text-orange-600 hover:bg-orange-50"
                >
                    Edit Hotel
                </button>

                <button
                    type="button"
                    onClick={() => onToggleStatus(hotel)}
                    className={`mt-3 w-full rounded-lg px-4 py-3 font-semibold text-white ${
                        hotel.isActive
                            ? "bg-red-500 hover:bg-red-600"
                            : "bg-green-600 hover:bg-green-700"
                    }`}
                >
                    {hotel.isActive ? "Deactivate Hotel" : "Activate Hotel"}
                </button>

                <Link
                    to={`/hotels/${hotel.id}/bookings`}
                    className="mt-3 block rounded-lg bg-orange-500 px-4 py-3 text-center font-semibold text-white hover:bg-orange-600"
                >
                    Manage Bookings
                </Link>
            </div>
        </article>
    );
}
