import { TwoNums } from "../models/TwoNums";
import { Move } from "./Move";

export interface LastMove{
    opponentMove:Move,
    yourDices:TwoNums
}