using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class OG_GameManager : MonoBehaviour
{

    public Text txt_DifficultyFloat;
    public Text txt_DifficultyState;
    public Text txt_StaticDifficultyState;
    public string st_StaticDifficultyName;
    public string st_DifficultyName;
    public float fl_difficulty;
    public float fl_minDifficultyFloat = 0f;

    public GameObject PauseGamePanel;
    FirstPersonController firstPersonController;
    OG_3D_Gun gunRef;

    public bool bl_DDA;
    public bool bl_EASY;
    public bool bl_MEDIUM;
    public bool bl_HARD;

    public enum Difficulty
    {
        EASY,
        MEDIUM,
        HARD
    }

    public Difficulty difficulty;

    // Start is called before the first frame update
    void Start()
    {
        firstPersonController = GameObject.Find("Player").GetComponent<FirstPersonController>();
        GameObject player = GameObject.Find("Player");
        gunRef = player.GetComponentInChildren<OG_3D_Gun>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            PauseGamePanel.gameObject.SetActive(true);
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            firstPersonController.enabled = false;
            gunRef.enabled = false;
        }

        if (fl_difficulty <= 0)
        {
            fl_difficulty = fl_minDifficultyFloat;
        }



        if (bl_DDA)
        {
            DifficultyAdjustment();
            txt_DifficultyFloat.gameObject.SetActive(true);
            txt_DifficultyState.gameObject.SetActive(true);
            txt_StaticDifficultyState.gameObject.SetActive(false);
        }
        else
        {
            StaticDifficulty();
            txt_StaticDifficultyState.gameObject.SetActive(true);
            txt_DifficultyFloat.gameObject.SetActive(false);
            txt_DifficultyState.gameObject.SetActive(false);
            return;
        }
    }

    public void ResumeGame()
    {
        PauseGamePanel.gameObject.SetActive(false);
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        firstPersonController.enabled = true;
        gunRef.enabled = true;
    }

    void StaticDifficulty()
    {
        txt_StaticDifficultyState.text = st_StaticDifficultyName;

        if(difficulty == Difficulty.EASY)
        {
            st_StaticDifficultyName = "Easy";
        }

        if (difficulty == Difficulty.MEDIUM)
        {
            st_StaticDifficultyName = "Medium";
        }

        if (difficulty == Difficulty.HARD)
        {
            st_StaticDifficultyName = "Hard";
        }
    }

    void DifficultyAdjustment()
    {
        txt_DifficultyState.text = st_DifficultyName;
        txt_DifficultyFloat.text = fl_difficulty.ToString();

        if (fl_difficulty <= 10)
        {
            difficulty = Difficulty.EASY;
            st_DifficultyName = "Easy";
        }

        if (fl_difficulty >= 11)
        {
            if(fl_difficulty <= 20)
            {
                difficulty = Difficulty.MEDIUM;
                st_DifficultyName = "Medium";
            }
        }

        if(fl_difficulty >= 21)
        {
            if(fl_difficulty <= 30)
            {
                difficulty = Difficulty.HARD;
                st_DifficultyName = "Hard";
            }
        }

    }


}
