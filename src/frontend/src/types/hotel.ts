export interface Hotel {
  id: string;
  name: string;
  description: string;
  address: string;
  city: string;
  country: string;
  starRating: number;
  primaryImageUrl: string | null;
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