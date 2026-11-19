import { Navigate, Outlet, useLocation } from "react-router";
import { useAuth } from "./AuthContext";

type Role = "Admin" | "HotelManager" | "Customer";

interface ProtectedRouteProps {
  allowedRoles?: Role[];
}

export default function ProtectedRoute({
  allowedRoles,
}: ProtectedRouteProps) {
  const {
    isAuthenticated,
    isAdmin,
    isHotelManager,
    isCustomer,
  } = useAuth();

  const location = useLocation();

  if (!isAuthenticated) {
    return (
      <Navigate
        to="/login"
        state={{ from: location }}
        replace
      />
    );
  }

  if (allowedRoles) {
    const hasRequiredRole = allowedRoles.some((role) => {
      if (role === "Admin") return isAdmin;
      if (role === "HotelManager") return isHotelManager;
      if (role === "Customer") return isCustomer;

      return false;
    });

    if (!hasRequiredRole) {
      return <Navigate to="/" replace />;
    }
  }

  return <Outlet />;
}