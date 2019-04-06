import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SecretPageComponent } from './secret-page.component';

describe('SecretPageComponent', () => {
  let component: SecretPageComponent;
  let fixture: ComponentFixture<SecretPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SecretPageComponent ]
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
