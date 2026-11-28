
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
    request: CreateBookingRequest
): Promise<Booking> {
    return apiRequest<Booking>(
        "/bookings",
        {
            method: "POST",
            body: JSON.stringify(request),
        }
    );
}

export function getMyBookings(): Promise<Booking[]> {
    return apiRequest<Booking[]>("/bookings/my-bookings");
}

export function getBookingById(
    bookingId: string
): Promise<Booking> {
    return apiRequest<Booking>(
        `/bookings/${encodeURIComponent(bookingId)}`
    );
}

export function getHotelBookings(
    hotelId: string
): Promise<Booking[]> {
    return apiRequest<Booking[]>(
        `/bookings/hotel/${encodeURIComponent(hotelId)}`
    );
}

export function approveBooking(
    bookingId: string
): Promise<void> {
    return apiRequest<void>(
        `/bookings/${encodeURIComponent(bookingId)}/approve`,
        { method: "PUT" }
    );
}

export function rejectBooking(
    bookingId: string
): Promise<void> {
    return apiRequest<void>(
        `/bookings/${encodeURIComponent(bookingId)}/reject`,
        { method: "PUT" }
    );
}

export function cancelBooking(
    bookingId: string
): Promise<void> {
    return apiRequest<void>(
        `/bookings/${encodeURIComponent(bookingId)}/cancel`,
        { method: "PUT" }
    );
}
