export interface HotelReview {
    id: string;
    hotelId: string;
    userId: string;
    userName: string;
    rating: number;
    comment: string;
    createdAt: string;
}

export interface CreateHotelReviewRequest {
    rating: number;
    comment: string;
}