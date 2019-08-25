import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { CarouselModule } from 'ngx-bootstrap/carousel';
import { MarkdownModule } from 'ngx-markdown';
import { AppComponent } from './app.component';
import { CreditsRoutingModule } from './credits/credits-routing.module';
import { CreditsModule } from './credits/credits.module';
import { EasterEggRoutingModule } from './easter-egg/easter-egg-routing.module';
import { EasterEggModule } from './easter-egg/easter-egg.module';
import { GithubRoutingModule } from './github/github-routing.module';
import { GithubModule } from './github/github.module';
import { HomeComponent } from './home/home.component';
import { HomeModule } from './home/home.module';
import { MediaComponent } from './media/media.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { UiModule } from './ui/ui.module';



const appRoutes: Routes = [
  { path: 'home', component: HomeComponent },
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'media', component: MediaComponent },
  { path: '**', component: PageNotFoundComponent }
];

@NgModule({
  declarations: [
    AppComponent,
    MediaComponent,
    PageNotFoundComponent
  ],
  imports: [
    BrowserModule,
    UiModule,
    RouterModule.forRoot(appRoutes),
    HttpClientModule,
    CarouselModule.forRoot(),
    EasterEggModule,
    EasterEggRoutingModule,
    GithubModule,
    GithubRoutingModule,
    MarkdownModule.forRoot(),
    CreditsModule,
    CreditsRoutingModule,
    HomeModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
