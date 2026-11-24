import { Link, useNavigate } from "react-router";
import { useAuth } from "../features/auth/AuthContext";

export default function Navbar() {
  const { isAuthenticated, isAdmin, isHotelManager, isCustomer, logout } =
    useAuth();

  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate("/login");
  }

  return (
    <header className="border-b border-slate-200 bg-white">
      <div className="mx-auto flex max-w-7xl items-center justify-between px-4 py-4">
        <Link to="/" className="flex items-center gap-2">
          <img
            src="/images/icon.png"
            alt="Rezerv"
            className="h-9 w-9 object-contain"
          />

          <span className="text-2xl font-bold text-orange-500">Rezerv</span>
        </Link>

        <nav className="flex items-center gap-6">
          {isAuthenticated ? (
            <>
              <Link
                to="/dashboard"
                className="text-sm font-medium text-slate-700 hover:text-orange-500"
              >
                Dashboard
              </Link>

              {isCustomer && (
                <Link
                  to="/my-bookings"
                  className="text-sm font-medium text-slate-700 hover:text-orange-500"
                >
                  My Bookings
                </Link>
              )}

              {(isAdmin || isHotelManager) && (
                <Link
                  to="/my-hotels"
                  className="text-sm font-medium text-slate-700 hover:text-orange-500"
                >
                  My Hotels
                </Link>
              )}

              <button
                onClick={handleLogout}
                className="text-sm font-medium text-slate-700 hover:text-orange-500"
              >
                Logout
              </button>
            </>
          ) : (
            <>
              <Link
                to="/login"
                className="text-sm font-medium text-slate-700 hover:text-orange-500"
              >
                Login
              </Link>

              <Link
                to="/register"
                className="rounded-lg bg-orange-500 px-4 py-2 text-sm font-medium text-white hover:bg-orange-600"
              >
                Register
              </Link>
            </>
          )}
        </nav>
      </div>
    </header>
  );
}
