import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DownloadComponent } from './download.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('DownloadComponent', () => {
  let component: DownloadComponent;
  let fixture: ComponentFixture<DownloadComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [DownloadComponent],
      schemas: [NO_ERRORS_SCHEMA]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DownloadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
