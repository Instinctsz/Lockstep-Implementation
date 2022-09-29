using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;

public class NakamaConnection : MonoBehaviour
{
    public static IClient Client;
    public static ISocket ClientSocket;
    public static ISession Session;

    [SerializeField] private bool appearOnline = true;
    [SerializeField] private int connectionTimeout = 30;

    // Start is called before the first frame update
    async void Awake()
    {
        Client = new Client("http", "127.0.0.1", 7350, "defaultkey");
        Client.Timeout = 10;

        ClientSocket = Socket.From(Client);
        Session = await Client.AuthenticateDeviceAsync(SystemInfo.deviceUniqueIdentifier);
    
        await ClientSocket.ConnectAsync(Session, appearOnline, connectionTimeout);
    }
}
