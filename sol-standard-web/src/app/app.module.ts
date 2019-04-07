import { CreditsRoutingModule } from './credits/credits-routing.module';
import { CreditsModule } from './credits/credits.module';
import { GithubRoutingModule } from './github/github-routing.module';
import { GithubModule } from './github/github.module';
import { EasterEggModule } from './easter-egg/easter-egg.module';
import { EasterEggRoutingModule } from './easter-egg/easter-egg-routing.module';
import { FeaturesRoutingModule } from './features/features-routing.module';
import { FeaturesModule } from './features/features.module';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { UiModule } from './ui/ui.module';
import { HomeComponent } from './home/home.component';
import { MediaComponent } from './media/media.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { CarouselModule } from 'ngx-bootstrap/carousel';
import { MarkdownModule } from 'ngx-markdown';


const appRoutes: Routes = [
  { path: 'home', component: HomeComponent },
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'media', component: MediaComponent },
  { path: '**', component: PageNotFoundComponent }
];

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    MediaComponent,
    PageNotFoundComponent
  ],
  imports: [
    BrowserModule,
    UiModule,
    RouterModule.forRoot(appRoutes),
    HttpClientModule,
    CarouselModule.forRoot(),
    FeaturesModule,
    FeaturesRoutingModule,
    EasterEggModule,
    EasterEggRoutingModule,
    GithubModule,
    GithubRoutingModule,
    MarkdownModule.forRoot(),
    CreditsModule,
    CreditsRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
