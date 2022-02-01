import { Injectable } from '@angular/core';

interface Point{
  left:number,
  top:number
}


@Injectable({
  providedIn: 'root'
})
export class AnimatePiecesService {

  private readonly BASE_PIECE_SPEED = 1.15;

  constructor() { 
    
  }

  async movePiece(player:string,from:number,to:number){
    let { fromPice, toPice, board, pice } = this.getElements(from, to,player);

    if(!fromPice || !toPice || !board || !pice) return;

    let moveFrom = this.getCurrentPosTo(fromPice,board,this.isBottom(from));
    let moveTo = this.getDestPosTo(toPice,board,this.isBottom(to));
    let boardSize = board.clientWidth;
    let time = this.getSpeedMs(moveFrom,moveTo,boardSize);

    let animation = this.animate(pice,moveFrom,moveTo,time);

    await animation.finished;
  }

  async removePiece(player:string,from:number){
    let  { fromPice, board, pice } = this.getElementsNoTo(from,player);

    if(!fromPice || !board || !pice) return;

    let moveFrom = this.getCurrentPosTo(fromPice,board,this.isBottom(from));

    let moveTo = this.getOutOfBoardPos(board);

    let boardSize = board.clientWidth;
    let time = this.getSpeedMs(moveFrom,moveTo,boardSize);

    let animation = this.animate(pice,moveFrom,moveTo,time);

    await animation.finished;

  }

  private isBottom(i:number){
    if(i==0)
      return false;
    if(i == 25)
      return true;
    if(i<13)
      return false;
    return true;
  }

  private animate(pice:HTMLElement,moveFrom:Point,moveTo:Point,time:number){
    return pice.animate([
      {transform:`translate(${moveFrom.left}px,${moveFrom.top}px)`,visibility:'visible'},
      {transform:`translate(${moveTo.left}px,${moveTo.top}px)`,visibility:'hidden'}
    ],{
      duration:time,
      iterations:1,
      easing:'ease-in'
    });
  }

  private getElements(from: number, to: number,player:string) {
    let pice = player == 'black' ? document.getElementById('moveable-black') : document.getElementById('moveable-white');
    let board = document.getElementById('game-board');
    let stackFrom = this.getStack(from);
    let stackTo = this.getStack(to);
    let fromPice = this.getLastPieceInStack(stackFrom,this.isBottom(from));
    let toPice = this.getLastPieceInStack(stackTo,this.isBottom(to));
    return { fromPice, toPice, board, pice };
  }

  private getElementsNoTo(from: number,player:string) {
    let pice = player == 'black' ? document.getElementById('moveable-black') : document.getElementById('moveable-white');
    let board = document.getElementById('game-board');
    let stackFrom = this.getStack(from);
    let fromPice = this.getLastPieceInStack(stackFrom,this.isBottom(from));
    return { fromPice, board, pice };
  }

  private getSpeedMs(point1:Point,point2:Point,boardSize:number):number{
    let dis = Math.sqrt(Math.pow((point1.left - point2.left),2) + Math.pow((point1.top - point2.top),2));
    let acl = this.BASE_PIECE_SPEED*boardSize;
    return (dis/acl)*1000;
  }

  private getLastPieceInStack(stack:HTMLElement|null,isBottom:boolean){
    let children = this.getDivs(stack?.children);
    if(children.length == 0)
      return null;
    let element:HTMLElement;
      if(isBottom)
      element = this.getFirstPieceInDiv(children[Math.max(0,children.length-1)]) as HTMLElement;
      else
      element = this.getLastPieceInDiv(children[Math.max(0,children.length-1)]) as HTMLElement;;
      return element
  }

  private getDivs(collection:HTMLCollection|undefined) : Element[]
  {
    if(collection)
      return Array.from(collection).filter(e=>e.tagName == 'DIV');
    return [];
  }

  private getFirstPieceInDiv(div:Element|null){
    let children = div?.children;
    if(!children || children.length == 0 || children.length == 6)
      return div;
    return children[0];
  }

  private getLastPieceInDiv(div:Element|null){
    let children = div?.children;
    if(!children || children.length == 0 || children.length == 6)
      return div;
    return children[Math.max(0,children.length-1)];
  }

  private getCurrentPosTo(element:HTMLElement,toElement:HTMLElement,isBottom:boolean)
  {
    let top:number;
    if(isBottom)
      top = (element.getBoundingClientRect().top+element.clientHeight-element.clientWidth) - toElement.getBoundingClientRect().top;
    else
      top = element.getBoundingClientRect().top - toElement.getBoundingClientRect().top;
    let left = element.getBoundingClientRect().left-toElement.getBoundingClientRect().left;
    return {top,left}
  }

  private getDestPosTo(element:HTMLElement,toElement:HTMLElement,isBottom:boolean)
  {
    let {top,left} = this.getRelative(element,toElement);

    if(element.tagName == 'DIV')
    {
      left = left + ((element.clientWidth -(0.9*element.clientWidth))/2);
      if(isBottom)
        top = top + element.clientHeight - element.clientWidth;
    }else
    {
      left = left + (((1.1*element.clientWidth) -element.clientWidth)/2);
      if(!isBottom)
        top = top + element.clientHeight;
      else
        top = top - element.clientHeight;
    }
      return {top,left}
  }

  private getRelative(element:HTMLElement,toElement:HTMLElement)
  {
    let left = element.getBoundingClientRect().left-toElement.getBoundingClientRect().left;
    let top = element.getBoundingClientRect().top - toElement.getBoundingClientRect().top;
    return {top,left};
  }

  private getStack(i:number){

      let id = `stack-${i}`;
      let stack = document.getElementById(id);
    return stack;
  }

  private getOutOfBoardPos(board:HTMLElement)
  {
    let width = board.clientWidth;
    let height = board.clientHeight;
    let top = height/2;
    let left = width + (width*0.075);
    return {top,left};
  }

}
