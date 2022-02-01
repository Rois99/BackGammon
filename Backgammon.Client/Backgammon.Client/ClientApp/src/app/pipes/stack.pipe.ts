import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'stack'
})
export class StackPipe implements PipeTransform {

  transform(numberOfPieces: number,maxInStack: number): number {
    if(numberOfPieces == 0) return 1;
    if(numberOfPieces%maxInStack == 0) return numberOfPieces/maxInStack;
    return (numberOfPieces/maxInStack) + 1;
  }

}
