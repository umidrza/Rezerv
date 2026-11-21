
interface BookingFiltersProps {
  status: string;
  onStatusChange: (status: string) => void;
}

const statuses = [
  "All",
  "Draft",
  "Submitted",
  "Approved",
  "Rejected",
  "Cancelled",
];

export default function BookingFilters({
  status,
  onStatusChange,
}: BookingFiltersProps) {
  return (
    <div className="mb-6">
      <label
        htmlFor="booking-status"
        className="mb-2 block text-sm font-medium text-slate-700"
      >
        Filter by status
      </label>

      <select
        id="booking-status"
        value={status}
        onChange={(event) => onStatusChange(event.target.value)}
        className="w-full rounded-lg border border-slate-300 px-3 py-2 sm:w-64"
      >
        {statuses.map((item) => (
          <option key={item} value={item}>
            {item}
          </option>
        ))}
      </select>
    </div>
  );
}
