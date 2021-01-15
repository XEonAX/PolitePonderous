using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private void Start()
    {
        WeaponTarget = GetComponentInParent<Spaceship>().WeaponTarget;
        LocalTarget.parent = WeaponTarget;
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
            var bullet = ObjectPool.Spawn<Bullet>(Bullet, Barrels[0].position, Barrels[0].rotation);
            bullet.Shoot(force);
        }
        if (idxAmmo < 0)
        {
            idxAmmo = AmmoRegen;
            Ammo = MaxAmmo;
        }
    }
    private void Update()
    {
        Turret.LookAt(WeaponTarget, transform.up);
        Gun.LookAt(WeaponTarget, transform.up);
    }
}
