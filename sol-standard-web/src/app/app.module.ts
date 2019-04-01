import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { UiModule } from './ui/ui.module';
import { HomeComponent } from './home/home.component';
import { FeaturesComponent } from './features/features.component';
import { MediaComponent } from './media/media.component';
import { DownloadComponent } from './download/download.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { CarouselModule } from 'ngx-bootstrap/carousel';


const appRoutes: Routes = [
  { path: 'home', component: HomeComponent },
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'features', component: FeaturesComponent },
  { path: 'media', component: MediaComponent },
  { path: 'download', component: DownloadComponent },
  { path: '**', component: PageNotFoundComponent }
];

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    FeaturesComponent,
    MediaComponent,
    DownloadComponent,
    PageNotFoundComponent
  ],
  imports: [
    BrowserModule,
    UiModule,
    RouterModule.forRoot(appRoutes),
    HttpClientModule,
    CarouselModule.forRoot()
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
