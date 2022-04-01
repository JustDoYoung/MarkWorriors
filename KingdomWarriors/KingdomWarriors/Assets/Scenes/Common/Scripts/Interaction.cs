using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interaction
{
    public interface Attack{
        public void OnAttack(int damage);
    }
}
