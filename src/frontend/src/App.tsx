import { BrowserRouter, Outlet, Route, Routes } from "react-router";

import MainLayout from "./layouts/MainLayout";
import ErrorBoundary from "./components/ErrorBoundary";

import HotelsPage from "./features/hotels/pages/HotelsPage";
import HotelDetailsPage from "./features/hotels/pages/HotelDetailsPage";
import MyHotelsPage from "./features/hotels/pages/MyHotelsPage";

import LoginPage from "./features/auth/pages/LoginPage";
import RegisterPage from "./features/auth/pages/RegisterPage";
import ProtectedRoute from "./features/auth/ProtectedRoute";

import MyBookingsPage from "./features/bookings/pages/MyBookingsPage";
import BookingDetailsPage from "./features/bookings/pages/BookingDetailsPage";
import HotelBookingsPage from "./features/bookings/pages/HotelBookingsPage";
import RoomManagementPage from "./features/hotels/pages/RoomManagementPage";
import MyFavoritesPage from "./features/hotels/pages/MyFavoritesPage";

import NotFoundPage from "./pages/NotFoundPage";
import DashboardPage from "./pages/DashboardPage";

export default function App() {
  return (
    <BrowserRouter>
      <ErrorBoundary>
        <Routes>
          <Route element={<MainLayout />}>
            <Route element={<ErrorBoundary><Outlet /></ErrorBoundary>}>
              {/* Public routes */}
              <Route path="/" element={<HotelsPage />} />
              <Route path="/hotels" element={<HotelsPage />} />
              <Route path="/login" element={<LoginPage />} />
              <Route path="/register" element={<RegisterPage />} />
              <Route path="/hotels/:id" element={<HotelDetailsPage />} />

              {/* Authenticated users */}
              <Route element={<ProtectedRoute />}>
                <Route path="/bookings/:id" element={<BookingDetailsPage />} />
                <Route path="/dashboard" element={<DashboardPage />} />
              </Route>

              {/* Customer only */}
              <Route element={<ProtectedRoute allowedRoles={["Customer"]} />}>
                <Route path="/my-bookings" element={<MyBookingsPage />} />
                <Route path="/my-favorites" element={<MyFavoritesPage />} />
              </Route>

              {/* Admin and HotelManager */}
              <Route
                element={
                  <ProtectedRoute allowedRoles={["Admin", "HotelManager"]} />
                }
              >
                <Route path="/my-hotels" element={<MyHotelsPage />} />
                <Route
                  path="/hotels/:hotelId/bookings"
                  element={<HotelBookingsPage />}
                />
                <Route
                  path="/room-types/:roomTypeId/rooms"
                  element={<RoomManagementPage />}
                />
              </Route>

              {/* Not found */}
              <Route path="*" element={<NotFoundPage />} />
            </Route>
          </Route>
        </Routes>
      </ErrorBoundary>
    </BrowserRouter>
  );
}
