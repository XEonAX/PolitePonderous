using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AimIndicator : MonoBehaviour
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
        rectTransform.localPosition = InputMgr.Instance.vAim * 100;

        rectTransform.localRotation = Quaternion.LookRotation(Vector3.forward, InputMgr.Instance.vAim);

        image.color = new Color(image.color.r, image.color.g, image.color.b, InputMgr.Instance.vAim.sqrMagnitude);
        if (InputMgr.Instance.vAim.sqrMagnitude < 0.02f)
        {
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
            rectTransform.localScale = Vector3.one * 0.3f;
        }
        else
        {
            rectTransform.localPosition = InputMgr.Instance.vAim * 100;
            rectTransform.localRotation = Quaternion.LookRotation(Vector3.forward, InputMgr.Instance.vAim);
            image.color = new Color(image.color.r, image.color.g, image.color.b, InputMgr.Instance.vAim.sqrMagnitude);
            rectTransform.localScale = Vector3.one * InputMgr.Instance.vAim.sqrMagnitude;
        }
    }
}
