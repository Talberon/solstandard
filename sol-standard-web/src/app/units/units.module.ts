import { UnitDetailComponent } from './../features/unit-detail/unit-detail.component';
import { UnitListComponent } from './../features/unit-list/unit-list.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UnitsComponent } from './units/units.component';
import { CarouselModule } from 'ngx-bootstrap/carousel';
import { UiModule } from '../ui/ui.module';

@NgModule({
  declarations: [
    UnitsComponent,
    UnitDetailComponent,
    UnitListComponent
  ],
  imports: [
    CommonModule,
    CarouselModule,
    UiModule
  ]
})
export class UnitsModule { }
