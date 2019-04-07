import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CreditsComponent } from './credits/credits.component';
import { Routes } from '@angular/router/src/config';
import { RouterModule } from '@angular/router';

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
