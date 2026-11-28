import { useEffect, useState } from "react";
import { Link, useLocation, useNavigate, useParams } from "react-router";
import { Heart } from "lucide-react";

import { useAuth } from "../../auth/AuthContext";

import { useHotelDetails } from "../hooks/useHotelDetails";
import { useHotelBooking } from "../hooks/useHotelBooking";
import { useHotelReviews } from "../hooks/useHotelReviews";

import {
  addFavorite,
  isFavorite as checkIsFavorite,
  removeFavorite,
} from "../../../services/hotelFavoriteService";

import HotelHeader from "../components/HotelHeader";
import HotelRoomList from "../components/HotelRoomList";
import BookingPanel from "../components/BookingPanel";
import BookingConfirmationModal from "../components/BookingConfirmationModal";
import LoadingState from "@/components/LoadingState";
import ErrorMessage from "@/components/ErrorMessage";
import HotelImageGallery from "../components/HotelImageGallery";
import HotelReviewsList from "../components/HotelReviewsList";
import HotelReviewForm from "../components/HotelReviewForm";

export default function HotelDetailsPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const location = useLocation();

  const { token, isAuthenticated, isCustomer } = useAuth();

  const { hotel, roomTypes, loading, error: hotelError } = useHotelDetails(id);

  const [isFavorite, setIsFavorite] = useState(false);
  const [favoriteLoading, setFavoriteLoading] = useState(false);
  const [favoriteError, setFavoriteError] = useState("");

  useEffect(() => {
    if (!token || !isCustomer || !id) return;

    checkIsFavorite(id, token)
      .then(setIsFavorite)
      .catch(() => setFavoriteError("Failed to load favorite status."));
  }, [id, token, isCustomer]);

  async function handleToggleFavorite() {
    if (!token || !id || favoriteLoading) return;

    setFavoriteLoading(true);
    setFavoriteError("");

    try {
      if (isFavorite) {
        await removeFavorite(id, token);
        setIsFavorite(false);
      } else {
        await addFavorite(id, token);
        setIsFavorite(true);
      }
    } catch (error) {
      setFavoriteError(
        error instanceof Error ? error.message : "Failed to update favorite.",
      );
    } finally {
      setFavoriteLoading(false);
    }
  }

  const {
    reviews,
    loading: reviewsLoading,
    error: reviewsError,
    submitting,
    submitReview,
    canReview,
    eligibilityLoading,
  } = useHotelReviews(id, token);

  const booking = useHotelBooking({
    hotel,
    token,
    isAuthenticated,

    onLoginRequired: (draft) => {
      sessionStorage.setItem("bookingDraft", JSON.stringify(draft));

      navigate("/login", {
        state: { from: location },
      });
    },

    onBookingCreated: (bookingId) => {
      navigate("/my-bookings", {
        state: {
          message: `Booking request submitted successfully. Booking ID: ${bookingId}`,
        },
      });
    },
  });

  if (loading) {
    return <LoadingState message="Loading hotel details..." />;
  }

  if (hotelError || !hotel) {
    return (
      <div className="py-20 text-center">
        <ErrorMessage message={hotelError || "Hotel not found."} />

        <Link
          to="/hotels"
          className="mt-4 inline-block text-orange-600 hover:underline"
        >
          Back to hotels
        </Link>
      </div>
    );
  }

  return (
    <>
      <main className="mx-auto max-w-7xl px-4 py-8">
        <Link
          to="/hotels"
          className="mb-6 inline-block text-sm text-orange-600 hover:underline"
        >
          ← Back to hotels
        </Link>

        <HotelImageGallery
          hotelId={hotel.id}
          primaryImageId={hotel.primaryImageId ?? null}
          primaryImageUrl={hotel.primaryImageUrl ?? null}
        />

        <HotelHeader
          hotel={hotel}
          isFavorite={isFavorite}
          onToggleFavorite={isCustomer ? handleToggleFavorite : undefined}
          favoriteLoading={favoriteLoading}
        />

        <div className="mt-8 grid grid-cols-1 gap-8 lg:grid-cols-3">
          <HotelRoomList
            roomTypes={roomTypes}
            selectedRoomId={booking.selectedRoomId}
            onRoomSelect={booking.setSelectedRoomId}
          />

          <BookingPanel
            checkIn={booking.checkIn}
            setCheckIn={booking.setCheckIn}
            checkOut={booking.checkOut}
            setCheckOut={booking.setCheckOut}
            numberOfGuests={booking.numberOfGuests}
            setNumberOfGuests={booking.setNumberOfGuests}
            availabilityLoading={booking.availabilityLoading}
            availabilityConfirmed={booking.availabilityConfirmed}
            availabilityMessage={booking.availabilityMessage}
            bookingLoading={booking.bookingLoading}
            error={booking.error}
            onCheckAvailability={booking.handleCheckAvailability}
            onRequestBooking={booking.handleBookingClick}
          />
        </div>

        <HotelReviewsList
          reviews={reviews}
          loading={reviewsLoading}
          error={reviewsError}
        />

        {isAuthenticated && isCustomer && !eligibilityLoading && canReview && (
          <HotelReviewForm onSubmit={submitReview} submitting={submitting} />
        )}
      </main>

      <BookingConfirmationModal
        isOpen={booking.showConfirmation}
        hotelName={hotel.name}
        roomId={booking.selectedRoomId ?? ""}
        checkIn={booking.checkIn}
        checkOut={booking.checkOut}
        numberOfGuests={booking.numberOfGuests}
        loading={booking.bookingLoading}
        onClose={() => booking.setShowConfirmation(false)}
        onConfirm={booking.handleConfirmBooking}
      />
    </>
  );
}
