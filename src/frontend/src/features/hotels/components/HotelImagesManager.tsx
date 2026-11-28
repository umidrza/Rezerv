
import { useHotelImages } from "../hooks/useHotelImages";
import API_BASE_URL from "../../../services/api";

interface HotelImagesManagerProps {
    hotelId: string;
    primaryImageId: string | null;
}

export default function HotelImagesManager({
    hotelId,
    primaryImageId,
}: HotelImagesManagerProps) {
    const {
        images,
        primaryImageId: selectedPrimaryImageId,
        loading,
        saving,
        error,
        success,
        uploadImage,
        selectPrimaryImage,
        removeImage,
    } = useHotelImages(hotelId, primaryImageId);

    const imageBaseUrl = API_BASE_URL.replace(/\/api\/?$/, "");

    function getImageUrl(imageUrl: string): string {
        if (
            imageUrl.startsWith("http://") ||
            imageUrl.startsWith("https://")
        ) {
            return imageUrl;
        }

        return `${imageBaseUrl}${imageUrl}`;
    }

    async function handleFileChange(
        event: React.ChangeEvent<HTMLInputElement>
    ) {
        const file = event.target.files?.[0];

        if (!file) return;

        if (file.size > 5 * 1024 * 1024) {
            alert("Image size cannot exceed 5 MB.");
            event.target.value = "";
            return;
        }

        await uploadImage(file);
        event.target.value = "";
    }

    async function handleDelete(imageId: string) {
        if (!window.confirm("Are you sure you want to delete this image?")) {
            return;
        }

        await removeImage(imageId);
    }

    if (loading) {
        return <p className="py-6 text-slate-500">Loading images...</p>;
    }

    return (
        <section className="rounded-xl bg-white p-6 shadow-sm">
            <div className="mb-6 flex flex-wrap items-center justify-between gap-4">
                <div>
                    <h2 className="text-xl font-semibold text-slate-900">
                        Hotel Images
                    </h2>
                    <p className="mt-1 text-sm text-slate-500">
                        Manage your hotel's image gallery.
                    </p>
                </div>

                <label className="cursor-pointer rounded-lg bg-orange-500 px-5 py-3 font-semibold text-white hover:bg-orange-600">
                    {saving ? "Processing..." : "Upload Image"}

                    <input
                        type="file"
                        accept=".jpg,.jpeg,.png,.webp"
                        onChange={handleFileChange}
                        disabled={saving}
                        className="hidden"
                    />
                </label>
            </div>

            {error && (
                <div className="mb-4 rounded-lg bg-red-50 px-4 py-3 text-sm text-red-600">
                    {error}
                </div>
            )}

            {success && (
                <div className="mb-4 rounded-lg bg-green-50 px-4 py-3 text-sm text-green-700">
                    {success}
                </div>
            )}

            {images.length === 0 ? (
                <div className="rounded-lg border border-dashed border-slate-300 p-10 text-center">
                    <p className="text-slate-500">
                        No images uploaded yet.
                    </p>
                </div>
            ) : (
                <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-3">
                    {images.map((image) => {
                        const isPrimary =
                            selectedPrimaryImageId === image.id;

                        return (
                            <div
                                key={image.id}
                                className="overflow-hidden rounded-lg border border-slate-200"
                            >
                                <img
                                    src={getImageUrl(image.imageUrl)}
                                    alt="Hotel"
                                    className="h-48 w-full object-cover"
                                />

                                <div className="space-y-3 p-4">
                                    {isPrimary && (
                                        <span className="inline-block rounded-full bg-green-100 px-3 py-1 text-xs font-semibold text-green-700">
                                            Primary Image
                                        </span>
                                    )}

                                    {!isPrimary && (
                                        <button
                                            type="button"
                                            disabled={saving}
                                            onClick={() =>
                                                selectPrimaryImage(image.id)
                                            }
                                            className="w-full rounded-lg border border-orange-500 px-3 py-2 text-sm font-semibold text-orange-600 hover:bg-orange-50 disabled:opacity-50"
                                        >
                                            Set as Primary
                                        </button>
                                    )}

                                    <button
                                        type="button"
                                        disabled={saving}
                                        onClick={() => handleDelete(image.id)}
                                        className="w-full rounded-lg bg-red-50 px-3 py-2 text-sm font-semibold text-red-600 hover:bg-red-100 disabled:opacity-50"
                                    >
                                        Delete Image
                                    </button>
                                </div>
                            </div>
                        );
                    })}
                </div>
            )}
        </section>
    );
}
