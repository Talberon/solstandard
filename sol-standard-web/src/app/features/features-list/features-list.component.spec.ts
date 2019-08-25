import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FeaturesListComponent } from './features-list.component';

describe('FeaturesListComponent', () => {
  let component: FeaturesListComponent;
  let fixture: ComponentFixture<FeaturesListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FeaturesListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FeaturesListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
