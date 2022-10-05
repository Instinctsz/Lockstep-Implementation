const matchName: string = "amogus";
let matchLinks = {

}

const InitModule: nkruntime.InitModule = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, initializer: nkruntime.Initializer) {
    logger.debug("Hello World!!@!!");

    initializer.registerRpc("CreateMatch", rpcCreateMatch);

    initializer.registerMatch(matchName, {
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
    const matchId = nk.matchCreate(matchName, { payload });
    logger.info(payload);
    const jsonPayload = JSON.parse(payload);
    logger.info(jsonPayload.msg);

    return JSON.stringify({ matchId });
}
