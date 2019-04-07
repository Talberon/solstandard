import { GithubService, ReleaseInfo } from './../github.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-release',
  templateUrl: './release.component.html',
  styleUrls: ['./release.component.less']
})
export class ReleaseComponent implements OnInit {

  releaseInfo: ReleaseInfo;

  constructor(private githubService: GithubService) { }

  ngOnInit() {
    this.getReleaseInfo();
  }

  getReleaseInfo() {
    this.githubService.getLatestRelease().subscribe(response => this.releaseInfo = response);
  }

}
