import { apiRequest } from "./api";
import type { Dashboard } from "../types/dashboard";

export function getAdminDashboard(token: string): Promise<Dashboard> {
    return apiRequest<Dashboard>("/dashboard/admin", {}, token);
}

export function getManagerDashboard(token: string): Promise<Dashboard> {
    return apiRequest<Dashboard>("/dashboard/manager", {}, token);
}

export function getCustomerDashboard(token: string): Promise<Dashboard> {
    return apiRequest<Dashboard>("/dashboard/customer", {}, token);
}