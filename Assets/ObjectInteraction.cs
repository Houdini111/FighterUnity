using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour//ScriptableObject //Since this is being stored by Master, a GameObject, it can't be a ScriptableObject
{
    public bool attack(IAttacker source, IAttackable target)
    {
        return target.getAttacked(source.damage);
    }
}
