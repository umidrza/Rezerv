
interface ErrorMessageProps {
  message: string;
}

export default function ErrorMessage({
  message,
}: ErrorMessageProps) {
  if (!message) return null;

  return (
    <div className="rounded-lg bg-red-100 p-3 text-red-700">
      {message}
    </div>
  );
}
