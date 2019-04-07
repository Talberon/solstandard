import { HttpClientModule } from '@angular/common/http';
import { MarkdownModule, MarkdownService, MarkedOptions } from 'ngx-markdown';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PullRequestsComponent } from './pull-requests.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('PullRequestsComponent', () => {
  let component: PullRequestsComponent;
  let fixture: ComponentFixture<PullRequestsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [PullRequestsComponent],
      schemas: [
        NO_ERRORS_SCHEMA
      ],
      imports: [MarkdownModule, HttpClientModule, MarkdownModule],
      providers: [MarkdownService, MarkedOptions]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PullRequestsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
