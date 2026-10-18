import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

import { Announcement } from './announcement.service';

@Component({
  selector: 'app-resident-announcements',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './resident-announcements.component.html',
  styleUrl: './resident-announcements.component.scss',
})
export class ResidentAnnouncementsComponent {
  @Input() title = 'הודעות לתושב';
  @Input() announcements: Announcement[] = [];

  // trackByAnnouncement(index: number, announcement: Announcement): string {
  //   return announcement.id ?? index;
  // }
}
