
import { format, parseISO, startOfDay } from "date-fns";
import { CalendarIcon } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Calendar } from "@/components/ui/calendar";
import {
    Popover,
    PopoverContent,
    PopoverTrigger,
} from "@/components/ui/popover";


interface DatePickerProps {
    value: string;
    onChange: (value: string) => void;
    placeholder: string;
    minDate?: Date;
}

export default function DatePicker({
    value,
    onChange,
    placeholder,
    minDate,
}: DatePickerProps) {
    const selectedDate = value ? parseISO(value) : undefined;

    const minimumDate = minDate
        ? startOfDay(minDate)
        : startOfDay(new Date());

    return (
        <Popover>
            <PopoverTrigger asChild>
                <Button
                    variant="outline"
                    className="h-12 w-full justify-start rounded-lg border border-slate-300 bg-white px-3 text-left font-normal text-slate-700 shadow-none hover:bg-white focus-visible:border-orange-500 focus-visible:ring-2 focus-visible:ring-orange-100"
                >
                    <CalendarIcon className="mr-2 h-4 w-4" />

                    {selectedDate
                        ? format(selectedDate, "dd MMM yyyy")
                        : placeholder}
                </Button>
            </PopoverTrigger>

            <PopoverContent className="w-auto p-0" align="start">
                <Calendar
                    mode="single"
                    selected={selectedDate}
                    onSelect={(date) =>
                        onChange(date ? format(date, "yyyy-MM-dd") : "")
                    }
                    disabled={{ before: minimumDate }}
                />
            </PopoverContent>
        </Popover>
    );
}
