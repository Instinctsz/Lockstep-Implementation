using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System;
using Nakama.TinyJson;
using System.Text;

public interface IStates
{
    int TickToQueueTo { get; set; }
    string Serialize();
}

public class State : IStates
{
    public int TickToQueueTo { 
        get => NakamaServerManager.CurrentTick + NakamaServerManager.TickDelay; 
        set { }
    }

    public virtual string Serialize()
    {
        throw new NotImplementedException();
    }
}
