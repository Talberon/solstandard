import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { Unit, Team, Role } from 'src/app/model/unit';
import { UnitService } from 'src/app/features/unit.service';

@Component({
  selector: 'app-unit-list',
  templateUrl: './unit-list.component.html',
  styleUrls: ['./unit-list.component.less']
})
export class UnitListComponent implements OnInit {
  @Output() selectedUnitEvent = new EventEmitter<number>();

  units: Unit[];
  unitHovering: Map<Unit, boolean> = new Map();

  constructor(private unitService: UnitService) { }

  ngOnInit() {
    this.getUnits();
  }

  selectUnit(unit: Unit) {
    this.selectedUnitEvent.emit(unit.id);
  }

  getUnits() {
    this.unitService.getUnits().subscribe(units => this.units = units);
  }

  roles() {
    return Object.keys(Role);
  }

  startHoverPortrait(unit: Unit) {
    this.unitHovering.set(unit, true);
  }

  stopHoverPortrait(unit: Unit) {
    this.unitHovering.set(unit, false);
  }

  unitIsHovered(unit: Unit) {
    if (this.unitHovering.has(unit)) {
      return this.unitHovering.get(unit);
    } else {
      return false;
    }
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
