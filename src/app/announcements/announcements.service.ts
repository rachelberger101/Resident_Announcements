import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface AnnouncementItem {
  id: string;
  title: string;
  summary: string;
  categoryName: string;
  isUrgent: boolean;
  publishDate: string;
}

export interface NormalizedResponse {
  authorityCode: string;
  authorityName: string;
  announcements: AnnouncementItem[];
}

@Injectable({ providedIn: 'root' })
export class AnnouncementsService {
  constructor(private http: HttpClient) {}

  getAnnouncements(authorityCode: string): Observable<NormalizedResponse> {
    const url = `/${authorityCode}/announcements/api`;
    return this.http.get<NormalizedResponse>(url);
  }
}