export interface CreateBookingRequest {
    hotelId: string;
    roomId: string;
    checkIn: string;
    checkOut: string;
    numberOfGuests: number;
}

export interface Booking {
    id: string;
    userId: string;
    hotelId: string;
    hotelName: string;
    roomId: string;
    roomNumber: string;
    roomTypeName: string;
    checkIn: string;
    checkOut: string;
    numberOfGuests: number;
    pricePerNight: number;
    totalPrice: number;
    status: string | number;
    createdAt: string;
}

export interface BookingDraft {
    hotelId: string;
    roomId: string;
    checkIn: string;
    checkOut: string;
    numberOfGuests: number;
}