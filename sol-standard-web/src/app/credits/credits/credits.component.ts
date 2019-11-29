import { GithubService } from 'src/app/github/github.service';
import { Component, OnInit } from '@angular/core';
import { BannerSize } from 'src/app/model/banner-size';

@Component({
  selector: 'app-credits',
  templateUrl: './credits.component.html',
  styleUrls: ['./credits.component.less']
})
export class CreditsComponent implements OnInit {

  credits: string;
  bannerSize = BannerSize.MEDIUM;

  constructor(private githubService: GithubService) { }

  ngOnInit() {
    this.githubService.getCredits().subscribe(response => this.credits = response);
  }

}
