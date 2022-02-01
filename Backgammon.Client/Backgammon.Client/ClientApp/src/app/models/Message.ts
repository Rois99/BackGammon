export interface Message{
    id:string,
    senderId:string,
    recipientId:string,
    messageBody:string,
    isReceived:boolean,
    sentAt:Date,
    receivedAd:Date
}