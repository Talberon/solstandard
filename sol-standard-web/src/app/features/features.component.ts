import { Unit, Team } from './../model/unit';
import { Component, OnInit } from '@angular/core';
import { UNITS } from '../model/unit-list';

@Component({
  selector: 'app-features',
  templateUrl: './features.component.html',
  styleUrls: ['./features.component.less']
})
export class FeaturesComponent implements OnInit {

  units: Unit[] = UNITS;

  constructor() { }

  ngOnInit() {
  }

  getBluePortrait(unit: Unit): string {
    return unit.getPortrait(Team.Blue);
  }

  getRedPortrait(unit: Unit): string {
    return unit.getPortrait(Team.Red);
  }

}
