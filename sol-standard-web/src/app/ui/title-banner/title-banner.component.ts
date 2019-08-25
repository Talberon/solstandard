import { BannerSize } from './../../model/banner-size';
import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-title-banner',
  templateUrl: './title-banner.component.html',
  styleUrls: ['./title-banner.component.less']
})
export class TitleBannerComponent implements OnInit {

  @Input() bannerSize: BannerSize;

  private readonly bannerSizes = {
    [BannerSize.SMALL]: '100px',
    [BannerSize.MEDIUM]: '200px',
    [BannerSize.LARGE]: '300px'
  };

  constructor() { }

  ngOnInit() {
  }
}
