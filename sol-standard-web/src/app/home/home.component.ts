import { GithubService } from '../github/github.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.less']
})
export class HomeComponent implements OnInit {

  readme: string;

  constructor(private githubService: GithubService) { }

  ngOnInit() {
    // this.githubService.getReadme().subscribe(response => this.readme = response);
  }

}
