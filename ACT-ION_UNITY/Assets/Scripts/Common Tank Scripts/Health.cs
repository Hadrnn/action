using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    public void heal(float amount);

    public void TakeDamage(float amount, InfoCollector.Team.TankHolder shellOwner);
}
