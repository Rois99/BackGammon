import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'enumerate'
})
export class EnumeratePipe implements PipeTransform {

  transform(n: number,from:number|undefined = undefined,to:number|undefined=undefined,reverse:boolean=false): number[] {
    let r_n = Math.floor(n);
    let r_f = from? Math.floor(from):undefined;
    let r_t = to? Math.floor(to):undefined;
    if(r_n <= 0)
      return [];
    if(reverse)
      return[...Array(r_n)].map((_,i) => i).slice(r_f,r_t).reverse();
    return [...Array(r_n)].map((_,i) => i).slice(r_f,r_t);
  }

}
