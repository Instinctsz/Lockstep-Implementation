using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System;

public class NakamaConnection : MonoBehaviour
{
    public static event Action OnConnect = delegate { };
    public static IClient Client;
    public static ISocket ClientSocket;
    public static ISession Session;

    // Start is called before the first frame update
    async void Awake()
    {
        
    }

    async void OnDestroy()
    {
        await ClientSocket.CloseAsync();
    }

    public async void Connect(string host, int port)
    {
        Client = new Client("http", host, port, "defaultkey");
        Client.Timeout = 10;

        ClientSocket = Socket.From(Client);
        Session = await Client.AuthenticateDeviceAsync(SystemInfo.deviceUniqueIdentifier);
        ClientSocket.Connected += SocketConnected;

        await ClientSocket.ConnectAsync(Session, true, 30);
    }

    private static void SocketConnected()
    {
        Debug.Log("Connected!");
        OnConnect.Invoke();
    }

}
