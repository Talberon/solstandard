import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HowToPlayComponent } from './how-to-play.component';

describe('HowToPlayComponent', () => {
  let component: HowToPlayComponent;
  let fixture: ComponentFixture<HowToPlayComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HowToPlayComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HowToPlayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
