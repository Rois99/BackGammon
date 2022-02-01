import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SoundService {

  private moveAudio = new Audio();
  private rollAudio = new Audio();

  constructor() {
    this.moveAudio.src = 'assets/sounds/piece-nock.wav';
    this.moveAudio.load();

    this.rollAudio.src = 'assets/sounds/dice-roll.wav';
    this.rollAudio.load();
   }

  playPieceMove(){
    this.moveAudio.play();
  }

  playDiceRoll(){
    this.rollAudio.play();
  }

}
