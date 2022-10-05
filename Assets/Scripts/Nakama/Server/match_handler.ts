// Match initialization function (runs once when the match is created either via rpc or through the matchmaker)
const matchInit: nkruntime.MatchInitFunction<nkruntime.MatchState> = function(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, params: {[key: string]: string}) : {state: nkruntime.MatchState, tickRate: number, label: string} {
    logger.debug('Match initialized.');
    return {
            state: { },
            tickRate: 1,
            label: ''
    };
};

// When a player tries to join
const matchJoinAttempt: nkruntime.MatchJoinAttemptFunction<nkruntime.MatchState> = function(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, presences: nkruntime.Presence, metadata : {[key : string]: any}) : {state: nkruntime.MatchState, accept: boolean } | null {        
    return {
        state,
        accept: true
    };
};

// When one (or multiple) player(s) actually join(s)
const matchJoin: nkruntime.MatchJoinFunction<nkruntime.MatchState> = function(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, presences: nkruntime.Presence[]) : {state: nkruntime.MatchState} | null {
    return {state};
};

// When a player leaves
const matchLeave: nkruntime.MatchLeaveFunction<nkruntime.MatchState> = function(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, presences: nkruntime.Presence[]) : {state: nkruntime.MatchState} | null {
    return {state};
};

// Runs every tick
const matchLoop: nkruntime.MatchLoopFunction<nkruntime.MatchState> = function(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, messages: nkruntime.MatchMessage[]) : {state: nkruntime.MatchState} | null {
    return {state};
};

// Runs when the match gets terminated
const matchTerminate: nkruntime.MatchTerminateFunction<nkruntime.MatchState> = function(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, graceSeconds: number) : {state: nkruntime.MatchState} | null {
    return {state};
};

const matchSignal: nkruntime.MatchSignalFunction<nkruntime.MatchState> = function(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, data: string) : {state: nkruntime.MatchState, data?: string} | null {
    logger.debug('Lobby match signal recieved: ' + data);
    return {state, data};
};