using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Weapon : MonoBehaviour
{
    public List<Transform> Barrels;
    public Bullet Bullet;
    public int RoF;
    public int Ammo;
    public int MaxAmmo;
    public int AmmoRegen;
    protected int idxRof;
    protected int idxAmmo;
    public int force;

    public bool Primary;
    public bool Secondary;

    public Transform WeaponTarget;
    public Transform LocalTarget;
    public Transform Turret;
    public Transform Gun;
    public Transform Orientation;

    public bool TargetLockNeeded;
    public bool TargetLocked;
    private Spaceship Spaceship;
    private void Start()
    {
        Spaceship = GetComponentInParent<Spaceship>();
        WeaponTarget = GetComponentInParent<Spaceship>().WeaponTarget;
        //LocalTarget.parent = WeaponTarget;
    }
    private void FixedUpdate()
    {
        Shoot();
    }

    private void Shoot()
    {
        if (idxRof >= 0)
            idxRof--;

        if (idxAmmo >= 0 && Ammo == 0)
            idxAmmo--;

        if (Primary && InputMgr.Instance.PrimaryFire && idxRof < 0 && Ammo > 0)
        {
            Ammo--;
            idxRof = RoF;
            if (TargetLockNeeded)
            {
                if (TargetLocked)
                {
                    var bullet = ObjectPool.Spawn<Bullet>(Bullet, Barrels[0].position, Barrels[0].rotation);
                    bullet.Shoot(force);
                    //Spaceship.rb.AddForceAtPosition(-Barrels[0].forward*force/20, Barrels[0].position,ForceMode.Impulse);
                }
            }
            else
            {
                var bullet = ObjectPool.Spawn<Bullet>(Bullet, Barrels[0].position, Barrels[0].rotation);
                bullet.Shoot(force);
                //Spaceship.rb.AddForceAtPosition(-Barrels[0].forward * force/20, Barrels[0].position, ForceMode.Impulse);
            }
        }
        if (idxAmmo < 0)
        {
            idxAmmo = AmmoRegen;
            Ammo = MaxAmmo;
        }
    }
    private void Update()
    {

        Vector3 rot = Quaternion.LookRotation(LocalTarget.localPosition - Turret.transform.localPosition).eulerAngles;
        rot.x = rot.z = 0;
        Turret.localRotation = Quaternion.Euler(rot);


        Gun.LookAt(LocalTarget, transform.up);
    }
}
