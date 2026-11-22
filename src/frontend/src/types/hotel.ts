
export interface Hotel {
  id: string;
  name: string;
  description: string;
  address: string;
  city: string;
  country: string;
  starRating: number;
  primaryImageId: string | null;
  primaryImageUrl: string | null;
  isActive: boolean;
}

export interface SearchHotelsParams {
  city?: string;
  starRating?: number;
  checkIn?: string;
  checkOut?: string;
  numberOfGuests?: number;
  page?: number;
  pageSize?: number;
}

export interface PaginatedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface UpdateHotelRequest {
  name: string;
  description: string;
  address: string;
  city: string;
  country: string;
  starRating: number;
  primaryImageUrl?: string;
}

export interface HotelImage {
  id: string;
  hotelId: string;
  imageUrl: string;
}

export interface CreateHotelImageRequest {
  hotelId: string;
  file: File;
}