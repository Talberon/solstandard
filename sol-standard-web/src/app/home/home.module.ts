import { UiModule } from './../ui/ui.module';
import { FeaturesModule } from './../features/features.module';
import { HomeComponent } from './home.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UpdatesComponent } from './updates/updates.component';
import { CommunityComponent } from './community/community.component';

@NgModule({
  declarations: [HomeComponent, UpdatesComponent, CommunityComponent],
  imports: [
    CommonModule,
    FeaturesModule,
    UiModule
  ]
})
export class HomeModule { }
