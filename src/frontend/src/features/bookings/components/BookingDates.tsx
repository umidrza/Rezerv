
import { formatBookingDate } from "../utils/bookingUtils";

interface BookingDatesProps {
  checkIn: string;
  checkOut: string;
}

export default function BookingDates({
  checkIn,
  checkOut,
}: BookingDatesProps) {
  return (
    <div className="grid gap-4 sm:grid-cols-2">
      <div>
        <p className="text-sm text-slate-500">Check-in</p>
        <p className="font-medium">{formatBookingDate(checkIn)}</p>
      </div>

      <div>
        <p className="text-sm text-slate-500">Check-out</p>
        <p className="font-medium">{formatBookingDate(checkOut)}</p>
      </div>
    </div>
  );
}
