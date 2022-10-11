using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Globalization;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI InviteLink;
    [SerializeField] TextMeshProUGUI Username;
    [SerializeField] GameObject[] ObjectsToEnable;
    [SerializeField] GameObject[] ObjectsToDisable;

    [SerializeField] NakamaConnection con;

    // Start is called before the first frame update
    void Start()
    {
        NakamaConnection.OnConnect += OnConnect;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Connect()
    {
        if (InviteLink.text.Length <= 1)
        {
            con.Connect("127.0.0.1", 7350, Username.text);
            return;
        }

        string[] split = InviteLink.text.Split(':');
        string url = split[0];
        string portString = split[1].Substring(0, 5);
        int port = Int32.Parse(portString);

        con.Connect(url, port, Username.text);
    }

    private void OnConnect()
    {
        MainThread.Enqueue(() =>
        {
            foreach (GameObject gameObject in ObjectsToEnable)
                gameObject.SetActive(true);

            foreach (GameObject gameObject in ObjectsToDisable)
                gameObject.SetActive(false);
        });
    }
}
