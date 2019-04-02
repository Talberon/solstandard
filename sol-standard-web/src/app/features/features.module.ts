import { FeaturesComponent } from './features.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UnitListComponent } from './unit-list/unit-list.component';
import { TilesComponent } from './tiles/tiles.component';

@NgModule({
  declarations: [
    FeaturesComponent,
    UnitListComponent,
    TilesComponent
  ],
  imports: [
    CommonModule
  ],
  exports: [
    FeaturesComponent
  ]
})
export class FeaturesModule { }
