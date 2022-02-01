import { Chatter } from "./Chatter";
import { TwoNums } from "./TwoNums";

export interface StartGame{
    gameId:string,
    player:Chatter,
    playerColor:string,
    isStarting:boolean,
    whoIsFirstRoll:TwoNums,
    firstRoll:TwoNums,
    opponent:Chatter
}