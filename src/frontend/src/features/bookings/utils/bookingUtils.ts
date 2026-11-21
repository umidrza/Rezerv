
const bookingStatuses = [
    "Draft",
    "Submitted",
    "Approved",
    "Rejected",
    "Cancelled",
];

export function getStatusLabel(status: string | number): string {
    if (typeof status === "number") {
        return bookingStatuses[status] ?? "Unknown";
    }

    return status.charAt(0).toUpperCase() + status.slice(1).toLowerCase();
}

export function getStatusStyle(status: string): string {
    switch (status.toLowerCase()) {
        case "approved":
            return "bg-green-100 text-green-700";

        case "submitted":
            return "bg-orange-100 text-orange-700";

        case "rejected":
        case "cancelled":
            return "bg-red-100 text-red-700";

        default:
            return "bg-slate-100 text-slate-700";
    }
}

export function canCancelBooking(status: string | number): boolean {
    return ["Submitted", "Approved"].includes(getStatusLabel(status));
}

export function formatBookingDate(date: string): string {
    return new Date(date).toLocaleDateString();
}

export function calculateNights(checkIn: string, checkOut: string): number {
    const start = new Date(checkIn);
    const end = new Date(checkOut);

    return Math.max(
        0,
        Math.ceil((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24))
    );
}
