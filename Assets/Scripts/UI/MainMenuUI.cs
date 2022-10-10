using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Globalization;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI InviteLink;
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
        string[] split = InviteLink.text.Split(':');
        string url = split[0];
        string portString = split[1].Substring(0, 5);
        int port = Int32.Parse(portString);

        Debug.Log("Here0");
        con.Connect(url, port);
    }

    private void OnConnect()
    {
        Debug.Log("Here4");
        MainThread.Enqueue(() =>
        {
            foreach (GameObject gameObject in ObjectsToEnable)
            {
                Debug.Log("enabling :" + gameObject.name);
                gameObject.SetActive(true);
            }

            foreach (GameObject gameObject in ObjectsToDisable)
                gameObject.SetActive(false);
        });
    }
}
