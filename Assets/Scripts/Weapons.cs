using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Weapon
{
    public string Name;
    public GameObject Projectile;
    public float Cooldown;
    public float Speed;
}

public class Weapons : MonoBehaviour
{
    public List<Weapon> WeaponList;
}
