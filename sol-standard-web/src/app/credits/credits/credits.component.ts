import { GithubService } from './../../github/github.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-credits',
  templateUrl: './credits.component.html',
  styleUrls: ['./credits.component.less']
})
export class CreditsComponent implements OnInit {

  credits: string;

  constructor(private githubService: GithubService) { }

  ngOnInit() {
    this.githubService.getCredits().subscribe(response => this.credits = response);
  }

}
