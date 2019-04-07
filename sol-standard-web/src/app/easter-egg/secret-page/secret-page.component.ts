import { Component, OnInit } from '@angular/core';
import {
  trigger,
  state,
  style,
  animate,
  transition,
  keyframes
  // ...
} from '@angular/animations';

@Component({
  selector: 'app-secret-page',
  templateUrl: './secret-page.component.html',
  styleUrls: ['./secret-page.component.less'],
  animations: [
    trigger('colourStatus', [
      state('true', style({
        color: 'red'
      })),
      state('false', style({
        color: 'rgb(80,80,255)'
      })),
      transition('red <=> blue', [
        animate('2s', keyframes([
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
