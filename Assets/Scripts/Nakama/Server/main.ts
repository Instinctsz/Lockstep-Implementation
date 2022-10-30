const defaultMatchName = "amogus";
const systemId = "00000000-0000-0000-0000-000000000000";

const opCodes = {
	Start_Match: 1,
	Position: 2,
	Attack: 3,
	Create_Unit: 4,
    Turn_Timer_Tick: 5,
    Rollback_Request : 6
}

const InitModule: nkruntime.InitModule = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, initializer: nkruntime.Initializer) {
    initializer.registerRpc("CreateMatch", rpcCreateMatch);
    initializer.registerRpc("GetMatchByName", rpcGetMatchIdByName);
    initializer.registerRpc("GetReplayData", rpcGetReplayData);

    initializer.registerMatch(defaultMatchName, {
        matchInit,
        matchJoinAttempt,
        matchJoin,
        matchLeave,
        matchLoop,
        matchSignal,
        matchTerminate
    });

    logger.debug("Registered match.");
}

function rpcCreateMatch(context: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): string {
    const matchId = nk.matchCreate(defaultMatchName, { payload });
    logger.info("Created match.");

    const jsonPayload = JSON.parse(payload.toString());

    const matchNamePayload = jsonPayload["MatchName"];

    let writeRequest: nkruntime.StorageWriteRequest[] = [{ collection: 'matches', key: matchNamePayload, userId: systemId, value: { matchId } }];

    try {
        nk.storageWrite(writeRequest);
        logger.debug("Write succcesful");
        logger.debug("Wrote name: " + matchNamePayload + " with: " + matchId);
    } catch (error) {
        logger.error(error);
    }

    return JSON.stringify({ matchId });
}

function rpcGetMatchIdByName(context: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): string {
    const jsonPayload = JSON.parse(payload.toString());

    const matchNamePayload = jsonPayload["MatchName"];

    let readRequest: nkruntime.StorageReadRequest[] = [{ collection: 'matches', key: matchNamePayload, userId: systemId }];
    let results: nkruntime.StorageObject[] = [];

    try {
        results = nk.storageRead(readRequest);
        logger.debug("Reading name: " + matchNamePayload)
    } catch (error) {
        logger.error(error);
    }

    const matchId = results[0].value.matchId;

    return JSON.stringify({ matchId });
}

function rpcGetReplayData(context: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): string {
    const jsonPayload = JSON.parse(payload.toString());

    const matchIdPayload = jsonPayload["matchId"];

    let result: nkruntime.StorageObjectList = {};

    try {
        result = nk.storageList(systemId, matchIdPayload, 10000);
    } catch (error) {
        logger.error("ERROR AT GETTING REPLAY DATA");
    }

    let objects = result.objects;

    return JSON.stringify({ objects });
}