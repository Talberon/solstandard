import { MarkdownModule } from 'ngx-markdown';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PullRequestsComponent } from './pull-requests/pull-requests.component';
import { DownloadComponent } from './download/download.component';
import { GithubRoutingModule } from './github-routing.module';

@NgModule({
  declarations: [PullRequestsComponent, DownloadComponent],
  imports: [
    CommonModule,
    GithubRoutingModule,
    MarkdownModule.forChild()
  ]
})
export class GithubModule { }
