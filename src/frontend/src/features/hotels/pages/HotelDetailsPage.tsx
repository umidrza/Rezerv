
import { useEffect, useState } from "react";
import { Link, useParams } from "react-router";
import { getHotelById } from "../../../services/hotelService";
import type { Hotel } from "../../../types/hotel";
import { getRoomTypesByHotel } from "../../../services/hotelService";
import type { RoomType } from "../../../types/roomType";

export default function HotelDetailsPage() {
    const { id } = useParams();

    const [hotel, setHotel] = useState<Hotel | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [roomTypes, setRoomTypes] = useState<RoomType[]>([]);

    useEffect(() => {
        async function loadHotel() {
            if (!id) {
                setError("Hotel ID is missing.");
                setLoading(false);
                return;
            }

            try {
                const result = await getHotelById(id);
                setHotel(result);

                const roomTypesResult = await getRoomTypesByHotel(id);
                setRoomTypes(roomTypesResult);
            } catch (err) {
                setError(
                    err instanceof Error
                        ? err.message
                        : "Something went wrong."
                );
            } finally {
                setLoading(false);
            }
        }

        loadHotel();
    }, [id]);

    if (loading) {
        return <p className="p-10 text-center">Loading hotel...</p>;
    }

    if (error || !hotel) {
        return (
            <div className="p-10 text-center">
                <p className="text-red-600">
                    {error || "Hotel not found."}
                </p>
                <Link to="/" className="mt-4 inline-block text-orange-600">
                    Back to home
                </Link>
            </div>
        );
    }

    return (
        <div className="mx-auto max-w-6xl px-6 py-10">
            <Link to="/" className="text-orange-600 hover:underline">
                ← Back to hotels
            </Link>

            {hotel.primaryImageUrl && (
                <img
                    src={hotel.primaryImageUrl}
                    alt={hotel.name}
                    className="mt-6 h-96 w-full rounded-xl object-cover"
                />
            )}

            <div className="mt-8">
                <h1 className="text-3xl font-bold text-slate-900">
                    {hotel.name}
                </h1>

                <p className="mt-2 text-slate-500">
                    {hotel.address}, {hotel.city}, {hotel.country}
                </p>

                <p className="mt-3 text-xl text-orange-500">
                    {"★".repeat(hotel.starRating)}
                </p>

                <p className="mt-6 leading-7 text-slate-700">
                    {hotel.description}
                </p>
            </div>

            <section className="mt-12">
                <h2 className="mb-6 text-2xl font-bold text-slate-900">
                    Available room types
                </h2>

                {roomTypes.length === 0 ? (
                    <p className="text-slate-500">
                        No room types available for this hotel.
                    </p>
                ) : (
                    <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
                        {roomTypes.map((roomType) => (
                            <div
                                key={roomType.id}
                                className="rounded-xl bg-white p-6 shadow"
                            >
                                <h3 className="text-xl font-bold">
                                    {roomType.name}
                                </h3>

                                <p className="mt-2 text-slate-600">
                                    {roomType.description}
                                </p>

                                <p className="mt-3 text-slate-500">
                                    Capacity: {roomType.capacity} guests
                                </p>

                                <p className="mt-4 text-xl font-bold text-orange-600">
                                    ${roomType.pricePerNight}
                                    <span className="text-sm font-normal text-slate-500">
                                        {" "} / night
                                    </span>
                                </p>
                            </div>
                        ))}
                    </div>
                )}
            </section>
        </div>
    );
}
