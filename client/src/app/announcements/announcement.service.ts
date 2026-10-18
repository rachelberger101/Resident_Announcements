import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Announcement {
  id: string;
  title: string;
  summary: string;
  categoryName: string;
  isUrgent: boolean;
  publishDate: string;
}

export interface AnnouncementsResponse {
  authorityCode: string;
  authorityName: string;
  announcements: Announcement[];
}

@Injectable({
  providedIn: 'root'
})
export class AnnouncementsService {
  private readonly apiUrl = 'https://localhost:7209/api/announcements';

  constructor(private http: HttpClient) {}

  getAnnouncements(authorityCode: string): Observable<AnnouncementsResponse> {
    return this.http.get<AnnouncementsResponse>(`${this.apiUrl}/${encodeURIComponent(authorityCode)}`);
  }
}
