let matchInit: nkruntime.MatchInitFunction = function (context: nkruntime.Context, logger: nkruntime.Logger, nakama: nkruntime.Nakama, params: { [key: string]: string })
{ 
    logger.debug('Lobby match created');
  
	return {
	  state: { Debug: true },
	  tickRate: 5,
	  label: ""
	};
  };


  let matchLoop: nkruntime.MatchLoopFunction = function (context: nkruntime.Context, logger: nkruntime.Logger, nakama: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, messages: nkruntime.MatchMessage[])
  { 
    logger.debug('Lobby match loop executed');
  
	// const opCode = 1234;
	// const message = JSON.stringify({ hello: 'world' });
	// const presences = null; // Send to all.
	// const sender = null; // Used if a message should come from a specific user.
	// dispatcher.broadcastMessage(opCode, message, presences, sender, true);


    // const currentTick: {[tickstring: string]: number} = {"Tick": tick};
  
    // logger.debug("Current tick:");
    // logger.debug(currentTick.Tick.toString());

	// return {
	//   state: {currentTick}
	// };
    return {
        state
    }
  }
  
  let matchJoin: nkruntime.MatchJoinFunction = function (context: nkruntime.Context, logger: nkruntime.Logger, nakama: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, presences: nkruntime.Presence[])
  { 
    //   presences.forEach(function (p) { 
    //   state.presences[p.sessionId] = p;
    // });
  
    return {
      state
    };
  }
  
  let matchLeave: nkruntime.MatchLeaveFunction = function (context: nkruntime.Context, logger: nkruntime.Logger, nakama: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, presences: nkruntime.Presence[])
  { 
    // presences.forEach(function (p) {
    //   delete(state.presences[p.sessionId]);
    // });
  
    return {
      state
    };
  }
  

  let matchJoinAttempt: nkruntime.MatchJoinAttemptFunction = function (context: nkruntime.Context, logger: nkruntime.Logger, nakama: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, presence: nkruntime.Presence, metadata: { [key: string]: any })
  { 
      logger.debug('%q attempted to join Lobby match', context.userId);
  
	return {
	  state,
	  accept: true
	};
  }

  let matchSignal: nkruntime.MatchSignalFunction = function (context: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, data: string)
  { 
    logger.debug('Lobby match signal received: ' + data);
  
	return {
	  state,
	  data: "Lobby match signal received: " + data
	};
  }

  let matchTerminate: nkruntime.MatchTerminateFunction = function (context: nkruntime.Context, logger: nkruntime.Logger, nakama: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, graceSeconds: number)
  {
    logger.debug('Lobby match terminated');
  
	return {
	  state
	};
  }