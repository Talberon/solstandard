import { TestBed } from '@angular/core/testing';

import { GameTileService } from './game-tile.service';

describe('GameTileService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: GameTileService = TestBed.get(GameTileService);
    expect(service).toBeTruthy();
  });
});
