using UnityEngine;

public class IInputMgr : MonoBehaviour
{
    public static IInputMgr IInstance { get; set; }
    public virtual bool disableStabilizer { get; set; } = false;
    public virtual Vector2 vAim { get; set; } = Vector2.zero;
    public virtual float vRoll { get; set; }
    public virtual float vUpDown { get; set; }
    public virtual float vLeftRight { get; set; }
    public virtual float vForwardBack { get; set; }

    private void Start()
    {
        IInstance = this;
    }
}