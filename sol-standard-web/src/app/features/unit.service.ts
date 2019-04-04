import { Injectable } from '@angular/core';
import { Unit } from '../model/unit';
import { UNITS } from '../model/unit-list';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UnitService {

  constructor() { }

  getUnits(): Observable<Unit[]> {
    return of(UNITS);
  }

  getUnit(id: number) {
    return of(UNITS.find(unit => unit.id === id));
  }
}
