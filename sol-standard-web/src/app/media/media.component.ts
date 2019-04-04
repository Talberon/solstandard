import { MediaImage } from '../model/media-image';
import { Component, OnInit } from '@angular/core';
import { IMAGES } from '../model/media-image-list';
import { CarouselConfig } from 'ngx-bootstrap/carousel';

@Component({
  selector: 'app-media',
  templateUrl: './media.component.html',
  styleUrls: ['./media.component.less'],
  providers: [
    { provide: CarouselConfig, useValue: { interval: 30000, noPause: false, showIndicators: true } }
  ]
})
export class MediaComponent implements OnInit {

  images: MediaImage[] = IMAGES;

  constructor() { }

  ngOnInit() {
  }

}
