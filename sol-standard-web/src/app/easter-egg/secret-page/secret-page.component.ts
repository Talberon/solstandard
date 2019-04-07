import { Component, OnInit } from '@angular/core';
import {
  trigger,
  style,
  animate,
  transition,
  keyframes
} from '@angular/animations';

@Component({
  selector: 'app-secret-page',
  templateUrl: './secret-page.component.html',
  styleUrls: ['./secret-page.component.less'],
  animations: [
    trigger('colourStatus', [
      transition(':enter', [
        animate('5s', keyframes([
          style({ color: 'red' }),
          style({ color: 'white' }),
          style({ color: 'rgb(80,80,255)' }),
          style({ color: 'white' }),
          style({ color: 'red' }),
          style({ color: 'white' })
        ]))
      ]
      )
    ])
  ]
})
export class SecretPageComponent implements OnInit {

  activated: boolean;

  constructor() { }

  ngOnInit() {
  }

  toggleActivated() {
    if (!this.activated) {
      this.activated = true;
    } else {
      this.activated = !this.activated;
    }

    return this.activated;
  }

}
