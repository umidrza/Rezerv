import { apiRequest } from "./api";
import type {
  HotelReview,
  CreateHotelReviewRequest,
} from "../types/hotelReview";

export function getHotelReviews(hotelId: string): Promise<HotelReview[]> {
  return apiRequest<HotelReview[]>(`/hotels/${hotelId}/reviews`);
}

export function createHotelReview(
  hotelId: string,
  data: CreateHotelReviewRequest
): Promise<HotelReview> {
  return apiRequest<HotelReview>(
    `/hotels/${hotelId}/reviews`,
    {
      method: "POST",
      body: JSON.stringify(data),
    }
  );
}

export function canReviewHotel(hotelId: string): Promise<boolean> {
  return apiRequest<boolean>(
    `/hotels/${hotelId}/reviews/eligibility`
  );
}
