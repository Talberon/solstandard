import { GameTileService } from './../game-tile.service';
import { Component, OnInit } from '@angular/core';
import { GameTile } from 'src/app/model/game-tile';

@Component({
  selector: 'app-tiles',
  templateUrl: './tiles.component.html',
  styleUrls: ['./tiles.component.less']
})
export class TilesComponent implements OnInit {

  gameTiles: GameTile[];

  constructor(private gameTileService: GameTileService) { }

  ngOnInit() {
    this.getTiles();
  }

  getTiles() {
    this.gameTileService.getTiles().subscribe(tiles => this.gameTiles = tiles);
  }

}
