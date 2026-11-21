
interface DetailItemProps {
    label: string;
    value: string;
}

export default function DetailItem({
    label,
    value,
}: DetailItemProps) {
    return (
        <div>
            <p className="text-sm text-slate-500">{label}</p>
            <p className="mt-1 font-medium text-slate-800">{value}</p>
        </div>
    );
}
