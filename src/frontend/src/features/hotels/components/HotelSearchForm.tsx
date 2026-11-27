import DatePicker from "../../../components/DatePicker";
import { parseISO } from "date-fns";

interface HotelSearchFormProps {
  city: string;
  setCity: (value: string) => void;
  country: string;
  setCountry: (value: string) => void;
  checkIn: string;
  setCheckIn: (value: string) => void;
  checkOut: string;
  setCheckOut: (value: string) => void;
  guests: string;
  setGuests: (value: string) => void;
  starRating: string;
  setStarRating: (value: string) => void;
  minimumAverageRating: string;
  setMinimumAverageRating: (value: string) => void;
  sortBy: string;
  setSortBy: (value: string) => void;
  loading: boolean;
  onSearch: (event: React.FormEvent<HTMLFormElement>) => void;
}

export default function HotelSearchForm({
  city,
  setCity,
  country,
  setCountry,
  checkIn,
  setCheckIn,
  checkOut,
  setCheckOut,
  guests,
  setGuests,
  starRating,
  setStarRating,
  minimumAverageRating,
  setMinimumAverageRating,
  sortBy,
  setSortBy,
  loading,
  onSearch,
}: HotelSearchFormProps) {
  function handleCheckInChange(value: string) {
    setCheckIn(value);

    if (checkOut && value && checkOut <= value) {
      setCheckOut("");
    }
  }

  const inputClassName =
    "h-12 w-full rounded-lg border border-slate-300 bg-white px-3 text-slate-700 outline-none transition focus:border-orange-500 focus:ring-2 focus:ring-orange-100";

  return (
    <form
      onSubmit={onSearch}
      className="grid gap-4 rounded-xl bg-white p-6 shadow-lg md:grid-cols-3"
    >
      <div>
        <label className="mb-2 block text-sm font-medium">City</label>
        <input
          value={city}
          onChange={(e) => setCity(e.target.value)}
          placeholder="Where are you going?"
          className={inputClassName}
        />
      </div>

      <div>
        <label className="mb-2 block text-sm font-medium">Country</label>
        <input
          value={country}
          onChange={(e) => setCountry(e.target.value)}
          placeholder="Enter country"
          className={inputClassName}
        />
      </div>

      <div>
        <label className="mb-2 block text-sm font-medium">Guests</label>
        <input
          type="number"
          min="1"
          value={guests}
          onChange={(e) => setGuests(e.target.value)}
          placeholder="Number of guests"
          className={inputClassName}
        />
      </div>

      <div>
        <label className="mb-2 block text-sm font-medium">Check-in</label>
        <DatePicker
          value={checkIn}
          onChange={handleCheckInChange}
          placeholder="Select check-in"
        />
      </div>

      <div>
        <label className="mb-2 block text-sm font-medium">Check-out</label>
        <DatePicker
          value={checkOut}
          onChange={setCheckOut}
          placeholder="Select check-out"
          minDate={checkIn ? parseISO(checkIn) : undefined}
        />
      </div>

      <div>
        <label className="mb-2 block text-sm font-medium">Star rating</label>
        <select
          value={starRating}
          onChange={(e) => setStarRating(e.target.value)}
          className={inputClassName}
        >
          <option value="">Any rating</option>
          {[3, 4, 5].map((rating) => (
            <option key={rating} value={rating}>
              {rating} stars
            </option>
          ))}
        </select>
      </div>

      <div>
        <label className="mb-2 block text-sm font-medium">
          Minimum guest rating
        </label>
        <select
          value={minimumAverageRating}
          onChange={(e) => setMinimumAverageRating(e.target.value)}
          className={inputClassName}
        >
          <option value="">Any guest rating</option>
          {[3, 4, 4.5].map((rating) => (
            <option key={rating} value={rating}>
              {rating}+ stars
            </option>
          ))}
        </select>
      </div>

      <div>
        <label className="mb-2 block text-sm font-medium">Sort by</label>
        <select
          value={sortBy}
          onChange={(e) => setSortBy(e.target.value)}
          className={inputClassName}
        >
          <option value="Name">Name</option>
          <option value="StarRating">Hotel stars</option>
          <option value="AverageRating">Guest rating</option>
        </select>
      </div>

      <div className="flex items-end">
        <button
          type="submit"
          disabled={loading}
          className="w-full rounded-lg bg-orange-500 p-3 font-semibold text-white hover:bg-orange-600 disabled:opacity-50"
        >
          {loading ? "Searching..." : "Search hotels"}
        </button>
      </div>
    </form>
  );
}
