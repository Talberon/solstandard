import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LayoutComponent } from './layout/layout.component';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { RouterModule } from '@angular/router';
import { EggComponent } from './egg/egg.component';
import { TitleBannerComponent } from './title-banner/title-banner.component';

@NgModule({
  declarations: [LayoutComponent, HeaderComponent, FooterComponent, EggComponent, TitleBannerComponent],
  imports: [
    CommonModule,
    RouterModule
  ],
  exports: [LayoutComponent, TitleBannerComponent]
})
export class UiModule { }