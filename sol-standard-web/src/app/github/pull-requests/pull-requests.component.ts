import { GithubService, PullRequest } from './../github.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-pull-requests',
  templateUrl: './pull-requests.component.html',
  styleUrls: ['./pull-requests.component.less']
})
export class PullRequestsComponent implements OnInit {

  pullRequests: PullRequest[];

  constructor(private githubService: GithubService) { }

  ngOnInit() {
    this.getPullRequests();
  }
  
  getPullRequests() {
    this.githubService.getPullRequests().subscribe(response => this.pullRequests = response.slice(0, 3));
  }

}
