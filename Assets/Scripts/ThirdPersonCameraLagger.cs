
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;
public class ThirdPersonCameraLagger : MonoBehaviour
{
    Spaceship spaceship;
    private Vector3 originalPositionOffset;
    private Quaternion originalRotationOffset;

    private void Start()
    {
        spaceship = GetComponentInParent<Spaceship>();
        originalPositionOffset = transform.localPosition;
        originalRotationOffset = transform.localRotation;
    }

    private void Update()
    {

        var lagPosition = Vector3.ClampMagnitude(spaceship.transform.InverseTransformDirection(spaceship.rb.velocity) / 10, 2);
        var pitch = Quaternion.AngleAxis(Vector3.Dot(-spaceship.transform.right, spaceship.rb.angularVelocity) * 10, spaceship.transform.right);
        transform.localPosition = pitch * (originalPositionOffset - lagPosition);
        transform.localRotation = Quaternion.AngleAxis(-Vector3.Dot(-spaceship.transform.forward, spaceship.rb.angularVelocity) * 2, transform.localPosition) * pitch;
        // transform.Rotate(-transform.localPosition, Vector3.Dot(-transform.localPosition, spaceship.rb.angularVelocity) / 10);
        // transform.localRotation = originalRotationOffset + spaceship.rb.angularVelocity;
    }
}