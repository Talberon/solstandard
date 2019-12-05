import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ItchIoComponent } from './itch-io.component';

describe('ItchIoComponent', () => {
  let component: ItchIoComponent;
  let fixture: ComponentFixture<ItchIoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ItchIoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ItchIoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
