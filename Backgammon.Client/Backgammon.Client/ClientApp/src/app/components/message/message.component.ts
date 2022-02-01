import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-message',
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.css']
})
export class MessageComponent implements OnInit {

  @Input() isUser:boolean = false;
  @Input() text?:string;
  @Input() sentAt?:Date;
  constructor() { }

  ngOnInit(): void {
  }

}
