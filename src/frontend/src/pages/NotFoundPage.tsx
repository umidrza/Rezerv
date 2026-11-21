import { Link } from "react-router";

export default function NotFoundPage() {
    return (
        <main className="flex min-h-[80vh] flex-col items-center justify-center px-4 text-center">
            <h1 className="text-8xl font-bold text-orange-500">
                404
            </h1>

            <h2 className="mt-4 text-2xl font-bold text-slate-900">
                Page not found
            </h2>

            <p className="mt-3 max-w-md text-slate-500">
                Sorry, the page you're looking for doesn't exist or may have
                been moved.
            </p>

            <Link
                to="/"
                className="mt-8 rounded-lg bg-orange-500 px-6 py-3 font-semibold text-white transition hover:bg-orange-600"
            >
                Back to home
            </Link>
        </main>
    );
}
