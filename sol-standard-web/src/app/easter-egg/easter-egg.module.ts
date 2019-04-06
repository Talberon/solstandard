import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { EasterEggRoutingModule } from './easter-egg-routing.module';
import { SecretPageComponent } from './secret-page/secret-page.component';

@NgModule({
  declarations: [SecretPageComponent],
  imports: [
    CommonModule,
    EasterEggRoutingModule
  ]
})
export class EasterEggModule { }
