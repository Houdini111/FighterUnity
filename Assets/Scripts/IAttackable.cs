using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    bool getAttacked(decimal damage);

    void die();
}
