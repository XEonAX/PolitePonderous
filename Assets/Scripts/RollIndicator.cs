using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollIndicator : MonoBehaviour
{
    private RectTransform rectTransform;

    private Image image;
    public IInputMgr inputMgr;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        image.fillClockwise = inputMgr.vRoll < 0;
        image.fillAmount = Mathf.Abs(inputMgr.vRoll);
    }
}
