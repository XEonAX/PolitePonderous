

using UnityEngine;

public class MLInputMgr : IInputMgr
{
    public static MLInputMgr Instance;

    private void Start()
    {
        Instance = this;
    }

}