using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using UnityEngine.UI;

public class MatchLobbyUI : MonoBehaviour
{
    [SerializeField] GameObject listContent;
    [SerializeField] GameObject playerRowPrefab;

    [SerializeField] Color evenRowColor;
    [SerializeField] Color unevenRowColor;

    // Start is called before the first frame update
    void Start()
    {
        NakamaMatchHandler.MatchStart += OnMatchStart;
        NakamaMatchHandler.MatchPlayerJoined += RefreshPlayerList;
        NakamaMatchHandler.MatchPlayerLeft += RefreshPlayerList;

        RefreshPlayerList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnMatchStart(IMatchState state)
    {
        MainThread.Enqueue(() =>
        {
            gameObject.SetActive(false);
        });
    }

    void RefreshPlayerList(IUserPresence player = null)
    {
        Debug.Log("Refreshing player list");
        Debug.Log("Users amount: " + NakamaMatchHandler.UsersInMatch.Count);
        MainThread.Enqueue(() =>
        {
            foreach (Transform child in listContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            for (int i = 0; i < NakamaMatchHandler.UsersInMatch.Count; i++)
            {
                IUserPresence user = NakamaMatchHandler.UsersInMatch[i];


                GameObject row = Instantiate(playerRowPrefab, listContent.transform);
                row.GetComponent<Image>().color = i % 2 == 0 ? evenRowColor : unevenRowColor;

                row.GetComponentInChildren<Text>().text = "# " + i + "   " + user.Username;
            }
        });       
    }
}
