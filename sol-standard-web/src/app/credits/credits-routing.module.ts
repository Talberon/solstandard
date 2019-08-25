import { UiModule } from './../ui/ui.module';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Routes } from '@angular/router/src/config';
import { CreditsComponent } from './credits/credits.component';

const routes: Routes = [
  { path: 'credits', component: CreditsComponent }
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ]
})
export class CreditsRoutingModule { }
