import { useRef, useState } from "react";

import { useAuth } from "../../auth/AuthContext";
import { useMyHotels } from "../hooks/useMyHotels";

import MyHotelCard from "../components/MyHotelCard";
import EditHotelModal from "../components/EditHotelModal";
import HotelImagesManager from "../components/HotelImagesManager";

import ErrorMessage from "../../../components/ErrorMessage";
import LoadingState from "../../../components/LoadingState";


import type { Hotel, UpdateHotelRequest } from "../../../types/hotel";

export default function MyHotelsPage() {
    const { token } = useAuth();

    const {
        hotels,
        loading,
        saving,
        error,
        success,
        setError,
        setSuccess,
        updateHotelDetails,
        toggleHotelStatus,
    } = useMyHotels(token);

    const [selectedHotel, setSelectedHotel] = useState<Hotel | null>(null);
    const [imageHotel, setImageHotel] = useState<Hotel | null>(null);

    const imagesSectionRef = useRef<HTMLDivElement>(null);

    async function handleSave(
        hotelId: string,
        form: UpdateHotelRequest
    ): Promise<void> {
        await updateHotelDetails(hotelId, form);
        setSelectedHotel(null);
    }

    function handleEdit(hotel: Hotel) {
        setError("");
        setSuccess("");
        setSelectedHotel(hotel);
    }

    function handleManageImages(hotel: Hotel) {
        setError("");
        setSuccess("");
        setImageHotel(hotel);

        setTimeout(() => {
            imagesSectionRef.current?.scrollIntoView({
                behavior: "smooth",
                block: "start",
            });
        }, 100);
    }

    if (loading) {
        return <LoadingState message="Loading your hotels..." />;
    }

    return (
        <div className="mx-auto max-w-7xl px-4 py-10">
            <div className="mb-8">
                <h1 className="text-3xl font-bold text-slate-900">
                    My Hotels
                </h1>

                <p className="mt-2 text-slate-600">
                    Manage your hotels and update their information.
                </p>
            </div>

            {error && (
                <div className="mb-4">
                    <ErrorMessage message={error} />
                </div>
            )}

            {success && (
                <div className="mb-4 rounded-lg bg-green-50 px-4 py-3 text-sm text-green-700">
                    {success}
                </div>
            )}

            {hotels.length === 0 ? (
                <div className="rounded-xl bg-white p-10 text-center shadow">
                    <p className="text-slate-600">
                        You don't manage any hotels yet.
                    </p>
                </div>
            ) : (
                <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
                    {hotels.map((hotel) => (
                        <div key={hotel.id}>
                            <MyHotelCard
                                hotel={hotel}
                                onEdit={handleEdit}
                                onToggleStatus={toggleHotelStatus}
                            />

                            <button
                                type="button"
                                onClick={() => handleManageImages(hotel)}
                                className="mt-3 w-full rounded-lg bg-slate-800 px-4 py-3 font-semibold text-white hover:bg-slate-700"
                            >
                                Manage Images
                            </button>
                        </div>
                    ))}
                </div>
            )}

            {imageHotel && (
                <div ref={imagesSectionRef}
                    className="mt-10 scroll-mt-8">
                    <div className="mb-4 flex items-center justify-between">
                        <h2 className="text-2xl font-bold text-slate-900">
                            Images — {imageHotel.name}
                        </h2>

                        <button
                            type="button"
                            onClick={() => setImageHotel(null)}
                            className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-600 hover:bg-slate-100"
                        >
                            Close
                        </button>
                    </div>

                    <HotelImagesManager
                        hotelId={imageHotel.id}
                        token={token}
                        primaryImageId={imageHotel.primaryImageId}
                    />
                </div>
            )}

            <EditHotelModal
                hotel={selectedHotel}
                saving={saving}
                onClose={() => setSelectedHotel(null)}
                onSave={handleSave}
            />
        </div>
    );
}