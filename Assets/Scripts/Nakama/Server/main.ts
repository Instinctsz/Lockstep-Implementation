const InitModule: nkruntime.InitModule = function(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, initializer: nkruntime.Initializer) {
    logger.debug("Hello World!!@!!");

    initializer.registerMatch('match', {
        matchInit,
        matchJoinAttempt,
        matchJoin,
        matchLeave,
        matchLoop,
        matchSignal,
        matchTerminate
      });
}