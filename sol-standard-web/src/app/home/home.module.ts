import { MediaComponent } from './../media/media.component';
import { UiModule } from './../ui/ui.module';
import { FeaturesModule } from './../features/features.module';
import { HomeComponent } from './home.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UpdatesComponent } from './updates/updates.component';
import { CommunityComponent } from './community/community.component';
import { CarouselModule } from 'ngx-bootstrap/carousel';

@NgModule({
  declarations: [HomeComponent, UpdatesComponent, CommunityComponent, MediaComponent],
  imports: [
    CommonModule,
    FeaturesModule,
    CarouselModule.forRoot(),
    UiModule
  ]
})
export class HomeModule { }
