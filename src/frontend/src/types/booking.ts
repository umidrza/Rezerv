
export interface CreateBookingRequest {
    hotelId: string;
    roomId: string;
    checkIn: string;
    checkOut: string;
    numberOfGuests: number;
}

export interface Booking extends CreateBookingRequest {
    id: string;
    userId: string;
    hotelName: string;
    roomNumber: string;
    roomTypeName: string;
    pricePerNight: number;
    totalPrice: number;
    status: string | number;
    createdAt: string;
}

export interface BookingDraft extends CreateBookingRequest { }
