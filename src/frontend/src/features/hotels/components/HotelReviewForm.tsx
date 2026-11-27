import { useState } from "react";
import type { CreateHotelReviewRequest } from "../../../types/hotelReview";

interface HotelReviewFormProps {
  onSubmit: (data: CreateHotelReviewRequest) => Promise<void>;
  submitting: boolean;
}

export default function HotelReviewForm({
  onSubmit,
  submitting,
}: HotelReviewFormProps) {
  const [rating, setRating] = useState(5);
  const [comment, setComment] = useState("");
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setError(null);

    if (!comment.trim()) {
      setError("Please write a review.");
      return;
    }

    try {
      await onSubmit({
        rating,
        comment: comment.trim(),
      });

      setRating(5);
      setComment("");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to submit review.");
    }
  };

  return (
    <section className="mt-10 rounded-xl border border-slate-200 bg-white p-6">
      <h2 className="text-xl font-bold text-slate-900">Write a Review</h2>

      <form onSubmit={handleSubmit} className="mt-5 space-y-5">
        <div>
          <label className="mb-2 block text-sm font-medium text-slate-700">
            Your Rating
          </label>

          <div className="flex gap-2">
            {[1, 2, 3, 4, 5].map((value) => (
              <button
                key={value}
                type="button"
                onClick={() => setRating(value)}
                className={`text-3xl ${
                  value <= rating ? "text-yellow-500" : "text-slate-300"
                }`}
                aria-label={`Rate ${value} out of 5`}
              >
                ★
              </button>
            ))}
          </div>
        </div>

        <div>
          <label
            htmlFor="review-comment"
            className="mb-2 block text-sm font-medium text-slate-700"
          >
            Your Review
          </label>

          <textarea
            id="review-comment"
            value={comment}
            onChange={(e) => setComment(e.target.value)}
            maxLength={1000}
            rows={4}
            placeholder="Share your experience..."
            className="w-full rounded-lg border border-slate-300 p-3 outline-none focus:border-orange-500 focus:ring-2 focus:ring-orange-100"
          />

          <p className="mt-1 text-right text-xs text-slate-500">
            {comment.length}/1000
          </p>
        </div>

        {error && <p className="text-sm text-red-600">{error}</p>}

        <button
          type="submit"
          disabled={submitting}
          className="rounded-lg bg-orange-600 px-5 py-2.5 font-medium text-white transition hover:bg-orange-700 disabled:cursor-not-allowed disabled:opacity-50"
        >
          {submitting ? "Submitting..." : "Submit Review"}
        </button>
      </form>
    </section>
  );
}
