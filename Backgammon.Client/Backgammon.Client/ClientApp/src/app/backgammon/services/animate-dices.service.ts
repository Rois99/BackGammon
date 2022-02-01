import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AnimateDicesService {

  private readonly ANIMATION_TIME = 2000;
  constructor() {
   }

  async animate(dice1:number,dice2:number){

    let dice1Element = document.getElementById("dice1");
    let dice2Element = document.getElementById("dice2");

    if(!dice1Element|| !dice2Element) return;

    let seq1 = this.getRandomSeq();
    let seq2 = this.getRandomSeq();

    this.switchNumbers(seq1,dice1);
    this.switchNumbers(seq2,dice2);

    let a1 = this.getAnimation(dice1Element,seq1);
    let a2 = this.getAnimation(dice2Element,seq2);
    await a1.finished;
    await a2.finished;
  }

  private switchNumbers(seq:number[],dice:number){
    let pos = seq.indexOf(dice);
    let endNum = seq[5];
    seq[5] = seq[pos];
    seq[pos] = endNum;
  }

  private getRandomSeq(){
    return Array(6).fill(0).map((_,i)=>i+1).sort(()=>Math.random()-0.5);
  }

  private getAnimation(dice:HTMLElement,sequence:number[]){
    return dice.animate([
      {visibility:'hidden'},
      {visibility:'hidden'},
      {visibility:'hidden'},
      {visibility:'hidden'},
      {visibility:'hidden'},
      {visibility:'hidden'},
      {visibility:'hidden'},
      {visibility:'hidden'},
      {backgroundImage:`url(dice-face-${sequence[0]}.svg)`,visibility:'visible'},
      {backgroundImage:`url(dice-face-${sequence[1]}.svg)`,visibility:'visible'},
      {backgroundImage:`url(dice-face-${sequence[2]}.svg)`,visibility:'visible'},
      {backgroundImage:`url(dice-face-${sequence[3]}.svg)`,visibility:'visible'},
      {backgroundImage:`url(dice-face-${sequence[4]}.svg)`,visibility:'visible'},
      {backgroundImage:`url(dice-face-${sequence[5]}.svg)`,visibility:'visible'},
    ],{
      duration:this.ANIMATION_TIME,
      iterations:1,
      easing:'linear'
    });
  }

}
