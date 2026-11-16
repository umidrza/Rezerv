
import { Link, Outlet } from "react-router";

function MainLayout() {
  return (
    <>
      <header className="border-b border-slate-200 bg-white">
        <nav className="mx-auto flex max-w-6xl items-center justify-between px-6 py-5">
          <Link
            to="/"
            className="text-2xl font-bold text-orange-500"
          >
            Rezerv
          </Link>

          <div className="flex items-center gap-6">
            <Link
              to="/"
              className="font-medium text-slate-600 transition hover:text-orange-500"
            >
              Home
            </Link>

            <Link
              to="/login"
              className="font-medium text-slate-600 transition hover:text-orange-500"
            >
              Login
            </Link>

            <Link
              to="/register"
              className="rounded-lg bg-orange-500 px-5 py-2.5 font-medium text-white transition hover:bg-orange-600"
            >
              Register
            </Link>
          </div>
        </nav>
      </header>

      <Outlet />
    </>
  );
}

export default MainLayout;
