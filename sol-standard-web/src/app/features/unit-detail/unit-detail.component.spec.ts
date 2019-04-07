import { NO_ERRORS_SCHEMA } from '@angular/core';
import { async, ComponentFixture, fakeAsync, TestBed } from '@angular/core/testing';
import { ActivatedRoute } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { of } from 'rxjs';
import { UnitService } from 'src/app/features/unit.service';
import { UnitDetailComponent } from './unit-detail.component';

class MockActivatedRoute extends ActivatedRoute {
  constructor() {
    super();
    this.params = of({ id: 1 });

  }
}

describe('UnitDetailComponent', () => {
  let component: UnitDetailComponent;
  let fixture: ComponentFixture<UnitDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [UnitDetailComponent],
      schemas: [
        NO_ERRORS_SCHEMA
      ],
      imports: [RouterTestingModule],
      providers: [
        MockActivatedRoute,
        UnitService
      ]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UnitDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', fakeAsync(() => {

    expect(component).toBeTruthy();
  }));
});
