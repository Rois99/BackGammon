import { Env } from "./environment";

export const environment : Env = {
  production: false,
  api:{
    identity:'http://localhost:5000/api',
    chat:'http://localhost:5050/api',
    game:'http://localhost:6060/api/game'

  },
  hubs:{
    chat:"http://localhost:5050/hubs/chat",
    game:"http://localhost:6060/game"
  }
};
