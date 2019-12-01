import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { CarouselModule } from 'ngx-bootstrap/carousel';
import { FeaturesListComponent } from './features-list/features-list.component';
import { FeaturesRoutingModule } from './features-routing.module';
import { TilesComponent } from './tiles/tiles.component';

@NgModule({
  declarations: [
    TilesComponent,
    FeaturesListComponent
  ],
  imports: [
    CommonModule,
    FeaturesRoutingModule,
    CarouselModule
  ],
  exports: [
    TilesComponent,
    FeaturesListComponent,
  ]
})
export class FeaturesModule { }
