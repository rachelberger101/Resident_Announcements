import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Announcement {
  id?: number;
  title?: string;
  message?: string;
  urgent?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class AnnouncementsService {
  private readonly apiUrl = 'https://localhost:7209/api/announcements';

  constructor(private http: HttpClient) {}

  getAnnouncements(residentId: number): Observable<Announcement[]> {
    return this.http.get<Announcement[]>(`${this.apiUrl}/${residentId}`);
  }
}
