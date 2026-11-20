
export default function BookingEmptyState() {
  return (
    <div className="rounded-xl border border-slate-200 bg-white p-8 text-center">
      <h2 className="text-lg font-semibold text-slate-800">
        No bookings yet
      </h2>

      <p className="mt-2 text-slate-500">
        Your bookings will appear here after you submit a booking request.
      </p>
    </div>
  );
}
