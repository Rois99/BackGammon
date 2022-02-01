import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'max'
})
export class MaxPipe implements PipeTransform {

  transform(value: number, value2:number): number {
    return Math.max(value,value2);
  }

}
