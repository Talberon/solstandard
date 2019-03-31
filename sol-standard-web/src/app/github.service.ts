import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class GithubService {

  githubURL = 'https://api.github.com';
  ownerName = 'Talberon';
  repoName = 'solstandard';
  headers = new HttpHeaders({ Accept: 'application/vnd.github.v3.html' });

  constructor(private http: HttpClient) { }

  getReadme() {
    return this.http.get<string>(`${this.githubURL}/repos/${this.ownerName}/${this.repoName}/readme`,
      { headers: this.headers, responseType: 'text' }
    );
  }
}
