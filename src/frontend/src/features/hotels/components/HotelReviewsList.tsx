
import type { HotelReview } from "../../../types/hotelReview";
import LoadingState from "../../../components/LoadingState";
import ErrorMessage from "../../../components/ErrorMessage";

interface HotelReviewsListProps {
    reviews: HotelReview[];
    loading: boolean;
    error: string | null;
}

export default function HotelReviewsList({
    reviews,
    loading,
    error,
}: HotelReviewsListProps) {
    if (loading) {
        return <LoadingState message="Loading reviews..." />;
    }

    if (error) {
        return <ErrorMessage message={error} />;
    }

    return (
        <section className="mt-10">
            <h2 className="mb-6 text-2xl font-bold text-slate-900">
                Guest Reviews ({reviews.length})
            </h2>

            {reviews.length === 0 ? (
                <p className="text-slate-500">
                    No reviews yet. Be the first to review this hotel!
                </p>
            ) : (
                <div className="space-y-4">
                    {reviews.map((review) => (
                        <article
                            key={review.id}
                            className="rounded-xl border border-slate-200 bg-white p-5"
                        >
                            <div className="flex flex-wrap items-center justify-between gap-2">
                                <h3 className="font-semibold text-slate-900">
                                    {review.userName}
                                </h3>

                                <span className="text-sm text-slate-500">
                                    {new Date(review.createdAt).toLocaleDateString()}
                                </span>
                            </div>

                            <div className="mt-2 text-yellow-500">
                                {"★".repeat(review.rating)}
                                <span className="text-slate-300">
                                    {"★".repeat(5 - review.rating)}
                                </span>
                            </div>

                            <p className="mt-3 leading-7 text-slate-700">
                                {review.comment}
                            </p>
                        </article>
                    ))}
                </div>
            )}
        </section>
    );
}
