import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { CreditsRoutingModule } from './credits-routing.module';
import { CreditsComponent } from './credits/credits.component';

@NgModule({
  declarations: [CreditsComponent],
  imports: [
    CommonModule,
    CreditsRoutingModule
  ]
})
export class CreditsModule { }
