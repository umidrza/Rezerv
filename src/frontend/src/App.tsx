import { BrowserRouter, Route, Routes } from "react-router";

import MainLayout from "./layouts/MainLayout";
import HotelsPage from "./features/hotels/pages/HotelsPage";
import HotelDetailsPage from "./features/hotels/pages/HotelDetailsPage";
import LoginPage from "./features/auth/pages/LoginPage";
import RegisterPage from "./features/auth/pages/RegisterPage";
import MyBookingsPage from "./features/bookings/pages/MyBookingsPage";
import ProtectedRoute from "./features/auth/ProtectedRoute";
import BookingDetailsPage from "./features/bookings/pages/BookingDetailsPage";
import HotelBookingsPage from "./features/bookings/pages/HotelBookingsPage";
import MyHotelsPage from "./features/hotels/pages/MyHotelsPage";

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route element={<MainLayout />}>
          <Route path="/" element={<HotelsPage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />

          <Route path="/hotels/:id" element={<HotelDetailsPage />} />

          {/* Authenticated users */}
          <Route element={<ProtectedRoute />}>
            <Route
              path="/bookings/:id"
              element={<BookingDetailsPage />}
            />
          </Route>

          {/* Customer only */}
          <Route element={<ProtectedRoute allowedRoles={["Customer"]} />}>
            <Route
              path="/my-bookings"
              element={<MyBookingsPage />}
            />
          </Route>

          {/* Admin and HotelManager */}
          <Route element={<ProtectedRoute allowedRoles={["Admin", "HotelManager"]} />}>
            <Route
              path="/my-hotels"
              element={<MyHotelsPage />}
            />
            <Route
              path="/hotels/:hotelId/bookings"
              element={<HotelBookingsPage />}
            />
          </Route>
        </Route>
      </Routes>
    </BrowserRouter>
  );
}