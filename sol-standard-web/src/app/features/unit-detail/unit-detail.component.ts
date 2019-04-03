import { Component, OnInit } from '@angular/core';
import { Unit } from 'src/app/model/unit';

@Component({
  selector: 'app-unit-detail',
  templateUrl: './unit-detail.component.html',
  styleUrls: ['./unit-detail.component.less']
})
export class UnitDetailComponent implements OnInit {

  unit: Unit;

  constructor() { }

  ngOnInit() {
  }

}
