import { BannerSize } from './../../model/banner-size';
import { Component, OnInit, Input } from '@angular/core';
import { ReleaseInfo, GithubService } from 'src/app/github/github.service';

@Component({
  selector: 'app-title-banner',
  templateUrl: './title-banner.component.html',
  styleUrls: ['./title-banner.component.less']
})
export class TitleBannerComponent implements OnInit {

  @Input() bannerSize: BannerSize;
  releaseInfo: ReleaseInfo;

  readonly bannerSizes = {
    [BannerSize.SMALL]: '100px',
    [BannerSize.MEDIUM]: '200px',
    [BannerSize.LARGE]: '300px'
  };

  constructor(private githubService: GithubService) { }

  ngOnInit() {
    this.getReleaseInfo();
  }

  getReleaseInfo() {
    this.githubService.getLatestRelease().subscribe(response => this.releaseInfo = response);
  }
}
