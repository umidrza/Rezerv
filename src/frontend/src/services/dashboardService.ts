import { apiRequest } from "./api";
import type { Dashboard } from "../types/dashboard";

export function getAdminDashboard(): Promise<Dashboard> {
    return apiRequest<Dashboard>("/dashboard/admin");
}

export function getManagerDashboard(): Promise<Dashboard> {
    return apiRequest<Dashboard>("/dashboard/manager");
}

export function getCustomerDashboard(): Promise<Dashboard> {
    return apiRequest<Dashboard>("/dashboard/customer");
}