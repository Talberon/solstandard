import { UiModule } from './../ui/ui.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HowToPlayComponent } from './how-to-play/how-to-play.component';

@NgModule({
  declarations: [HowToPlayComponent],
  imports: [
    CommonModule,
    UiModule
  ]
})
export class HowToPlayModule { }
