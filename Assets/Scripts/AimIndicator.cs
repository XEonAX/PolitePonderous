using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AimIndicator : MonoBehaviour
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
        rectTransform.localPosition = inputMgr.vAim * 100;

        rectTransform.localRotation = Quaternion.LookRotation(Vector3.forward, inputMgr.vAim);

        image.color = new Color(image.color.r, image.color.g, image.color.b, inputMgr.vAim.sqrMagnitude);
        if (inputMgr.vAim.sqrMagnitude < 0.02f)
        {
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
            rectTransform.localScale = Vector3.one * 0.3f;
        }
        else
        {
            rectTransform.localPosition = inputMgr.vAim * 100;
            rectTransform.localRotation = Quaternion.LookRotation(Vector3.forward, inputMgr.vAim);
            image.color = new Color(image.color.r, image.color.g, image.color.b, inputMgr.vAim.sqrMagnitude);
            rectTransform.localScale = Vector3.one * inputMgr.vAim.sqrMagnitude;
        }
    }
}
