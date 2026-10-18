import { Component, OnInit } from '@angular/core';
import { Announcement, AnnouncementsService } from './announcements/announcement.service';
import { ResidentAnnouncementsComponent } from './announcements/resident-announcements.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [ResidentAnnouncementsComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit {
  title = 'הודעות לתושב';
  announcements: Announcement[] = [];
  errorMessage = '';
  private readonly authorityCode = '6500';

  constructor(private readonly announcementsService: AnnouncementsService) {}

  ngOnInit(): void {
    this.announcementsService.getAnnouncements(this.authorityCode).subscribe({
      next: (response) => {
        this.announcements = response.announcements ?? [];
      },
      error: () => {
        this.errorMessage = 'לא ניתן לטעון הודעות כרגע. נסו שוב מאוחר יותר.';
      },
    });
  }
}
