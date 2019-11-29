import { Component, OnInit } from '@angular/core';
import { BannerSize } from './../model/banner-size';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.less']
})
export class HomeComponent implements OnInit {

  readme: string;
  bannerSize = BannerSize.MEDIUM;
  selectedUnitId: number = 1;

  constructor() { }

  ngOnInit() {
  }

  selectUnit(unitId: number) {
    this.selectedUnitId = unitId;
  }
}
