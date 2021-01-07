using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollIndicator : MonoBehaviour
{
    private RectTransform rectTransform;

    private Image image;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        image.fillClockwise = InputMgr.Instance.vRoll < 0;
        image.fillAmount = Mathf.Abs(InputMgr.Instance.vRoll);
    }
}
