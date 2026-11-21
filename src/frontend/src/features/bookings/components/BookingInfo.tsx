
import DetailItem from "./DetailItem";
import { formatBookingDate } from "../utils/bookingUtils";

interface BookingInfoProps {
  bookingId: string;
  createdAt: string;
}

export default function BookingInfo({
  bookingId,
  createdAt,
}: BookingInfoProps) {
  return (
    <div className="space-y-4">
      <DetailItem label="Booking ID" value={bookingId} />

      <DetailItem
        label="Created at"
        value={formatBookingDate(createdAt)}
      />
    </div>
  );
}
