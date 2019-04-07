import { ReleaseInfo } from './github.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { shareReplay, map } from 'rxjs/operators';

export interface PullRequest {
  title: string;
  body: string;
  merged_at: string;
}

export interface ReleaseInfo {
  name: string;
  body_html: string;
  html_url: string;
}

@Injectable({
  providedIn: 'root'
})
export class GithubService {

  githubURL = 'https://api.github.com';
  ownerName = 'Talberon';
  repoName = 'solstandard';
  markdownHeaders = new HttpHeaders({ Accept: 'application/vnd.github.v3.html' });

  readmeCache: Observable<string>;
  pullRequestsCache: Observable<PullRequest[]>;
  creditsCache: Observable<string>;
  releaseCache: Observable<ReleaseInfo>;

  constructor(private http: HttpClient) { }

  getReadme(): Observable<string> {
    if (!this.readmeCache) {
      this.readmeCache = this.http.get(`${this.githubURL}/repos/${this.ownerName}/${this.repoName}/readme`,
        { headers: this.markdownHeaders, responseType: 'text' }
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

  getCredits(): Observable<string> {
    if (!this.creditsCache) {
      this.creditsCache = this.http.get(`${this.githubURL}/repos/${this.ownerName}/${this.repoName}/contents/CREDITS.md`,
        { headers: this.markdownHeaders, responseType: 'text' }
      ).pipe(
        shareReplay()
      );
    }

    return this.creditsCache;
  }

  getLatestRelease(): Observable<ReleaseInfo> {
    if (!this.releaseCache) {
      this.releaseCache =
        this.http.get(`${this.githubURL}/repos/${this.ownerName}/${this.repoName}/releases/latest`,
          { headers: this.markdownHeaders }
        ).pipe(shareReplay()) as Observable<ReleaseInfo>;
    }

    return this.releaseCache;
  }
}
