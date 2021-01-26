using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;

public class SpaceShipManager : MonoBehaviour
{
    public List<Spaceship> Spaceships;
    public Spaceship FocusedSpaceship;
    public static SpaceShipManager Instance;

    public Camera PersonalCamera;

    public AimConstraint PersonalCameraAimConstraint;
    public Transform DramaCameraFocusPoint;

    private void Awake()
    {
        Instance = this;
        //Spaceships = GetComponentsInChildren<Spaceship>().ToList();
    }
    // Start is called before the first frame update
    void Start()
    {
        FocusedSpaceship = Spaceships.FirstOrDefault();
        PersonalCamera.transform.parent = FocusedSpaceship.CameraPosition;
        //PersonalCameraAimConstraint.SetSource(0, new ConstraintSource() { sourceTransform = FocusedSpaceship.CoM, weight = 1 });
        DramaCameraFocusPoint.parent = FocusedSpaceship.CoM;
        // FocusedSpaceship.PersonalCamera.gameObject.SetActive(true);
    }


    // Update is called once per frame
    void Update()
    {
        PersonalCamera.transform.localPosition = Vector3.Lerp(PersonalCamera.transform.localPosition, Vector3.zero, Time.deltaTime);
        PersonalCamera.transform.localRotation = Quaternion.Lerp(PersonalCamera.transform.localRotation, Quaternion.identity, Time.deltaTime);
        DramaCameraFocusPoint.localPosition = Vector3.Lerp(DramaCameraFocusPoint.localPosition, Vector3.zero, Time.deltaTime);
    }

    internal void FocusNextShip()
    {
        var index = Spaceships.IndexOf(FocusedSpaceship);
        index++;
        if (index >= Spaceships.Count)
        {
            index = 0;
        }
        FocusedSpaceship = Spaceships[index];
        PersonalCamera.transform.parent = FocusedSpaceship.CameraPosition;
        //PersonalCameraAimConstraint.SetSource(0, new ConstraintSource() { sourceTransform = FocusedSpaceship.CoM, weight = 1 });
        DramaCameraFocusPoint.parent = FocusedSpaceship.CoM;
    }

    // private void LateUpdate()
    // {
    //     Vector3 velocity = Vector3.zero;
    //     PersonalCamera.transform.position = Vector3.SmoothDamp(PersonalCamera.transform.position, FocusedSpaceship.CameraPosition.position, ref velocity, Time.deltaTime, FocusedSpaceship.MaxVelocity * 10);
    //     PersonalCamera.transform.rotation = Quaternion.Slerp(PersonalCamera.transform.rotation, FocusedSpaceship.CameraPosition.rotation, Time.deltaTime * 10);
    // }
}
