
interface ErrorMessageProps {
    message: string;
}

export default function ErrorMessage({ message }: ErrorMessageProps) {
    if (!message) return null;

    return (
        <div
            role="alert"
            className="rounded-lg bg-red-50 px-4 py-3 text-sm text-red-600"
        >
            {message}
        </div>
    );
}
