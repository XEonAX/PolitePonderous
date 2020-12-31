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
    }
}
