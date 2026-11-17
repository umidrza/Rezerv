
import { useEffect, useState } from "react";
import { Link } from "react-router";
import {
    getMyHotels,
    updateHotel,
    activateHotel,
    deactivateHotel,
} from "../../../services/hotelService";
import type { UpdateHotelRequest } from "../../../services/hotelService";
import type { Hotel } from "../../../types/hotel";
import { useAuth } from "../../auth/AuthContext";

const emptyForm: UpdateHotelRequest = {
    name: "",
    description: "",
    address: "",
    city: "",
    country: "",
    starRating: 3,
    primaryImageUrl: "",
};

export default function MyHotelsPage() {
    const { token } = useAuth();

    const [hotels, setHotels] = useState<Hotel[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");
    const [editingHotelId, setEditingHotelId] = useState<string | null>(null);
    const [form, setForm] = useState<UpdateHotelRequest>(emptyForm);
    const [saving, setSaving] = useState(false);

    async function loadHotels() {
        if (!token) {
            setError("Please log in to continue.");
            setLoading(false);
            return;
        }

        try {
            const result = await getMyHotels(token);
            setHotels(result);
        } catch (err) {
            setError(err instanceof Error ? err.message : "Failed to load hotels.");
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        loadHotels();
    }, [token]);

    function startEditing(hotel: Hotel) {
        setEditingHotelId(hotel.id);
        setForm({
            name: hotel.name,
            description: hotel.description,
            address: hotel.address,
            city: hotel.city,
            country: hotel.country,
            starRating: hotel.starRating,
            primaryImageUrl: hotel.primaryImageUrl ?? "",
        });
        setError("");
        setSuccess("");
    }

    function handleInputChange(
        e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>
    ) {
        const { name, value } = e.target;

        setForm((current) => ({
            ...current,
            [name]: name === "starRating" ? Number(value) : value,
        }));
    }

    async function handleUpdate(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();

        if (!token || !editingHotelId) return;

        setSaving(true);
        setError("");
        setSuccess("");

        try {
            const updatedHotel = await updateHotel(
                editingHotelId,
                form,
                token
            );

            setHotels((current) =>
                current.map((hotel) =>
                    hotel.id === editingHotelId ? updatedHotel : hotel
                )
            );

            setEditingHotelId(null);
            setSuccess("Hotel updated successfully.");
        } catch (err) {
            setError(err instanceof Error ? err.message : "Failed to update hotel.");
        } finally {
            setSaving(false);
        }
    }

    async function handleStatusChange(hotel: Hotel) {
        if (!token) return;

        const action = hotel.isActive ? "deactivate" : "activate";

        if (!window.confirm(`Are you sure you want to ${action} this hotel?`)) {
            return;
        }

        setError("");
        setSuccess("");

        try {
            if (hotel.isActive) {
                await deactivateHotel(hotel.id, token);
            } else {
                await activateHotel(hotel.id, token);
            }

            setHotels((current) =>
                current.map((item) =>
                    item.id === hotel.id
                        ? { ...item, isActive: !item.isActive }
                        : item
                )
            );

            setSuccess(`Hotel ${action}d successfully.`);
        } catch (err) {
            setError(err instanceof Error ? err.message : "Failed to change hotel status.");
        }
    }

    if (loading) {
        return <p className="p-6">Loading your hotels...</p>;
    }

    return (
        <div className="min-h-screen bg-slate-50 px-6 py-10">
            <div className="mx-auto max-w-6xl">
                <h1 className="text-3xl font-bold text-slate-900">
                    My Hotels
                </h1>

                <p className="mt-2 text-slate-500">
                    Manage your hotels and booking requests.
                </p>

                {error && (
                    <p className="mt-6 rounded-lg bg-red-100 p-4 text-red-700">
                        {error}
                    </p>
                )}

                {success && (
                    <p className="mt-6 rounded-lg bg-green-100 p-4 text-green-700">
                        {success}
                    </p>
                )}

                {!error && hotels.length === 0 && (
                    <p className="mt-8 rounded-xl bg-white p-6 text-slate-500 shadow-sm">
                        No hotels assigned to you yet.
                    </p>
                )}

                <div className="mt-8 grid gap-6 md:grid-cols-2 lg:grid-cols-3">
                    {hotels.map((hotel) => (
                        <div
                            key={hotel.id}
                            className="overflow-hidden rounded-xl bg-white shadow-sm"
                        >
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

                                    <span className={`rounded-full px-3 py-1 text-xs font-semibold ${
                                        hotel.isActive
                                            ? "bg-green-100 text-green-700"
                                            : "bg-red-100 text-red-700"
                                    }`}>
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
                                    onClick={() => startEditing(hotel)}
                                    className="mt-5 w-full rounded-lg border border-orange-500 px-4 py-3 font-semibold text-orange-600 hover:bg-orange-50"
                                >
                                    Edit Hotel
                                </button>

                                <button
                                    onClick={() => handleStatusChange(hotel)}
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
                        </div>
                    ))}
                </div>

                {editingHotelId && (
                    <div className="fixed inset-0 z-50 flex items-center justify-center overflow-y-auto bg-black/50 p-4">
                        <form
                            onSubmit={handleUpdate}
                            className="my-8 w-full max-w-2xl space-y-4 rounded-xl bg-white p-6 shadow-xl"
                        >
                            <h2 className="text-2xl font-bold text-slate-900">
                                Edit Hotel
                            </h2>

                            <div>
                                <label className="mb-1 block text-sm font-medium">
                                    Hotel Name
                                </label>
                                <input
                                    name="name"
                                    value={form.name}
                                    onChange={handleInputChange}
                                    required
                                    className="w-full rounded-lg border p-3"
                                />
                            </div>

                            <div>
                                <label className="mb-1 block text-sm font-medium">
                                    Description
                                </label>
                                <textarea
                                    name="description"
                                    value={form.description}
                                    onChange={handleInputChange}
                                    rows={3}
                                    className="w-full rounded-lg border p-3"
                                />
                            </div>

                            <div>
                                <label className="mb-1 block text-sm font-medium">
                                    Address
                                </label>
                                <input
                                    name="address"
                                    value={form.address}
                                    onChange={handleInputChange}
                                    required
                                    className="w-full rounded-lg border p-3"
                                />
                            </div>

                            <div className="grid gap-4 sm:grid-cols-2">
                                <div>
                                    <label className="mb-1 block text-sm font-medium">
                                        City
                                    </label>
                                    <input
                                        name="city"
                                        value={form.city}
                                        onChange={handleInputChange}
                                        required
                                        className="w-full rounded-lg border p-3"
                                    />
                                </div>

                                <div>
                                    <label className="mb-1 block text-sm font-medium">
                                        Country
                                    </label>
                                    <input
                                        name="country"
                                        value={form.country}
                                        onChange={handleInputChange}
                                        required
                                        className="w-full rounded-lg border p-3"
                                    />
                                </div>
                            </div>

                            <div>
                                <label className="mb-1 block text-sm font-medium">
                                    Star Rating
                                </label>
                                <select
                                    name="starRating"
                                    value={form.starRating}
                                    onChange={handleInputChange}
                                    className="w-full rounded-lg border p-3"
                                >
                                    {[1, 2, 3, 4, 5].map((rating) => (
                                        <option key={rating} value={rating}>
                                            {rating} Stars
                                        </option>
                                    ))}
                                </select>
                            </div>

                            <div>
                                <label className="mb-1 block text-sm font-medium">
                                    Primary Image URL
                                </label>
                                <input
                                    name="primaryImageUrl"
                                    type="url"
                                    value={form.primaryImageUrl ?? ""}
                                    onChange={handleInputChange}
                                    className="w-full rounded-lg border p-3"
                                    placeholder="https://example.com/hotel.jpg"
                                />
                            </div>

                            <div className="flex gap-3 pt-2">
                                <button
                                    type="submit"
                                    disabled={saving}
                                    className="flex-1 rounded-lg bg-orange-500 px-4 py-3 font-semibold text-white hover:bg-orange-600 disabled:opacity-50"
                                >
                                    {saving ? "Saving..." : "Save Changes"}
                                </button>

                                <button
                                    type="button"
                                    onClick={() => setEditingHotelId(null)}
                                    disabled={saving}
                                    className="flex-1 rounded-lg border px-4 py-3 font-semibold text-slate-700 hover:bg-slate-50"
                                >
                                    Cancel
                                </button>
                            </div>
                        </form>
                    </div>
                )}
            </div>
        </div>
    );
}
