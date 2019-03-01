using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OG_PlayerHealth : MonoBehaviour
{
    public float fl_currnetPcHealth { get; set; }
    public float fl_maxHealth { get; set; }

    public Slider healthBar;
    public Text tx_healthTxt;
    public string healthString;

    public float fl_displayHelathInInsp;
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
        fl_maxHealth = 100;
        fl_currnetPcHealth = fl_maxHealth;
        fl_displayHelathInInsp = fl_currnetPcHealth;

        healthBar.value = CalculateHealth();
    }

    private void Update()
    {
        if (fl_currnetPcHealth <= 0f)
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

        fl_displayHelathInInsp = fl_currnetPcHealth;

        if(fl_currnetPcHealth > fl_maxHealth)
        {
            fl_currnetPcHealth = fl_maxHealth;
        }

        healthBar.value = CalculateHealth();
        UpdateHealthUI();

    }

    void UpdateHealthUI()
    {
        healthString = fl_currnetPcHealth + " Health";
        tx_healthTxt.text = healthString.ToString();
    }

    float CalculateHealth()
    {
        return fl_currnetPcHealth / fl_maxHealth;
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
        fl_currnetPcHealth -= amount;
        AudioSource.PlayClipAtPoint(painClip, transform.position);
    }
}
