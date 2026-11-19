import { apiRequest } from "./api";

import type {
    Booking,
    CreateBookingRequest,
} from "../types/booking";

import type {
    CheckRoomAvailabilityRequest,
    CheckRoomAvailabilityResponse,
} from "../types/room";

export const checkRoomAvailability = async (
    request: CheckRoomAvailabilityRequest
): Promise<CheckRoomAvailabilityResponse> => {
    const params = new URLSearchParams({
        roomId: request.roomId,
        checkIn: request.checkIn,
        checkOut: request.checkOut,
    });

    return apiRequest<CheckRoomAvailabilityResponse>(
        `/bookings/check-availability?${params}`
    );
};

export const createBooking = async (
    request: CreateBookingRequest,
    token: string
): Promise<Booking> => {
    return apiRequest<Booking>(
        "/bookings",
        {
            method: "POST",
            body: JSON.stringify(request),
        },
        token
    );
};

export const getMyBookings = async (
    token: string
): Promise<Booking[]> => {
    return apiRequest<Booking[]>(
        "/bookings/my-bookings",
        {},
        token
    );
};

export const getBookingById = async (
    bookingId: string
): Promise<Booking> => {
    const token = localStorage.getItem("token");

    return apiRequest<Booking>(
        `/bookings/${bookingId}`,
        {},
        token
    );
};

export const getHotelBookings = async (
    hotelId: string,
    token: string
): Promise<Booking[]> => {
    return apiRequest<Booking[]>(
        `/bookings/hotel/${hotelId}`,
        {},
        token
    );
};

export const approveBooking = async (
    bookingId: string,
    token: string
): Promise<void> => {
    return apiRequest<void>(
        `/bookings/${bookingId}/approve`,
        { method: "PUT" },
        token
    );
};

export const rejectBooking = async (
    bookingId: string,
    token: string
): Promise<void> => {
    return apiRequest<void>(
        `/bookings/${bookingId}/reject`,
        { method: "PUT" },
        token
    );
};
