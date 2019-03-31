import { Unit } from './../model/unit';
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

}
