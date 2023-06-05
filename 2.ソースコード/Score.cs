using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NCMB;

public class Score : MonoBehaviour
{
    #region // スコアパネル用
    [SerializeField] TMP_Text wordCountText;                    // 打てた文字数
    [SerializeField] TMP_Text rankText;                         // ランク
    [SerializeField] TMP_Text commentText;                      // 文字数に応じたコメント
    [SerializeField] GameObject scorePanel;                     // スコアパネル
    [SerializeField] Image rankImagePos;                        // ランクイメージ表示位置
    [SerializeField] Sprite[] rankSprits = new Sprite[5];       // ランクイメージスプライト
    [SerializeField] GameObject retryKey;                       // リトライボタン
    [SerializeField] GameObject titleKey;                       // タイトルバックボタン
    [SerializeField] GameObject rankingKey;                     // ランキングボタン
    #endregion

    #region // ハイスコアパネル用
    [SerializeField] JsonScoreSave jsonScoreSave;               // データの読み書き
    [SerializeField] GameObject hiscorePanel;                   // ハイスコアパネル
    [SerializeField] TMP_Text displayName;                      // ハイスコア登録表示ネーム
    [SerializeField] TMP_Text displayScore;                     // ハイスコア登録表示
    [SerializeField] TMP_Text displayAther;                     // ハイスコア同点者表示
    [SerializeField] TMP_InputField inputField;                 // 名前入力
    int atherNum;                                               // ハイスコア同点者数
    bool isCount;
    #endregion

    string[] ranks 
        = { "Cランク", "Bランク", "Aランク", "Sランク", "SSランク" };

    int wordCount;                                              // プレイヤーが打てた文字数
    JsonScoreSave.Data data;


    void Start()
    {
        isCount = true;
        DataLoadMove();
        wordCount = NewManager.wordCount;
        StartCoroutine(ScoreBoard(wordCount));
    }

    void Update()
    {
        // ランキングパネルが表示されてない時に反応する
        GameObject rankingBoard = GameObject.Find("HiScore");
        if (rankingBoard == null && retryKey.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.R))                // リトライ
            {
                SceneManager.LoadScene("GameScene_FullScreen");
            }
            else if (Input.GetKeyDown(KeyCode.T))           // タイトルバック
            {
                SceneManager.LoadScene("TitleScene");
            }
            else if (Input.GetKeyDown(KeyCode.E))           // ハイスコア表示
            {
                //naichilab.RankingLoader.Instance.SendScoreAndShowRanking(wordCount);
                hiscorePanel.SetActive(true);
            }
        }

        if (hiscorePanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Delete))           // ハイスコアパネルを閉じる
                hiscorePanel.SetActive(false);

            if (wordCount == data.score && isCount)
            {
                inputField.interactable = true;
                atherNum = data.ather + 1;
                isCount = false;
            }
            else if (wordCount > data.score && isCount)
            {
                inputField.interactable = true;
                atherNum = 0;
                isCount = false;
            }

            if (inputField.interactable)
            {
                if (Input.GetKeyDown(KeyCode.LeftAlt))
                {
                    jsonScoreSave.SaveCall(atherNum);
                    DataLoadMove();
                    inputField.text = "";
                }
            }
        }
    }

    // データロード時の画面表示更新
    void DataLoadMove()
    {
        data = jsonScoreSave.LoadCall();
        displayName.SetText(data.name);
        displayScore.SetText(data.score.ToString());
        displayAther.SetText("同点者他" + data.ather + "名");
    }

    // スコアボードを表示させる
    IEnumerator ScoreBoard(int wordCt)
    {
        scorePanel.SetActive(true);
        yield return new WaitForSeconds(1);
        wordCountText.SetText(wordCt.ToString());
        yield return new WaitForSeconds(1);
        RankCheck(wordCt);
        retryKey.SetActive(true);
        titleKey.SetActive(true);
        rankingKey.SetActive(true);
    }

    // 打てた文字数によってランクの振り分け
    void RankCheck(int count)
    {
        if (count <= 5)                         // C rank
        {
            int nextCount = 6 - count;
            rankText.color = new Color32(150, 70, 0, 255);
            rankText.SetText(ranks[0]);
            commentText.SetText("あと" + nextCount + "個でランク昇格！");
            rankImagePos.sprite = rankSprits[0];
        }
        else if (count >= 6 && count <= 10)     // B rank
        {
            int nextCount = 11 - count;
            rankText.color = Color.gray;
            rankText.SetText(ranks[1]);
            commentText.SetText("あと" + nextCount + "個でランク昇格！");
            rankImagePos.sprite = rankSprits[1];
        }
        else if (count >= 11 && count <= 16)    // A rank
        {
            int nextCount = 17 - count;
            rankText.color = Color.yellow;
            rankText.SetText(ranks[2]);
            commentText.SetText("あと" + nextCount + "個でランク昇格！");
            rankImagePos.sprite = rankSprits[2];
        }
        else if (count >= 17 && count <= 19)    // S rank
        {
            int nextCount = 20 - count;
            rankText.color = new Color32(180, 0, 255, 255);
            rankText.SetText(ranks[3]);
            commentText.SetText("あと" + nextCount + "個でランク昇格！");
            rankImagePos.sprite = rankSprits[3];
        }
        else if (count >= 20)                   // SS rank
        {
            rankText.color = Color.red;
            rankText.SetText(ranks[4]);
            commentText.SetText("最高ランク！神速レスポンサー！");
            rankImagePos.sprite = rankSprits[4];
        }
    }

}
