import { SCREENSHOTS } from './../../model/screenshot-list';
import { BannerSize } from 'src/app/model/banner-size';
import { Component, OnInit } from '@angular/core';
import { Screenshot } from 'src/app/model/screenshot';

@Component({
  selector: 'app-gallery',
  templateUrl: './gallery.component.html',
  styleUrls: ['./gallery.component.less']
})
export class GalleryComponent implements OnInit {
  bannerSize = BannerSize.SMALL;
  screenshots: Screenshot[];

  constructor() {
    this.screenshots = SCREENSHOTS;
  }

  ngOnInit() {
  }

}
