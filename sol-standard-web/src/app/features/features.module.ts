import { CarouselModule } from 'ngx-bootstrap/carousel';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UnitListComponent } from './unit-list/unit-list.component';
import { UnitDetailComponent } from './unit-detail/unit-detail.component';
import { TilesComponent } from './tiles/tiles.component';
import { FeaturesRoutingModule } from './features-routing.module';
import { FeaturesListComponent } from './features-list/features-list.component';

@NgModule({
  declarations: [
    UnitListComponent,
    UnitDetailComponent,
    TilesComponent,
    FeaturesListComponent
  ],
  imports: [
    CommonModule,
    FeaturesRoutingModule,
    CarouselModule
  ],
  exports: [
    UnitListComponent,
    TilesComponent,
    FeaturesListComponent,
    UnitDetailComponent
  ]
})
export class FeaturesModule { }
