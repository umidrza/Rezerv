export interface CreateBookingRequest {
    hotelId: string;
    checkIn: string;
    checkOut: string;
    numberOfGuests: number;
    roomIds: string[];
}

export interface Booking {
    id: string;
    hotelId: string;
    checkIn: string;
    checkOut: string;
    numberOfGuests: number;
    totalPrice: number;
    status: string | number;
}