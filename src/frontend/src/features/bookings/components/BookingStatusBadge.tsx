
import {
  getStatusLabel,
  getStatusStyle,
} from "../../../utils/bookingUtils";

interface BookingStatusBadgeProps {
  status: string | number;
}

export default function BookingStatusBadge({
  status,
}: BookingStatusBadgeProps) {
  const label = getStatusLabel(status);

  return (
    <span
      className={`rounded-full px-3 py-1 text-sm font-medium ${getStatusStyle(label)}`}
    >
      {label}
    </span>
  );
}
