using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OG_GameManager : MonoBehaviour
{

    public Text txt_DifficultyFloat;
    public Text txt_DifficultyState;
    public string st_DifficultyName;
    public float fl_difficulty;
    public float fl_minDifficultyFloat = 0f;

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

    }

    // Update is called once per frame
    void Update()
    {
        if (fl_difficulty <= 0)
        {
            fl_difficulty = fl_minDifficultyFloat;
        }

        txt_DifficultyFloat.text = fl_difficulty.ToString();
        txt_DifficultyState.text = st_DifficultyName;
        DifficultyAdjustment();
    }

    void DifficultyAdjustment()
    {
        if(fl_difficulty <= 10)
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
