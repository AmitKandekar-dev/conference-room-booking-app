import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { BookingRequest, BookingResponse, ConferenceRoom } from '../models/booking.models';

@Injectable({ providedIn: 'root' })
export class BookingApiService {
  private readonly apiBaseUrl = environment.apiBaseUrl;

  constructor(private readonly http: HttpClient) {}

  getRooms(): Observable<ConferenceRoom[]> {
    return this.http.get<ConferenceRoom[]>(`${this.apiBaseUrl}/conferencerooms`);
  }

  getBookings(): Observable<BookingResponse[]> {
    return this.http.get<BookingResponse[]>(`${this.apiBaseUrl}/bookings`);
  }

  getAdminBookings(): Observable<BookingResponse[]> {
    return this.http.get<BookingResponse[]>(`${this.apiBaseUrl}/admin/bookings`);
  }

  createBooking(request: BookingRequest): Observable<BookingResponse> {
    return this.http.post<BookingResponse>(`${this.apiBaseUrl}/bookings`, request);
  }

  updateBooking(bookingId: number, request: BookingRequest): Observable<BookingResponse> {
    return this.http.put<BookingResponse>(`${this.apiBaseUrl}/bookings/${bookingId}`, request);
  }

  cancelBooking(bookingId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/bookings/${bookingId}`);
  }
}
