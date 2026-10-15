import { Component } from '@angular/core';

import { Announcement } from './announcements/announcement.service';
import { ResidentAnnouncementsComponent } from './announcements/resident-announcements.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [ResidentAnnouncementsComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  title = 'הודעות לתושב';

  announcements: Announcement[] = [
    {
      id: 1,
      title: 'הודעה דחופה',
      message: 'שימו לב: החל ממחר יושבת תחבורה ציבורית בשירות מוגבל באזור צפון העיר.',
      urgent: true,
    },
    {
      id: 2,
      title: 'עדכון תפעולי',
      message: 'הדלתות במבנה העירייה יפתחו בשעה 08:00 במקום 07:30.',
    },
  ];
}
