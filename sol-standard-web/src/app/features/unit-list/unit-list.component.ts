import { Component, OnInit } from '@angular/core';
import { Unit, Team } from 'src/app/model/unit';
import { UnitService } from 'src/app/features/unit.service';

@Component({
  selector: 'app-unit-list',
  templateUrl: './unit-list.component.html',
  styleUrls: ['./unit-list.component.less']
})
export class UnitListComponent implements OnInit {
  units: Unit[];

  constructor(private unitService: UnitService) { }

  ngOnInit() {
    this.getUnits();
  }

  getUnits() {
    this.unitService.getUnits().subscribe(units => this.units = units);
  }

  getBluePortrait(unit: Unit): string {
    return unit.getPortrait(Team.Blue);
  }

  getRedPortrait(unit: Unit): string {
    return unit.getPortrait(Team.Red);
  }
}
