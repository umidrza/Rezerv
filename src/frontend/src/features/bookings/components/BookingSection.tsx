
import type { ReactNode } from "react";

interface BookingSectionProps {
  title: string;
  children: ReactNode;
}

export default function BookingSection({
  title,
  children,
}: BookingSectionProps) {
  return (
    <section className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
      <h2 className="text-lg font-semibold text-slate-900">
        {title}
      </h2>

      <div className="mt-5">
        {children}
      </div>
    </section>
  );
}
