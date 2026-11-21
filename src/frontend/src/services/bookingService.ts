
import { apiRequest } from "./api";

import type {
    Booking,
    CreateBookingRequest,
} from "../types/booking";

import type {
    CheckRoomAvailabilityRequest,
    CheckRoomAvailabilityResponse,
} from "../types/room";

export function checkRoomAvailability(
    request: CheckRoomAvailabilityRequest
): Promise<CheckRoomAvailabilityResponse> {
    const params = new URLSearchParams({
        roomId: request.roomId,
        checkIn: request.checkIn,
        checkOut: request.checkOut,
    });

    return apiRequest<CheckRoomAvailabilityResponse>(
        `/bookings/check-availability?${params.toString()}`
    );
}

export function createBooking(
    request: CreateBookingRequest,
    token: string
): Promise<Booking> {
    return apiRequest<Booking>(
        "/bookings",
        {
            method: "POST",
            body: JSON.stringify(request),
        },
        token
    );
}

export function getMyBookings(token: string): Promise<Booking[]> {
    return apiRequest<Booking[]>(
        "/bookings/my-bookings",
        {},
        token
    );
}

export function getBookingById(
    bookingId: string,
    token: string
): Promise<Booking> {
    return apiRequest<Booking>(
        `/bookings/${encodeURIComponent(bookingId)}`,
        {},
        token
    );
}

export function getHotelBookings(
    hotelId: string,
    token: string
): Promise<Booking[]> {
    return apiRequest<Booking[]>(
        `/bookings/hotel/${encodeURIComponent(hotelId)}`,
        {},
        token
    );
}

export function approveBooking(
    bookingId: string,
    token: string
): Promise<void> {
    return apiRequest<void>(
        `/bookings/${encodeURIComponent(bookingId)}/approve`,
        { method: "PUT" },
        token
    );
}

export function rejectBooking(
    bookingId: string,
    token: string
): Promise<void> {
    return apiRequest<void>(
        `/bookings/${encodeURIComponent(bookingId)}/reject`,
        { method: "PUT" },
        token
    );
}

export function cancelBooking(
    bookingId: string,
    token: string
): Promise<void> {
    return apiRequest<void>(
        `/bookings/${encodeURIComponent(bookingId)}/cancel`,
        { method: "PUT" },
        token
    );
}
