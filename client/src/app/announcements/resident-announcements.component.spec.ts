import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ResidentAnnouncementsComponent } from './resident-announcements.component';

describe('ResidentAnnouncementsComponent', () => {
  let component: ResidentAnnouncementsComponent;
  let fixture: ComponentFixture<ResidentAnnouncementsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ResidentAnnouncementsComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ResidentAnnouncementsComponent);
    component = fixture.componentInstance;
  });

  it('should show empty state when there are no active announcements', () => {
    component.announcements = [];
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('[data-testid="empty-state"]')?.textContent).toContain('אין הודעות חדשות');
  });

  it('should highlight urgent announcements', () => {
    component.announcements = [
      {
        id: 1,
        title: 'הודעת חירום',
        message: 'יש לעקוב אחרי ההנחיות העירייה.',
        urgent: true,
      },
    ];

    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const urgentCard = compiled.querySelector('.announcement-card--urgent');
    expect(urgentCard).not.toBeNull();
    expect(urgentCard?.textContent).toContain('דחופה');
  });
});
