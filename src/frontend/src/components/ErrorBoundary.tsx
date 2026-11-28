import { Component } from "react";
import type { ErrorInfo, ReactNode } from "react";

interface ErrorBoundaryProps {
    children: ReactNode;
}

interface ErrorBoundaryState {
    hasError: boolean;
}

export default class ErrorBoundary extends Component<
    ErrorBoundaryProps,
    ErrorBoundaryState
> {
    state: ErrorBoundaryState = { hasError: false };

    static getDerivedStateFromError(): ErrorBoundaryState {
        return { hasError: true };
    }

    componentDidCatch(error: Error, errorInfo: ErrorInfo) {
        console.error("An unexpected application error occurred.", error, errorInfo);
    }

    private reset = () => {
        this.setState({ hasError: false });
    };

    render() {
        if (this.state.hasError) {
            return (
                <main className="flex min-h-screen items-center justify-center bg-slate-50 px-4 py-12">
                    <section
                        role="alert"
                        aria-live="assertive"
                        className="w-full max-w-lg rounded-2xl bg-white p-8 text-center shadow-sm"
                    >
                        <h1 className="text-2xl font-bold text-slate-900">
                            Something went wrong
                        </h1>
                        <p className="mt-3 text-slate-600">
                            An unexpected error occurred. You can try again or return home.
                        </p>
                        <div className="mt-6 flex flex-wrap justify-center gap-3">
                            <button
                                type="button"
                                onClick={this.reset}
                                className="rounded-lg bg-orange-500 px-5 py-3 font-semibold text-white hover:bg-orange-600 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-orange-600"
                            >
                                Try Again
                            </button>
                            <a
                                href="/"
                                className="rounded-lg border border-slate-300 px-5 py-3 font-semibold text-slate-700 hover:bg-slate-50 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-slate-600"
                            >
                                Back to Home
                            </a>
                        </div>
                    </section>
                </main>
            );
        }

        return this.props.children;
    }
}
