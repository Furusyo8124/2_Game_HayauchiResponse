using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NCMB;

public class SystemManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    public void Retry()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void TitleBack()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void Ranking()
    {
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(NewManager.wordCount);
    }
}
