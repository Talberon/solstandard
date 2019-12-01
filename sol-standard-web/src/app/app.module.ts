import { GalleryModule } from './gallery/gallery.module';
import { GalleryComponent } from './gallery/gallery/gallery.component';
import { UnitsModule } from './units/units.module';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { MarkdownModule } from 'ngx-markdown';
import { AppComponent } from './app.component';
import { CreditsRoutingModule } from './credits/credits-routing.module';
import { CreditsModule } from './credits/credits.module';
import { CreditsComponent } from './credits/credits/credits.component';
import { EasterEggRoutingModule } from './easter-egg/easter-egg-routing.module';
import { EasterEggModule } from './easter-egg/easter-egg.module';
import { GithubRoutingModule } from './github/github-routing.module';
import { GithubModule } from './github/github.module';
import { HomeComponent } from './home/home.component';
import { HomeModule } from './home/home.module';
import { HowToPlayModule } from './how-to-play/how-to-play.module';
import { HowToPlayComponent } from './how-to-play/how-to-play/how-to-play.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { UiModule } from './ui/ui.module';
import { UnitsComponent } from './units/units/units.component';



const appRoutes: Routes = [
  { path: 'home', component: HomeComponent },
  { path: 'units', component: UnitsComponent },
  { path: 'gallery', component: GalleryComponent },
  { path: 'how-to-play', component: HowToPlayComponent },
  { path: 'credits', component: CreditsComponent },
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: '**', component: PageNotFoundComponent }
];

@NgModule({
  declarations: [
    AppComponent,
    PageNotFoundComponent
  ],
  imports: [
    BrowserModule,
    UiModule,
    RouterModule.forRoot(appRoutes),
    HttpClientModule,
    EasterEggModule,
    EasterEggRoutingModule,
    GithubModule,
    GithubRoutingModule,
    MarkdownModule.forRoot(),
    CreditsModule,
    CreditsRoutingModule,
    HomeModule,
    HowToPlayModule,
    UnitsModule,
    GalleryModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
