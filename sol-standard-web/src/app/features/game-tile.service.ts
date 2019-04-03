import { Injectable } from '@angular/core';
import { GameTile } from '../model/game-tile';
import { TILE_LIST } from '../model/game-tile-list';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GameTileService {

  constructor() { }

  getTiles(): Observable<GameTile[]> {
    return of(TILE_LIST);
  }
}
