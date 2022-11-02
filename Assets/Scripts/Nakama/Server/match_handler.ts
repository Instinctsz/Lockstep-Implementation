// Match initialization function (runs once when the match is created either via rpc or through the matchmaker)
const matchInit: nkruntime.MatchInitFunction<nkruntime.MatchState> = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, params: { [key: string]: string }): { state: nkruntime.MatchState, tickRate: number, label: string } {
    logger.debug('Match initialized.');
    return {
        state: {},
        tickRate: 10,
        label: ''
    };
};

// When a player tries to join
const matchJoinAttempt: nkruntime.MatchJoinAttemptFunction<nkruntime.MatchState> = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, presences: nkruntime.Presence, metadata: { [key: string]: any }): { state: nkruntime.MatchState, accept: boolean } | null {
    logger.debug('Player tries to join!');
    return {
        state,
        accept: true
    };
};

// When one (or multiple) player(s) actually join(s)
const matchJoin: nkruntime.MatchJoinFunction<nkruntime.MatchState> = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, presences: nkruntime.Presence[]): { state: nkruntime.MatchState } | null {
    logger.debug("Player joined");
    logger.debug(presences[0].username);
    return { state };
};

// When a player leaves
const matchLeave: nkruntime.MatchLeaveFunction<nkruntime.MatchState> = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, presences: nkruntime.Presence[]): { state: nkruntime.MatchState } | null {
    logger.debug("Match leave, deleting collections");
    deleteCollection("matches", nk, logger);

    return null;
    return { state };
};

// Runs every tick
const matchLoop: nkruntime.MatchLoopFunction<nkruntime.MatchState> = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, messages: nkruntime.MatchMessage[]): { state: nkruntime.MatchState } | null {
    let currentTick = tick.toString();
    dispatcher.broadcastMessage(opCodes.Turn_Timer_Tick, nk.stringToBinary(currentTick), null);

    //Handle queueing current tick
    let readRequest: nkruntime.StorageReadRequest[] = [{ collection: ctx.matchId, key: tick.toString(), userId: systemId }];
    let results: nkruntime.StorageObject[] = [];

    try {
        results = nk.storageRead(readRequest);
    } catch (error) {
        console.error(error);
    }

    for (const result of results) {
        for (const packet of result.value.packets) {
            dispatcher.broadcastMessage(packet.opCode, packet.data, null, packet.sender);
            logger.debug("Sending packet which is from " + packet.sender.username);
        }
    }

    if (messages.length == 0)
        return { state };

    // Handle queueing messages to future tick
    let writeRequestData = [];
    let tickToQueueTo = tick + 1;

    for (const message of messages) {
        let dataString = "";
        
        if (message.data.byteLength > 0)
            dataString = nk.binaryToString(message.data);

        let parsedData = JSON.parse(dataString);
        tickToQueueTo = parseInt(parsedData.TickToQueueTo);
        
        if (isNaN(tickToQueueTo))
            tickToQueueTo = tick + 1;
        
        // Rollback
        if (tickToQueueTo <= tick) {
            parsedData.tickToQueueTo = tickToQueueTo;
            logger.error("PACKET ARRIVED FOR TICK: " + tickToQueueTo + ", BUT ITS TICK: " + tick);
            dispatcher.broadcastMessage(opCodes.Rollback_Request, JSON.stringify(parsedData), null, message.sender);
            logger.error("Sent roll back request to tick: " + tickToQueueTo.toString());
            dispatcher.broadcastMessage(message.opCode, message.data, null, message.sender);
            continue;
        }

        let value = { opCode: message.opCode, sender: message.sender, data: dataString }
        writeRequestData.push(value);
    }

    let messagesToBeQueued: nkruntime.StorageWriteRequest[] = [{ collection: ctx.matchId, key: tickToQueueTo.toString(), userId: systemId, value: { packets: writeRequestData } }];

    try {
        nk.storageWrite(messagesToBeQueued);
        logger.debug("Write to queue succcesful");
    } catch (error) {
        logger.error(error);
    }

    return { state };
};

// Runs when the match gets terminated
const matchTerminate: nkruntime.MatchTerminateFunction<nkruntime.MatchState> = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, graceSeconds: number): { state: nkruntime.MatchState } | null {
    return { state };
};

const matchSignal: nkruntime.MatchSignalFunction<nkruntime.MatchState> = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, data: string): { state: nkruntime.MatchState, data?: string } | null {
    logger.debug('Lobby match signal recieved: ' + data);
    return { state, data };
};

const deleteCollection = function (name: string, nk: nkruntime.Nakama, logger: nkruntime.Logger) : void {
    let results: nkruntime.StorageObjectList = {};

    try {
        results = nk.storageList(systemId, name);
    } catch (error) {
        // Handle error
    }

    let deleteObjects: nkruntime.StorageDeleteRequest[] = [];

    for (const result of results.objects) {
        logger.debug("Deleting: " + result.collection + ", key: " + result.key);
        deleteObjects.push({ collection: result.collection, key: result.key, userId: systemId })
    }

    try {
        nk.storageDelete(deleteObjects);
    } catch (error) {

    }
    


}