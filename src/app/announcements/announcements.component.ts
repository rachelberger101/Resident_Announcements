import { Component, Input, OnInit } from '@angular/core';
import { AnnouncementsService, AnnouncementItem } from './announcements.service';

@Component({
  selector: 'app-announcements',
  templateUrl: './announcements.component.html',
  styleUrls: ['./announcements.component.scss']
})
export class AnnouncementsComponent implements OnInit {
  @Input() authorityCode = '6500';
  authorityName = '';
  announcements: AnnouncementItem[] = [];
  loading = true;
  error = '';

  constructor(private svc: AnnouncementsService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.svc.getAnnouncements(this.authorityCode).subscribe({
      next: (r) => {
        this.authorityName = r.authorityName;
        this.announcements = r.announcements;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'לא ניתן לטעון הודעות';
        this.loading = false;
      }
    });
  }
}