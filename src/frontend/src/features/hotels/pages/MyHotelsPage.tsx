
import { useState } from "react";

import { useAuth } from "../../auth/AuthContext";
import { useMyHotels } from "../hooks/useMyHotels";

import MyHotelCard from "../components/MyHotelCard";
import EditHotelModal from "../components/EditHotelModal";

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

    function handleCloseModal() {
        setSelectedHotel(null);
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
                        <MyHotelCard
                            key={hotel.id}
                            hotel={hotel}
                            onEdit={handleEdit}
                            onToggleStatus={toggleHotelStatus}
                        />
                    ))}
                </div>
            )}

            <EditHotelModal
                hotel={selectedHotel}
                saving={saving}
                onClose={handleCloseModal}
                onSave={handleSave}
            />
        </div>
    );
}
