import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { shareReplay } from 'rxjs/operators';

export interface PullRequest {
  title: string;
  body: string;
}


@Injectable({
  providedIn: 'root'
})
export class GithubService {

  githubURL = 'https://api.github.com';
  ownerName = 'Talberon';
  repoName = 'solstandard';
  headers = new HttpHeaders({ Accept: 'application/vnd.github.v3.html' });

  readmeCache: Observable<string>;
  pullRequestsCache: Observable<PullRequest[]>;

  constructor(private http: HttpClient) { }

  getReadme(): Observable<string> {
    if (!this.readmeCache) {
      this.readmeCache = this.http.get(`${this.githubURL}/repos/${this.ownerName}/${this.repoName}/readme`,
        { headers: this.headers, responseType: 'text' }
      ).pipe(
        shareReplay()
      );
    }

    return this.readmeCache;
  }

  getPullRequests(): Observable<PullRequest[]> {
    if (!this.pullRequestsCache) {
      this.pullRequestsCache = this.http.get(
        `${this.githubURL}/repos/${this.ownerName}/${this.repoName}/pulls?state=closed$base=develop`
      ).pipe(
        shareReplay()
      ) as Observable<PullRequest[]>;
    }

    return this.pullRequestsCache;
  }
}
