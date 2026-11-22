
import { useEffect, useState } from "react";

import { getHotelImages } from "../../../services/hotelImageService";
import API_BASE_URL from "../../../services/api";

import type { HotelImage } from "../../../types/hotel";

interface HotelImageGalleryProps {
    hotelId: string;
    primaryImageId: string | null;
    primaryImageUrl: string | null;
}

interface GalleryImage {
    id: string;
    url: string;
}

export default function HotelImageGallery({
    hotelId,
    primaryImageId,
    primaryImageUrl,
}: HotelImageGalleryProps) {
    const [images, setImages] = useState<GalleryImage[]>([]);
    const [currentIndex, setCurrentIndex] = useState(0);
    const [loading, setLoading] = useState(true);

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

    useEffect(() => {
        async function loadImages() {
            try {
                setLoading(true);

                const result: HotelImage[] = await getHotelImages(hotelId);

                const galleryImages = result.map((image) => ({
                    id: image.id,
                    url: getImageUrl(image.imageUrl),
                }));

                if (
                    primaryImageUrl &&
                    !result.some((image) => image.id === primaryImageId)
                ) {
                    galleryImages.unshift({
                        id: "primary",
                        url: getImageUrl(primaryImageUrl),
                    });
                }

                const primaryIndex = galleryImages.findIndex(
                    (image) => image.id === primaryImageId
                );

                setImages(galleryImages);
                setCurrentIndex(primaryIndex >= 0 ? primaryIndex : 0);
            } catch {
                setImages(
                    primaryImageUrl
                        ? [{ id: "primary", url: getImageUrl(primaryImageUrl) }]
                        : []
                );
                setCurrentIndex(0);
            } finally {
                setLoading(false);
            }
        }

        loadImages();
    }, [hotelId, primaryImageId, primaryImageUrl]);

    function showPrevious() {
        setCurrentIndex((current) =>
            current === 0 ? images.length - 1 : current - 1
        );
    }

    function showNext() {
        setCurrentIndex((current) =>
            current === images.length - 1 ? 0 : current + 1
        );
    }

    if (loading) {
        return (
            <div className="h-96 animate-pulse rounded-xl bg-slate-200" />
        );
    }

    if (images.length === 0) {
        return (
            <div className="flex h-96 items-center justify-center rounded-xl bg-slate-100 text-slate-500">
                No images available
            </div>
        );
    }

    return (
        <div className="space-y-4">
            <div className="group relative overflow-hidden rounded-xl bg-slate-200">
                <img
                    src={images[currentIndex].url}
                    alt="Hotel"
                    className="h-96 w-full object-cover"
                />

                {images.length > 1 && (
                    <>
                        <button
                            type="button"
                            onClick={showPrevious}
                            aria-label="Previous image"
                            className="absolute left-4 top-1/2 -translate-y-1/2 rounded-full bg-black/50 p-3 text-white opacity-0 transition hover:bg-black/70 group-hover:opacity-100"
                        >
                            &#10094;
                        </button>

                        <button
                            type="button"
                            onClick={showNext}
                            aria-label="Next image"
                            className="absolute right-4 top-1/2 -translate-y-1/2 rounded-full bg-black/50 p-3 text-white opacity-0 transition hover:bg-black/70 group-hover:opacity-100"
                        >
                            &#10095;
                        </button>

                        <div className="absolute bottom-4 right-4 rounded-full bg-black/60 px-3 py-1 text-sm text-white">
                            {currentIndex + 1} / {images.length}
                        </div>
                    </>
                )}
            </div>

            {images.length > 1 && (
                <div className="flex justify-center gap-2">
                    {images.map((image, index) => (
                        <button
                            key={image.id}
                            type="button"
                            onClick={() => setCurrentIndex(index)}
                            aria-label={`Go to image ${index + 1}`}
                            className={`h-2.5 rounded-full transition-all ${
                                currentIndex === index
                                    ? "w-7 bg-orange-500"
                                    : "w-2.5 bg-slate-300 hover:bg-slate-400"
                            }`}
                        />
                    ))}
                </div>
            )}
        </div>
    );
}
