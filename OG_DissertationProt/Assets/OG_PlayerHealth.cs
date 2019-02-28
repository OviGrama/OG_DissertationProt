using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;

public class OG_PlayerHealth : MonoBehaviour
{
    public float fl_pcHealth = 100f;
    public float fl_resetAfterDeathTimer = 5f;
    public AudioClip deathClip;
    public AudioClip painClip;

    //private Animator anim;

    private FirstPersonController playerController;
    private float fl_resetTimer;
    public bool bl_playerDead;


    private void Awake()
    {
        //anim = GetComponent<Animator>();
        playerController = GameObject.Find("Player").GetComponent<FirstPersonController>();
    }

    private void Update()
    {
        if (fl_pcHealth <= 0f)
        {
            if (!bl_playerDead)
            {
                PlayerDying();
            }
            else
            {
                PlayerDead();
                LevelReset();
            }

            bl_playerDead = true;
        }
    }

    void PlayerDying()
    {
        bl_playerDead = true;
        AudioSource.PlayClipAtPoint(deathClip, transform.position);
    }

    void PlayerDead()
    {
        playerController.enabled = false;
    }

    void LevelReset()
    {
        fl_resetTimer += Time.deltaTime;
        if(fl_resetTimer >= fl_resetAfterDeathTimer)
        {
            SceneManager.LoadScene(0);
        }
    }

    public void TakeDamage(float amount)
    {
        fl_pcHealth -= amount;
        AudioSource.PlayClipAtPoint(painClip, transform.position);
    }
}
