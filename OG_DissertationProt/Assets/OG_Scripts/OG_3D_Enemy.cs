using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OG_3D_Enemy : MonoBehaviour
{

    public float fl_health = 100;

    public void TakeDamage(float amount)
    {
        fl_health -= amount;
        if(fl_health <= 0f)
        {
            Death();
        }
    }

    void Death()
    {
        Destroy(gameObject);
    }


}
