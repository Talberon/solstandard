import { GithubService } from './../github.service';
import { Component, OnInit } from '@angular/core';
import { HttpResponse } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.less']
})
export class HomeComponent implements OnInit {

  readme: string;

  constructor(private githubService: GithubService) { }

  ngOnInit() {
    this.githubService.getReadme().subscribe(response => {
      console.log(response);
      this.readme = response;
    });
  }

}
