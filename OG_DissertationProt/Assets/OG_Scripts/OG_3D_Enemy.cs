using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OG_3D_Enemy : MonoBehaviour
{

    public float fl_health = 100;
    public GameObject DeathVFX;
    public Transform dVFXoffSet;

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
        GameObject deathParticle = Instantiate(DeathVFX, dVFXoffSet.transform.position, transform.rotation);
        Destroy(gameObject);
        Destroy(deathParticle, 3);
    }


}
