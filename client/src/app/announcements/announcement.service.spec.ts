import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { Announcement, AnnouncementsService } from './announcement.service';

describe('AnnouncementsService', () => {
  let service: AnnouncementsService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AnnouncementsService]
    });

    service = TestBed.inject(AnnouncementsService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should fetch announcements for a resident id', () => {
    const mockAnnouncements: Announcement[] = [
      { id: 1, title: 'Announcement 1', message: 'hello world' },
      { id: 2, title: 'Announcement 2', message: 'hello again' }
    ];

    service.getAnnouncements(6500).subscribe((announcements) => {
      expect(announcements).toEqual(mockAnnouncements);
    });

    const req = httpMock.expectOne('https://localhost:7209/api/announcements/6500');
    expect(req.request.method).toBe('GET');
    req.flush(mockAnnouncements);
  });
});
