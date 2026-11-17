import API_BASE_URL from "./api";
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

    const response = await fetch(
        `${API_BASE_URL}/bookings/check-availability?${params}`
    );

    if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || "Failed to check room availability.");
    }

    return response.json();
};

export const createBooking = async (
    request: CreateBookingRequest,
    token: string
): Promise<Booking> => {
    const response = await fetch(`${API_BASE_URL}/bookings`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(request),
    });

    if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || "Failed to create booking.");
    }

    return response.json();
};

export const getMyBookings = async (
    token: string
): Promise<Booking[]> => {
    const response = await fetch(`${API_BASE_URL}/bookings/my-bookings`, {
        headers: {
            Authorization: `Bearer ${token}`,
        },
    });

    if (!response.ok) {
        throw new Error("Failed to load bookings.");
    }

    return response.json();
};

export const getBookingById = async (
    bookingId: string
): Promise<Booking> => {
    const token = localStorage.getItem("token");

    const response = await fetch(
        `${API_BASE_URL}/bookings/${bookingId}`,
        {
            headers: {
                Authorization: `Bearer ${token}`,
            },
        }
    );

    if (!response.ok) {
        throw new Error("Failed to load booking details.");
    }

    return response.json();
};

export const getHotelBookings = async (
    hotelId: string,
    token: string
): Promise<Booking[]> => {
    const response = await fetch(
        `${API_BASE_URL}/bookings/hotel/${hotelId}`,
        {
            headers: {
                Authorization: `Bearer ${token}`,
            },
        }
    );

    if (!response.ok) {
        throw new Error("Failed to load hotel bookings.");
    }

    return response.json();
};

export const approveBooking = async (
    bookingId: string,
    token: string
): Promise<void> => {
    const response = await fetch(
        `${API_BASE_URL}/bookings/${bookingId}/approve`,
        {
            method: "PUT",
            headers: {
                Authorization: `Bearer ${token}`,
            },
        }
    );

    if (!response.ok) {
        throw new Error("Failed to approve booking.");
    }
};

export const rejectBooking = async (
    bookingId: string,
    token: string
): Promise<void> => {
    const response = await fetch(
        `${API_BASE_URL}/bookings/${bookingId}/reject`,
        {
            method: "PUT",
            headers: {
                Authorization: `Bearer ${token}`,
            },
        }
    );

    if (!response.ok) {
        throw new Error("Failed to reject booking.");
    }
};