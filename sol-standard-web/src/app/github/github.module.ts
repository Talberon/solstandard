import { UiModule } from './../ui/ui.module';
import { MarkdownModule } from 'ngx-markdown';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PullRequestsComponent } from './pull-requests/pull-requests.component';
import { DownloadComponent } from './download/download.component';
import { GithubRoutingModule } from './github-routing.module';
import { ReleaseComponent } from './release/release.component';

@NgModule({
  declarations: [PullRequestsComponent, DownloadComponent, ReleaseComponent],
  imports: [
    CommonModule,
    GithubRoutingModule,
    MarkdownModule.forChild(),
    UiModule
  ]
})
export class GithubModule { }
