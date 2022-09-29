using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
    public Text textUI;
    public static DebugText Instance;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public void Set(string text) {
        textUI.text = text;
    }

    public void Add(string text) {
        textUI.text += "\n" + text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
