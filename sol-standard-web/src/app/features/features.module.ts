import { FeaturesComponent } from './features.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UnitListComponent } from './unit-list/unit-list.component';

@NgModule({
  declarations: [
    FeaturesComponent,
    UnitListComponent
  ],
  imports: [
    CommonModule
  ],
  exports: [
    FeaturesComponent
  ]
})
export class FeaturesModule { }
