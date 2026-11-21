
interface BookingGuestInfoProps {
  numberOfGuests: number;
}

export default function BookingGuestInfo({
  numberOfGuests,
}: BookingGuestInfoProps) {
  return (
    <div>
      <p className="text-sm text-slate-500">Guests</p>
      <p className="font-medium">{numberOfGuests}</p>
    </div>
  );
}
