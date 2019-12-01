import { UiModule } from './../ui/ui.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GalleryComponent } from './gallery/gallery.component';

@NgModule({
  declarations: [GalleryComponent],
  imports: [
    CommonModule,
    UiModule
  ]
})
export class GalleryModule { }
