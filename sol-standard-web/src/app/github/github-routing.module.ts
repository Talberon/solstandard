import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DownloadComponent } from './download/download.component';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: 'download',
    component: DownloadComponent
  }
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ]
})
export class GithubRoutingModule { }
