export interface Room {
  id: string;
  roomTypeId: string;
  roomNumber: string;
  floor: number;
  isActive: boolean;
}

export interface CheckRoomAvailabilityRequest {
  roomIds: string[];
  checkIn: string;
  checkOut: string;
}

export interface CheckRoomAvailabilityResponse {
  isAvailable: boolean;
}