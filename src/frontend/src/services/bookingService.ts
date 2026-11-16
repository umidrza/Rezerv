import API_BASE_URL from "./api";

import type {
    CheckRoomAvailabilityRequest,
    CheckRoomAvailabilityResponse,
} from "../types/room";
import type { CreateBookingRequest, Booking } from "../types/booking";

export async function checkRoomAvailability(
    request: CheckRoomAvailabilityRequest
): Promise<CheckRoomAvailabilityResponse> {
    const response = await fetch(
        `${API_BASE_URL}/bookings/check-availability`,
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(request),
        }
    );

    const data = await response.json();

    if (!response.ok) {
        throw new Error(data.message || "Failed to check room availability.");
    }

    return data;
}

export async function createBooking(
    request: CreateBookingRequest,
    token: string
): Promise<Booking> {
    const response = await fetch(`${API_BASE_URL}/bookings`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(request),
    });

    const data = await response.json();

    if (!response.ok) {
        throw new Error(data.message || "Failed to submit booking.");
    }

    return data;
}

export async function getMyBookings(token: string): Promise<Booking[]> {
    const response = await fetch(`${API_BASE_URL}/bookings/my-bookings`, {
        method: "GET",
        headers: {
            Authorization: `Bearer ${token}`,
        },
    });

    if (!response.ok) {
        throw new Error("Failed to load your bookings.");
    }

    return response.json();
}