
import type { ReactNode } from "react";

interface BookingListProps {
  children: ReactNode;
}

export default function BookingList({
  children,
}: BookingListProps) {
  return <div className="space-y-4">{children}</div>;
}
