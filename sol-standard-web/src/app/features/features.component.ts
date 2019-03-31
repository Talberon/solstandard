import { UnitService } from './../unit.service';
import { Unit, Team } from './../model/unit';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-features',
  templateUrl: './features.component.html',
  styleUrls: ['./features.component.less']
})
export class FeaturesComponent implements OnInit {

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
