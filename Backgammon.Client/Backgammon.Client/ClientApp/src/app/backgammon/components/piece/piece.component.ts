import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-piece',
  templateUrl: './piece.component.html',
  styleUrls: ['./piece.component.css']
})
export class PieceComponent implements OnInit {

  @Input() stackNumber:number = 0;

  @Input() isWhite:boolean = true;

  @Input() isMoveable:boolean = false;

  constructor() { }

  ngOnInit(): void {
  }

}
