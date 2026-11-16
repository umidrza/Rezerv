import { BrowserRouter, Route, Routes } from "react-router";

import MainLayout from "./layouts/MainLayout";
import HotelsPage from "./features/hotels/pages/HotelsPage";
import HotelDetailsPage from "./features/hotels/pages/HotelDetailsPage";
import LoginPage from "./features/auth/pages/LoginPage";
import RegisterPage from "./features/auth/pages/RegisterPage";
import MyBookingsPage from "./features/bookings/pages/MyBookingsPage";
import ProtectedRoute from "./features/auth/ProtectedRoute";

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route element={<MainLayout />}>
          <Route path="/" element={<HotelsPage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />

          <Route path="/hotels/:id" element={<HotelDetailsPage />} />

          <Route element={<ProtectedRoute />}>
            <Route path="/my-bookings" element={<MyBookingsPage />} />
          </Route>
        </Route>
      </Routes>
    </BrowserRouter>
  );
}