import type { Booking } from "./booking";

export interface Dashboard {
    totalBookings: number;
    pendingBookings: number;
    approvedBookings: number;
    totalHotels: number;
    activeHotels: number;
    totalUsers: number;
    recentBookings: Booking[];
}