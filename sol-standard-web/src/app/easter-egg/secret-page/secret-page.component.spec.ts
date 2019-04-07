import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SecretPageComponent } from './secret-page.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('SecretPageComponent', () => {
  let component: SecretPageComponent;
  let fixture: ComponentFixture<SecretPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [SecretPageComponent],
      imports: [BrowserAnimationsModule],
      schemas: [
        NO_ERRORS_SCHEMA
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SecretPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
