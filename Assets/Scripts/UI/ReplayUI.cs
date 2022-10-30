using Nakama;
using Nakama.TinyJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ReplayUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI MatchId;
    [SerializeField] ReplayHandler replayHandler;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public async void PlayReplay()
    {
        string matchId = MatchId.text.Trim((char)8203);

        Debug.Log(matchId == "10f8e96d-0e2c-4a3c-8d1b-9b4c151697d6.nakama");

        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload.Add("matchId", matchId);

        IApiRpc response = await NakamaConnection.Client.RpcAsync(NakamaConnection.Session, "GetReplayData", JsonWriter.ToJson(payload));

        Dictionary<string, StorageItem[]> responseParsed = JsonParser.FromJson<Dictionary<string, StorageItem[]>>(response.Payload);
        Debug.Log(response.Payload);

        StorageItem[] items = responseParsed["objects"];
        StorageItem[] sortedItems = items.OrderBy(item => int.Parse(item.key)).ToArray();

        replayHandler.StartReplay(sortedItems);
        gameObject.SetActive(false);
    }
}

public class StorageItem
{
    public string key;
    public string userid;
    public int createTime;
    public string version;
    public PacketArray value;
    public string collection;
    public int permissionRead;
    public int permissionWrite;
    public int updateTime;
}

public class PacketArray
{
    public Packet[] packets;
}

public class Packet
{
    public string data;
    public int opCode;
    public Sender sender;
}

public class Sender
{
    public string node;
    public string userId;
    public string username;
    public string sessionId;
}