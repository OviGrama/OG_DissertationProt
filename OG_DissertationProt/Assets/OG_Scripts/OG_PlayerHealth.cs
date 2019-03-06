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
    public int in_pcLives;
    public int in_pcMaxLives;

    public Slider healthBar;
    public Text tx_healthTxt;
    public string healthString;
    public Text txt_pcLives;
    public Transform tra_SpawnPoint;
    public Transform tra_PlayerInstance;
    public GameObject GameOverPanel;

    public float fl_displayHealthInInsp;
    public float fl_resetAfterDeathTimer = 5f;
    public AudioClip deathClip;
    public AudioClip painClip;


    private FirstPersonController playerController;
    private MouseLook mouseLookRef;
    private float fl_resetTimer;
    public bool bl_playerDead;

    private OG_GameManager gameManager;


    private void Awake()
    {
        Time.timeScale = 1;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        playerController = GameObject.Find("Player").GetComponent<FirstPersonController>();
        gameManager = GameObject.Find("GameManager").GetComponent<OG_GameManager>();
        fl_maxHealth = 100;
        fl_currnetPcHealth = fl_maxHealth;
        in_pcMaxLives = 0;
        in_pcLives = in_pcMaxLives;
        fl_displayHealthInInsp = fl_currnetPcHealth;

        healthBar.value = CalculateHealth();
    }

    private void Update()
    {
        if (fl_currnetPcHealth <= 0f)
        {
            fl_currnetPcHealth = 0;
            if (!bl_playerDead)
            {
                PlayerDying();
            }
            else
            {
                PlayerDead();
                bl_playerDead = true;
            }

            if (in_pcLives <= 0)
            {
                GameOver();
            }
            else
            {
                Respawn();
            }
        }

        fl_displayHealthInInsp = fl_currnetPcHealth;

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
        txt_pcLives.text = in_pcLives.ToString();
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

    void Respawn()
    {
        fl_resetTimer += Time.deltaTime;
        if (fl_resetTimer >= fl_resetAfterDeathTimer)
        {
            tra_PlayerInstance.transform.position = tra_SpawnPoint.position;
            playerController.enabled = true;
            bl_playerDead = false;
            fl_currnetPcHealth = fl_maxHealth;
            gameManager.fl_difficulty -= 0.75f;
            in_pcLives--;
        }
    }

    void GameOver()
    {
        GameOverPanel.gameObject.SetActive(true);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void LevelReset()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void TakeDamage(float amount)
    {
        fl_currnetPcHealth -= amount;
        AudioSource.PlayClipAtPoint(painClip, transform.position);
    }
}
