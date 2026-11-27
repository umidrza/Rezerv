import HotelCard from "../components/HotelCard";
import HotelSearchForm from "../components/HotelSearchForm";
import { useHotelSearch } from "../hooks/useHotelSearch";

export default function HotelsPage() {
  const search = useHotelSearch();

  function handleSearch(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    search.loadHotels(1);
  }

  return (
    <main className="min-h-screen bg-slate-50">
      <section className="bg-orange-500 px-6 py-20 text-center text-white">
        <h1 className="text-4xl font-bold">Find your perfect stay</h1>
        <p className="mt-4 text-lg">
          Discover hotels and make your next trip memorable.
        </p>
      </section>

      <section className="mx-auto -mt-8 max-w-6xl px-6">
        <HotelSearchForm
          city={search.city}
          setCity={search.setCity}
          country={search.country}
          setCountry={search.setCountry}
          checkIn={search.checkIn}
          setCheckIn={search.setCheckIn}
          checkOut={search.checkOut}
          setCheckOut={search.setCheckOut}
          guests={search.guests}
          setGuests={search.setGuests}
          starRating={search.starRating}
          setStarRating={search.setStarRating}
          minimumAverageRating={search.minimumAverageRating}
          setMinimumAverageRating={search.setMinimumAverageRating}
          sortBy={search.sortBy}
          setSortBy={search.setSortBy}
          loading={search.loading}
          onSearch={handleSearch}
        />
      </section>

      <section className="mx-auto max-w-6xl px-6 py-12">
        <h2 className="mb-6 text-2xl font-bold text-slate-900">
          {search.hotels.length > 0 ? "Search results" : "Discover hotels"}
        </h2>

        {search.error && (
          <p className="mb-6 rounded-lg bg-red-100 p-4 text-red-700">
            {search.error}
          </p>
        )}

        {!search.loading && search.hotels.length === 0 && !search.error && (
          <p className="text-slate-500">
            Search for hotels to see available results.
          </p>
        )}

        <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
          {search.hotels.map((hotel) => (
            <HotelCard key={hotel.id} hotel={hotel} />
          ))}
        </div>

        {search.totalPages > 1 && (
          <div className="mt-8 flex items-center justify-center gap-4">
            <button
              disabled={search.page === 1 || search.loading}
              onClick={() => search.loadHotels(search.page - 1)}
              className="rounded-lg border px-4 py-2 disabled:opacity-40"
            >
              Previous
            </button>

            <span className="text-slate-600">
              Page {search.page} of {search.totalPages}
            </span>

            <button
              disabled={search.page === search.totalPages || search.loading}
              onClick={() => search.loadHotels(search.page + 1)}
              className="rounded-lg border px-4 py-2 disabled:opacity-40"
            >
              Next
            </button>
          </div>
        )}
      </section>
    </main>
  );
}
