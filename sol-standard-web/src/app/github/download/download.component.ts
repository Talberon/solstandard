import { Component, OnInit } from '@angular/core';
import { BannerSize } from 'src/app/model/banner-size';

@Component({
  selector: 'app-download',
  templateUrl: './download.component.html',
  styleUrls: ['./download.component.less']
})
export class DownloadComponent implements OnInit {

  bannerSize: BannerSize = BannerSize.SMALL;

  constructor() { }

  ngOnInit() {
  }

}
