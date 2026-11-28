
import { useEffect, useState } from "react";
import { useAuth } from "../features/auth/AuthContext";

import {
    getAdminDashboard,
    getManagerDashboard,
    getCustomerDashboard,
} from "../services/dashboardService";

import type { Dashboard } from "../types/dashboard";

export default function DashboardPage() {
    const { isAdmin, isHotelManager, isCustomer } = useAuth();

    const [dashboard, setDashboard] = useState<Dashboard | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        async function loadDashboard() {
            try {
                setLoading(true);
                setError("");

                let data: Dashboard;

                if (isAdmin) {
                    data = await getAdminDashboard();
                } else if (isHotelManager) {
                    data = await getManagerDashboard();
                } else if (isCustomer) {
                    data = await getCustomerDashboard();
                } else {
                    throw new Error("Unauthorized dashboard access.");
                }

                setDashboard(data);
            } catch {
                setError("Failed to load dashboard.");
            } finally {
                setLoading(false);
            }
        }

        loadDashboard();
    }, [isAdmin, isHotelManager, isCustomer]);

    if (loading) {
        return (
            <div
                role="status"
                aria-live="polite"
                className="mx-auto max-w-7xl px-4 py-12 sm:px-6 lg:px-8"
            >
                <p className="text-slate-500">Loading dashboard...</p>
            </div>
        );
    }

    if (error || !dashboard) {
        return (
            <div
                role="alert"
                aria-live="assertive"
                className="mx-auto max-w-7xl px-4 py-12 sm:px-6 lg:px-8"
            >
                <p className="rounded-xl border border-red-200 bg-red-50 p-4 text-sm text-red-600">
                    {error || "Dashboard data unavailable."}
                </p>
            </div>
        );
    }

    const statistics = [
        {
            label: "Total Bookings",
            value: dashboard.totalBookings,
            color: "bg-blue-50 text-blue-600",
            icon: "B",
        },
        {
            label: "Pending Bookings",
            value: dashboard.pendingBookings,
            color: "bg-amber-50 text-amber-600",
            icon: "P",
        },
        {
            label: "Approved Bookings",
            value: dashboard.approvedBookings,
            color: "bg-emerald-50 text-emerald-600",
            icon: "A",
        },
        ...(isCustomer ? [] : [
            {
                label: "Total Hotels",
                value: dashboard.totalHotels,
                color: "bg-purple-50 text-purple-600",
                icon: "H",
            },
            {
                label: "Active Hotels",
                value: dashboard.activeHotels,
                color: "bg-teal-50 text-teal-600",
                icon: "H",
            },
        ]),
        ...(isAdmin ? [
            {
                label: "Total Users",
                value: dashboard.totalUsers,
                color: "bg-rose-50 text-rose-600",
                icon: "U",
            },
        ] : []),
    ];

    function getStatusStyle(status: string | number) {
        const value = String(status).toLowerCase();

        if (value === "approved" || value === "1") {
            return "bg-emerald-50 text-emerald-700";
        }

        if (value === "submitted" || value === "0") {
            return "bg-amber-50 text-amber-700";
        }

        if (value === "rejected" || value === "2") {
            return "bg-red-50 text-red-700";
        }

        return "bg-slate-100 text-slate-700";
    }

    return (
        <div className="min-h-screen bg-slate-50">
            <div className="mx-auto max-w-7xl space-y-8 px-4 py-8 sm:px-6 lg:px-8 lg:py-10">

                {/* Header */}
                <div>
                    <p className="text-sm font-medium text-orange-500">
                        Overview
                    </p>
                    <h1 className="mt-1 text-3xl font-bold tracking-tight text-slate-900">
                        Dashboard
                    </h1>
                    <p className="mt-2 text-sm text-slate-500">
                        Overview of your account activity and recent bookings.
                    </p>
                </div>

                {/* Statistics */}
                <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-3">
                    {statistics.map((stat) => (
                        <div
                            key={stat.label}
                            className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm transition-shadow hover:shadow-md"
                        >
                            <div className="flex items-center justify-between">
                                <p className="text-sm font-medium text-slate-500">
                                    {stat.label}
                                </p>
                                <span
                                    className={`flex h-10 w-10 items-center justify-center rounded-xl text-sm font-bold ${stat.color}`}
                                >
                                    {stat.icon}
                                </span>
                            </div>

                            <p className="mt-4 text-3xl font-bold tracking-tight text-slate-900">
                                {stat.value.toLocaleString()}
                            </p>
                        </div>
                    ))}
                </div>

                {/* Recent Bookings */}
                <section className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
                    <div className="flex items-center justify-between border-b border-slate-100 px-6 py-5">
                        <div>
                            <h2 className="text-lg font-semibold text-slate-900">
                                Recent Bookings
                            </h2>
                            <p className="mt-1 text-sm text-slate-500">
                                Latest booking activity
                            </p>
                        </div>

                        <span className="rounded-full bg-orange-50 px-3 py-1 text-xs font-semibold text-orange-600">
                            {dashboard.recentBookings.length} bookings
                        </span>
                    </div>

                    {dashboard.recentBookings.length === 0 ? (
                        <div className="px-6 py-16 text-center">
                            <p className="font-medium text-slate-700">
                                No bookings yet
                            </p>
                            <p className="mt-1 text-sm text-slate-500">
                                Recent bookings will appear here.
                            </p>
                        </div>
                    ) : (
                        <div className="overflow-x-auto">
                            <table className="w-full text-left text-sm">
                                <thead className="bg-slate-50 text-xs uppercase tracking-wider text-slate-500">
                                    <tr>
                                        <th scope="col" className="px-6 py-4 font-semibold">Hotel</th>
                                        <th scope="col" className="px-6 py-4 font-semibold">Room</th>
                                        <th scope="col" className="px-6 py-4 font-semibold">Check-in</th>
                                        <th scope="col" className="px-6 py-4 font-semibold">Check-out</th>
                                        <th scope="col" className="px-6 py-4 font-semibold">Total</th>
                                        <th scope="col" className="px-6 py-4 font-semibold">Status</th>
                                    </tr>
                                </thead>

                                <tbody className="divide-y divide-slate-100">
                                    {dashboard.recentBookings.map((booking) => (
                                        <tr
                                            key={booking.id}
                                            className="transition-colors hover:bg-slate-50"
                                        >
                                            <td className="whitespace-nowrap px-6 py-4 font-medium text-slate-900">
                                                {booking.hotelName}
                                            </td>
                                            <td className="whitespace-nowrap px-6 py-4 text-slate-600">
                                                {booking.roomNumber}
                                            </td>
                                            <td className="whitespace-nowrap px-6 py-4 text-slate-600">
                                                {new Date(booking.checkIn).toLocaleDateString()}
                                            </td>
                                            <td className="whitespace-nowrap px-6 py-4 text-slate-600">
                                                {new Date(booking.checkOut).toLocaleDateString()}
                                            </td>
                                            <td className="whitespace-nowrap px-6 py-4 font-semibold text-slate-900">
                                                ${booking.totalPrice.toLocaleString()}
                                            </td>
                                            <td className="whitespace-nowrap px-6 py-4">
                                                <span
                                                    className={`inline-flex rounded-full px-3 py-1 text-xs font-semibold ${getStatusStyle(booking.status)}`}
                                                >
                                                    {String(booking.status)}
                                                </span>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    )}
                </section>
            </div>
        </div>
    );
}
