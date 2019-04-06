import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/auth/auth.service';

@Component({
  selector: 'app-egg',
  templateUrl: './egg.component.html',
  styleUrls: ['./egg.component.less']
})
export class EggComponent implements OnInit {

  constructor(private authService: AuthService) { }

  ngOnInit() {
  }

  activateSecret() {
    console.log('Activating secret page!');
    this.authService.activateSecret().subscribe(value => console.log(`AuthService replied with ${value}`));
  }

}
