import { Component, OnInit } from '@angular/core';
import { BannerSize } from 'src/app/model/banner-size';
import { GithubService } from 'src/app/github/github.service';

@Component({
  selector: 'app-how-to-play',
  templateUrl: './how-to-play.component.html',
  styleUrls: ['./how-to-play.component.less']
})
export class HowToPlayComponent implements OnInit {

  howToPlay: string;
  bannerSize = BannerSize.MEDIUM;

  constructor(private githubService: GithubService) { }

  ngOnInit() {
    this.githubService.getHowToPlay().subscribe(response => this.howToPlay = response);
  }

}
