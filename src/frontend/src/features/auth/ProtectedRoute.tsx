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
    isLoading,
    isAdmin,
    isHotelManager,
    isCustomer,
  } = useAuth();

  const location = useLocation();

  if (isLoading) {
    return <p role="status" className="p-6 text-center text-slate-500">Checking your session...</p>;
  }

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