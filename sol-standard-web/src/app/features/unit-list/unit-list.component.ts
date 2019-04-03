import { Component, OnInit } from '@angular/core';
import { Unit, Team, Role } from 'src/app/model/unit';
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

  roles() {
    return Object.keys(Role);
  }

  getUnitsByRole(role: string): Unit[] {
    return this.units.filter(unit => unit.role === role);
  }

  getRandomPortrait(unit: Unit): string {
    return (Math.floor(Math.random() * 2) === 0) ? this.getBluePortrait(unit) : this.getRedPortrait(unit);
  }

  getBluePortrait(unit: Unit): string {
    return unit.getPortrait(Team.Blue);
  }

  getRedPortrait(unit: Unit): string {
    return unit.getPortrait(Team.Red);
  }
}
