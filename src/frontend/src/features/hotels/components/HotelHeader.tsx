
import type { Hotel } from "../../../types/hotel";

interface HotelHeaderProps {
    hotel: Hotel;
}

export default function HotelHeader({ hotel }: HotelHeaderProps) {
    return (
        <section className="overflow-hidden rounded-xl bg-white shadow-sm">
            {hotel.primaryImageUrl && (
                <img
                    src={hotel.primaryImageUrl}
                    alt={hotel.name}
                    className="h-72 w-full object-cover"
                />
            )}

            <div className="p-6">
                <div className="flex flex-wrap items-center justify-between gap-3">
                    <h1 className="text-3xl font-bold text-slate-900">
                        {hotel.name}
                    </h1>

                    <span className="rounded-full bg-orange-100 px-3 py-1 text-sm font-semibold text-orange-700">
                        {hotel.starRating} Stars
                    </span>
                </div>

                <p className="mt-2 text-slate-500">
                    {hotel.address}, {hotel.city}, {hotel.country}
                </p>

                <p className="mt-5 leading-7 text-slate-700">
                    {hotel.description}
                </p>
            </div>
        </section>
    );
}
