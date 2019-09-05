using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarTower : Tower
{
    [SerializeField, Range(0.5f, 2f)] private float shotsPerSecond = 1f;

    [SerializeField] private Transform mortar;

    public override TowerType TowerType
    {
        get { return TowerType.Mortar; }
    }


}
