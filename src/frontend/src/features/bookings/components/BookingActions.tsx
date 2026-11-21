
interface BookingActionsProps {
  onApprove?: () => void;
  onReject?: () => void;
  onCancel?: () => void;
  showApproveReject?: boolean;
  showCancel?: boolean;
  disabled?: boolean;
  loading?: boolean;
  loadingAction?: "approve" | "reject" | "cancel";
}

export default function BookingActions({
  onApprove,
  onReject,
  onCancel,
  showApproveReject = false,
  showCancel = false,
  disabled = false,
  loading = false,
  loadingAction,
}: BookingActionsProps) {
  return (
    <div className="flex flex-wrap gap-2">
      {showApproveReject && (
        <>
          <button
            type="button"
            onClick={onApprove}
            disabled={disabled}
            className="rounded-lg bg-green-600 px-4 py-2 text-white hover:bg-green-700 disabled:opacity-50"
          >
            {loading && loadingAction === "approve"
              ? "Approving..."
              : "Approve"}
          </button>

          <button
            type="button"
            onClick={onReject}
            disabled={disabled}
            className="rounded-lg bg-red-600 px-4 py-2 text-white hover:bg-red-700 disabled:opacity-50"
          >
            {loading && loadingAction === "reject"
              ? "Rejecting..."
              : "Reject"}
          </button>
        </>
      )}

      {showCancel && (
        <button
          type="button"
          onClick={onCancel}
          disabled={disabled}
          className="rounded-lg bg-red-600 px-4 py-2 text-white hover:bg-red-700 disabled:opacity-50"
        >
          {loading ? "Cancelling..." : "Cancel Booking"}
        </button>
      )}
    </div>
  );
}
