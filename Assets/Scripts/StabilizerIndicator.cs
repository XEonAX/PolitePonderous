using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StabilizerIndicator : MonoBehaviour
{

    public Text txt;
    public IInputMgr inputMgr;
    // Start is called before the first frame update
    void Start()
    {
        txt = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inputMgr.disableStabilizer)
        {
            txt.text = "Stabilize OFF";
            txt.color = Color.red;
        }
        else
        {
            txt.text = "Stabilize ON";
            txt.color = Color.green;
        }
    }
}
