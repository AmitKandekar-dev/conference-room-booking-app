import { Component, OnInit, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { BookingApiService } from './services/booking-api.service';
import { BookingRequest, BookingResponse, ConferenceRoom } from './models/booking.models';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  rooms = signal<ConferenceRoom[]>([]);
  bookings = signal<BookingResponse[]>([]);
  selectedBooking = signal<BookingResponse | null>(null);
  loading = signal(false);
  message = signal('');
  error = signal('');

  activeBookings = computed(() => this.bookings().filter(booking => !booking.isCancelled));

  bookingForm = this.fb.nonNullable.group({
    conferenceRoomId: [null as number | null, [Validators.required]],
    bookingDate: ['', [Validators.required]],
    startTime: ['', [Validators.required]],
    endTime: ['', [Validators.required]],
    meetingTitle: ['', [Validators.required, Validators.maxLength(200)]],
    bookedBy: ['', [Validators.required, Validators.maxLength(150)]],
    bookedByEmail: ['', [Validators.required, Validators.email, Validators.maxLength(254)]],
    numberOfPersons: [1 as number | null, [Validators.required, Validators.min(1), Validators.max(1000)]]
  });

  constructor(private readonly fb: FormBuilder, private readonly bookingApi: BookingApiService) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading.set(true);
    this.error.set('');

    this.bookingApi.getRooms().subscribe({
      next: rooms => this.rooms.set(rooms),
      error: () => this.error.set('Unable to load conference rooms.')
    });

    this.bookingApi.getAdminBookings().subscribe({
      next: bookings => {
        this.bookings.set(bookings);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Unable to load bookings.');
        this.loading.set(false);
      }
    });
  }

  submitBooking(): void {
    this.message.set('');
    this.error.set('');
    this.bookingForm.markAllAsTouched();

    if (this.bookingForm.invalid) {
      this.error.set('Please complete the booking form with a valid email address.');
      return;
    }

    const request = this.bookingForm.getRawValue() as BookingRequest;
    const selected = this.selectedBooking();
    const operation = selected
      ? this.bookingApi.updateBooking(selected.bookingId, request)
      : this.bookingApi.createBooking(request);

    this.loading.set(true);
    operation.subscribe({
      next: () => {
        this.message.set(selected ? 'Booking updated and notification queued.' : 'Booking created and notification queued.');
        this.resetForm();
        this.loadData();
      },
      error: response => {
        this.error.set(response?.error?.message ?? 'Booking could not be saved.');
        this.loading.set(false);
      }
    });
  }

  editBooking(booking: BookingResponse): void {
    this.selectedBooking.set(booking);
    this.message.set('');
    this.error.set('');
    this.bookingForm.patchValue({
      conferenceRoomId: booking.conferenceRoomId,
      bookingDate: booking.bookingDate,
      startTime: this.normalizeTime(booking.startTime),
      endTime: this.normalizeTime(booking.endTime),
      meetingTitle: booking.meetingTitle,
      bookedBy: booking.bookedBy,
      bookedByEmail: booking.bookedByEmail,
      numberOfPersons: booking.numberOfPersons
    });
  }

  cancelBooking(booking: BookingResponse): void {
    this.message.set('');
    this.error.set('');
    this.loading.set(true);

    this.bookingApi.cancelBooking(booking.bookingId).subscribe({
      next: () => {
        this.message.set('Booking cancelled and notification queued.');
        this.resetForm();
        this.loadData();
      },
      error: response => {
        this.error.set(response?.error?.message ?? 'Booking could not be cancelled.');
        this.loading.set(false);
      }
    });
  }

  resetForm(): void {
    this.selectedBooking.set(null);
    this.bookingForm.reset({
      conferenceRoomId: null,
      bookingDate: '',
      startTime: '',
      endTime: '',
      meetingTitle: '',
      bookedBy: '',
      bookedByEmail: '',
      numberOfPersons: 1
    });
  }

  isInvalid(controlName: keyof typeof this.bookingForm.controls): boolean {
    const control = this.bookingForm.controls[controlName];
    return control.invalid && (control.dirty || control.touched);
  }

  private normalizeTime(value: string): string {
    return value?.length >= 5 ? value.substring(0, 5) : value;
  }
}
