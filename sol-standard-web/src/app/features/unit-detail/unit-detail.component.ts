import { Component, OnInit } from '@angular/core';
import { Unit, Team } from 'src/app/model/unit';
import { UnitService } from '../unit.service';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-unit-detail',
  templateUrl: './unit-detail.component.html',
  styleUrls: ['./unit-detail.component.less'],
})
export class UnitDetailComponent implements OnInit {

  unit: Unit;

  constructor(private unitService: UnitService, private route: ActivatedRoute, private router: Router) { }

  ngOnInit() {
    this.route.paramMap.pipe(
      switchMap((params: ParamMap) =>
        this.unitService.getUnit(parseFloat(params.get('id'))))
    ).subscribe(unit => this.unit = unit);
  }

  getBluePortrait(unit: Unit): string {
    return unit.getPortrait(Team.Blue);
  }

  getRedPortrait(unit: Unit): string {
    return unit.getPortrait(Team.Red);
  }
}
