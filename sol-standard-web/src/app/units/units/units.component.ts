import { Component, OnInit } from '@angular/core';
import { BannerSize } from 'src/app/model/banner-size';

@Component({
  selector: 'app-units',
  templateUrl: './units.component.html',
  styleUrls: ['./units.component.less']
})
export class UnitsComponent implements OnInit {
  bannerSize: BannerSize = BannerSize.SMALL;

  selectedUnitId = 1;

  constructor() { }

  ngOnInit() {
  }

  selectUnit(unitId: number) {
    this.selectedUnitId = unitId;
  }
}
