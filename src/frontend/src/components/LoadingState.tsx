
interface LoadingStateProps {
  message?: string;
}

export default function LoadingState({
  message = "Loading...",
}: LoadingStateProps) {
  return (
    <div className="py-12 text-center text-slate-500">
      {message}
    </div>
  );
}
