import { UnitDetailComponent } from './unit-detail/unit-detail.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';

const featureRoutes: Routes = [
  { path: 'features/unit/:id', component: UnitDetailComponent }
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(featureRoutes)
  ],
  exports: [RouterModule]
})
export class FeaturesRoutingModule { }
