import { Component, Input, OnInit } from '@angular/core';
import { CarouselConfig } from 'ngx-bootstrap/carousel';
import { take } from 'rxjs/operators';
import { Team, Unit } from 'src/app/model/unit';
import { UnitService } from '../unit.service';

@Component({
  selector: 'app-unit-detail',
  templateUrl: './unit-detail.component.html',
  styleUrls: ['./unit-detail.component.less'],
  providers: [
    { provide: CarouselConfig, useValue: { interval: 30000, noPause: false, showIndicators: true } }
  ]
})
export class UnitDetailComponent implements OnInit {

  private _unitId: number;

  unit: Unit;

  constructor(private unitService: UnitService) { }

  ngOnInit() {
    this.loadUnitDetail();
  }

  loadUnitDetail() {
    this.unitService.getUnit(this._unitId).pipe(take(1)).subscribe(unit => this.unit = unit);
  }

  @Input()
  set unitId(unitId: number) {
    this._unitId = unitId;
    this.loadUnitDetail();
  }

  getBluePortrait(unit: Unit): string {
    return unit.getPortrait(Team.Blue);
  }

  getRedPortrait(unit: Unit): string {
    return unit.getPortrait(Team.Red);
  }
}
