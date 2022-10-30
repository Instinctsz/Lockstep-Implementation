using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System;
using Nakama.TinyJson;
using System.Text;

public interface IStates
{
    string Serialize();
}

public class State : IStates
{
    public int TickToQueueTo = NakamaServerManager.CurrentTick + NakamaServerManager.TickDelay;   

    public virtual string Serialize()
    {
        throw new NotImplementedException();
    }
}
