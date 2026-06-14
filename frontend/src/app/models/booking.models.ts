export interface ConferenceRoom {
  conferenceRoomId: number;
  name: string;
  capacity: number;
}

export interface BookingRequest {
  conferenceRoomId: number | null;
  bookingDate: string;
  startTime: string;
  endTime: string;
  meetingTitle: string;
  bookedBy: string;
  bookedByEmail: string;
  numberOfPersons: number | null;
}

export interface BookingResponse {
  bookingId: number;
  conferenceRoomId: number;
  conferenceRoomName: string;
  bookingDate: string;
  startTime: string;
  endTime: string;
  meetingTitle: string;
  bookedBy: string;
  bookedByEmail: string;
  machineNameOrWindowsUsername: string;
  numberOfPersons: number;
  creditsUsed: number;
  createdDate: string;
  updatedDate: string;
  cancelledDate: string | null;
  isCancelled: boolean;
  bookingStatus: string;
}
