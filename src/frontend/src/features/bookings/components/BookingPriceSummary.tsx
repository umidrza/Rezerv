
interface BookingPriceSummaryProps {
  pricePerNight?: number;
  totalPrice: number;
}

export default function BookingPriceSummary({
  pricePerNight,
  totalPrice,
}: BookingPriceSummaryProps) {
  return (
    <div className="space-y-4">
      {pricePerNight !== undefined && (
        <div className="flex justify-between gap-4">
          <span className="text-slate-500">Price per night</span>
          <span className="font-medium">
            ${pricePerNight.toFixed(2)}
          </span>
        </div>
      )}

      <div className="flex justify-between gap-4 border-t border-slate-100 pt-4">
        <span className="font-semibold text-slate-900">
          Total price
        </span>

        <span className="text-xl font-bold text-orange-600">
          ${totalPrice.toFixed(2)}
        </span>
      </div>
    </div>
  );
}
