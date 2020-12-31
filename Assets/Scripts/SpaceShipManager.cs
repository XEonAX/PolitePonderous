using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpaceShipManager : MonoBehaviour
{
    public List<Spaceship> Spaceships;
    public Spaceship FocusedSpaceship;
    public static SpaceShipManager Instance;

    public Camera PersonalCamera;
    public Transform DramaCameraFocusPoint;

    private void Awake()
    {
        Instance = this;
        Spaceships = GetComponentsInChildren<Spaceship>().ToList();
    }
    // Start is called before the first frame update
    void Start()
    {
        DisableAllCameras();
        FocusedSpaceship = Spaceships.FirstOrDefault();
        PersonalCamera.transform.parent = FocusedSpaceship.CameraPosition;
        DramaCameraFocusPoint.parent = FocusedSpaceship.CoM;
        // FocusedSpaceship.PersonalCamera.gameObject.SetActive(true);
    }

    private void DisableAllCameras()
    {
        // foreach (var spaceship in Spaceships)
        // {
        //     spaceship.PersonalCamera.gameObject.SetActive(false);
        // }
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
        DisableAllCameras();
        FocusedSpaceship = Spaceships[index];
        PersonalCamera.transform.parent = FocusedSpaceship.CameraPosition;
        DramaCameraFocusPoint.parent = FocusedSpaceship.CoM;
    }
}
