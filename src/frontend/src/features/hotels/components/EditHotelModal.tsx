import { useEffect, useState } from "react";
import type { Hotel, UpdateHotelRequest } from "../../../types/hotel";

interface EditHotelModalProps {
    hotel: Hotel | null;
    saving: boolean;
    onClose: () => void;
    onSave: (hotelId: string, form: UpdateHotelRequest) => Promise<void>;
}

export default function EditHotelModal({
    hotel,
    saving,
    onClose,
    onSave,
}: EditHotelModalProps) {
    const [form, setForm] = useState<UpdateHotelRequest>({
        name: "",
        description: "",
        address: "",
        city: "",
        country: "",
        starRating: 3,
        primaryImageUrl: "",
    });

    useEffect(() => {
        if (!hotel) return;

        setForm({
            name: hotel.name,
            description: hotel.description,
            address: hotel.address,
            city: hotel.city,
            country: hotel.country,
            starRating: hotel.starRating,
            primaryImageUrl: hotel.primaryImageUrl ?? "",
        });
    }, [hotel]);

    if (!hotel) return null;

    function handleChange(
        event: React.ChangeEvent<
            HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
        >
    ) {
        const { name, value } = event.target;

        setForm((current) => ({
            ...current,
            [name]: name === "starRating" ? Number(value) : value,
        }));
    }

    async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
        event.preventDefault();
        if (!hotel) return null;
        
        await onSave(hotel.id, form);
    }

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center overflow-y-auto bg-black/50 p-4">
            <form
                onSubmit={handleSubmit}
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
                        onChange={handleChange}
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
                        onChange={handleChange}
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
                        onChange={handleChange}
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
                            onChange={handleChange}
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
                            onChange={handleChange}
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
                        onChange={handleChange}
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
                        onChange={handleChange}
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
                        onClick={onClose}
                        disabled={saving}
                        className="flex-1 rounded-lg border px-4 py-3 font-semibold text-slate-700 hover:bg-slate-50"
                    >
                        Cancel
                    </button>
                </div>
            </form>
        </div>
    );
}
