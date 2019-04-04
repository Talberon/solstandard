import { FeaturesComponent } from './features.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UnitListComponent } from './unit-list/unit-list.component';
import { UnitDetailComponent } from './unit-detail/unit-detail.component';
import { TilesComponent } from './tiles/tiles.component';
import { FeaturesRoutingModule } from './features-routing.module';

@NgModule({
  declarations: [
    FeaturesComponent,
    UnitListComponent,
    UnitDetailComponent,
    TilesComponent
  ],
  imports: [
    CommonModule,
    FeaturesRoutingModule
  ],
  exports: [
    FeaturesComponent
  ]
})
export class FeaturesModule { }
