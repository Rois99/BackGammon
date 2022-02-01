import { TwoNums } from "../models/TwoNums";

export interface FirstMove{
    gameId:string,
    playerOne:string,
    playerTwo:string,
    whosFirstCubes:TwoNums,
    playingCubes:TwoNums
}