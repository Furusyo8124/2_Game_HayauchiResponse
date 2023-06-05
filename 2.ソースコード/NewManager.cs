using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NCMB;

public class NewManager : MonoBehaviour
{
    #region//インスペクター上の各テキスト
    [SerializeField] TMP_Text levelText;                                   // 現在のレベル
    [SerializeField] TMP_Text questionText;                                // 問題
    [SerializeField] TMP_Text missText;                                    // 押し間違い時
    [SerializeField] TMP_Text timeText;                                    // 押した速さ
    [SerializeField] TMP_Text timeLimitText;                               // 残り時間
    [SerializeField] TMP_Text explanationText;                             // 最初に出てくる遊び方
    [SerializeField] TMP_Text serifText;                                   // 右上のセリフ
    [SerializeField] TMP_Text timeUpText;                                  // 時間切れ時
    [SerializeField] TMP_Text startText;                                   // ゲームスタート
    #endregion

    #region//インスペクター上の各ゲームオブジェクト
    [SerializeField] GameObject answer;                                    // 正解時の〇イラスト
    [SerializeField] GameObject notAnswer;                                 // 不正解時の×イラスト
    [SerializeField] GameObject serifCat;                                  // 右上のコメントキャットのイラスト
    [SerializeField] GameObject[] keyBoardChilds = new GameObject[26];     // 各キー
    #endregion

    #region//インスペクター上の各画像系
    [SerializeField] Image timeBar;                                        // 持ち時間の緑の方
    [SerializeField] Image pinkTimeBar;                                    // 持ち時間削られた時のピンクの方
    #endregion

    // 効果音
    [SerializeField] AudioClip[] se = new AudioClip[3];

    #region//プライベート変数
    string[] catSrif 
        = { "はやいニャ！", "まぁまぁだニャ", "のろまだニャ" };            // 文字を打った時の速さに応じたセリフ
    int currentLevel = 0;                                                  // 問題レベルの数値
    string level = "LEVEL ";
    float keyTime;
    float startTime;
    float keyTimeSum = 10;

    Animator catAnim;
    AudioSource audio;

    SpriteRenderer[] spriteRenderer = new SpriteRenderer[26];

    // レベルごとに足されるキー
    KeyCode[][] levelRelease = new KeyCode[][]
    {
        new KeyCode[]{KeyCode.J},                                                               // レベル1
        new KeyCode[]{KeyCode.F},                                                               // レベル2
        new KeyCode[]{KeyCode.K,KeyCode.D},                                                     // レベル3
        new KeyCode[]{KeyCode.G,KeyCode.H,KeyCode.A,KeyCode.S,KeyCode.L},                       // レベル4
        new KeyCode[]{KeyCode.Q,KeyCode.W,KeyCode.P,KeyCode.O,KeyCode.E},                       // レベル5
        new KeyCode[]{KeyCode.R,KeyCode.T,KeyCode.I,KeyCode.U,KeyCode.Y},                       // レベル6
        new KeyCode[]{KeyCode.Z,KeyCode.X,KeyCode.M,KeyCode.N,KeyCode.B,KeyCode.C,KeyCode.V}    // レベル7
    };

    List<KeyCode> useKeyCode = new List<KeyCode>();                        // 問題に使われる英字リスト
    #endregion

    // スコアランキング用
    public static int wordCount;                                           // 打てた文字数

    private void Awake()
    {
        // キーボードの取得
        for (int i = 0; i < keyBoardChilds.Length; i++)
        {
            spriteRenderer[i] = keyBoardChilds[i].GetComponent<SpriteRenderer>();
        }

        // 最初のレベルを導入
        useKeyCode.AddRange(levelRelease[currentLevel]);
        currentLevel++;
    }

    void Start()
    {
        wordCount = 0;
        catAnim = serifCat.GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        StartCoroutine(RuleTextMove());
    }

    // レベルによって使われるキーを光らせる
    void KeyBoardColor()
    {
        foreach (KeyCode keyCord in useKeyCode)
        {
            int i = (int)keyCord - (int)KeyCode.A;
            spriteRenderer[i].color = new Color32(255, 255, 255, 255);
        }
    }

    // キーカラーリセット
    void KeyBoardReset()
    {
        for (int i = 0; i < spriteRenderer.Length; i++)
        {
            spriteRenderer[i].color = new Color32(255, 255, 255, 60);
        }
    }

    // 一番最初だけの演出、遊び方のテキスト
    IEnumerator RuleTextMove()
    {
        RectTransform rectTransform;
        rectTransform = explanationText.GetComponent<RectTransform>();
        yield return Slide(rectTransform,0);
        yield return FirstTextCheck();
        yield return Slide(rectTransform,1600);
        StartCoroutine(LevelTextMove());
    }

    // 現在のレベルの表示の動き
    IEnumerator LevelTextMove()
    {
        if(currentLevel < 7)
            levelText.SetText(level + currentLevel);
        else
            levelText.SetText("LEVELMAX");
        KeyBoardColor();
        RectTransform rectTransform;
        rectTransform = levelText.GetComponent<RectTransform>();
        yield return LevelScalePlus(rectTransform,1.4f);
        yield return LevelScaleMinus(rectTransform, 1);
        StartCoroutine(Question());
    }

    // レベル看板の動き
    IEnumerator LevelScalePlus(RectTransform rectTransform,float end)
    {
        while (true)
        {
            rectTransform.localScale += new Vector3(0.05f, 0.05f, 0.05f);
            if (rectTransform.localScale.x >= end)
                break;

            yield return null;
        }
    }

    IEnumerator LevelScaleMinus(RectTransform rectTransform, float end)
    {
        while (true)
        {
            rectTransform.localScale -= new Vector3(0.05f, 0.05f, 0.05f);
            if (rectTransform.localScale.x <= end)
                break;

            yield return null;
        }
    }

    // テキストスライドさせる動き
    IEnumerator Slide(RectTransform rectTransform,float end)
    {
        while (true)
        {
            rectTransform.localPosition += new Vector3(15, 0, 0);
            if (rectTransform.localPosition.x >= end)
                break;

            yield return null;
        }
        yield return new WaitForSeconds(1);
    }

    // 最初の遊び方テキストの次へボタン
    IEnumerator FirstTextCheck()
    {
        startText.SetText("←スタート！");
        spriteRenderer[9].color = new Color32(255, 255, 255, 255);
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                startText.SetText("");
                break;
            }
            yield return null;
        }
    }

    // 問題の表示
    IEnumerator Display(float delay)
    {
        yield return new WaitForSeconds(delay);
        int i = Random.Range(0, useKeyCode.Count);                          // どの英字を出すかランダムに選ぶ
        questionText.SetText(useKeyCode[i].ToString());                     // 問題が表示される
        audio.Stop();
        startTime = Time.time;                                              // 問題が表示されてから入力されるまでの時間を計測開始
    }

    // 問題の表示、表示されたあとのプレイヤーの入力チェック
    IEnumerator Question()
    {
        StartCoroutine(Display(Random.Range(1f, 3f)));
        startTime = 0;

        // 出題されてからプレイヤーが押したキーボードと答えが合っているかのチェック
        bool isHit = false;
        bool isOther = false;
        while (true)
        {
            for (KeyCode j = KeyCode.A; j <= KeyCode.Z; j++)
            {
                if (Input.GetKeyDown(j))
                {
                    if (j.ToString() == questionText.text)
                    {
                        isHit = true;
                        // 押したキーを光る処理
                        int n = (int)j - (int)KeyCode.A;
                        spriteRenderer[n].color = new Color32(0, 255, 255, 255);
                    }
                    else if (questionText.text == "")
                    {

                    }
                    else
                    {
                        isOther = true;
                        // 押したキーを光る処理
                        int n = (int)j - (int)KeyCode.A;
                        spriteRenderer[n].color = new Color32(255, 0, 255, 255);
                    }
                    break;
                }
            }

            if (isHit || isOther)
                break;

            yield return null;
        }

        keyTime = Time.time - startTime;

        // 問題と違うキーを押したとき
        if (isOther)
        {
            audio.PlayOneShot(se[1]);
            audio.Play();

            // 持ち時間の管理
            keyTimeSum -= keyTime;
            timeBar.fillAmount -= (keyTime / 10);

            if (keyTimeSum <= 0)
            {
                bool isBatsu = false;
                StartCoroutine(TimeUp(isBatsu));
                yield break;
            }

            yield return UnsuccessfulMove();
            StartCoroutine(LevelTextMove());
        }

        // 正解のキーを押したとき
        else if (isHit)
        {
            audio.Play();
            audio.PlayOneShot(se[0]);
            wordCount++;                                                        // 正解した文字数を足す
            if (currentLevel < levelRelease.Length)                             // レベルMAXでなければレベルを1足す
            {
                useKeyCode.AddRange(levelRelease[currentLevel]);
                currentLevel++;
            }

            // 持ち時間の管理
            keyTimeSum -= keyTime;
            timeBar.fillAmount -= (keyTime / 10);

            if (keyTimeSum <= 0)
            {
                bool isMaru = true;
                StartCoroutine(TimeUp(isMaru));
                yield break;
            }

            yield return SuccessMove();
            StartCoroutine(LevelTextMove());
        }
    }

    // 不正解だった時の動き
    IEnumerator UnsuccessfulMove()
    {
        missText.SetText("MISS！");
        serifText.SetText("おてつきニャ！");
        catAnim.SetBool("move", true);
        timeText.SetText(keyTime.ToString("F2") + "秒");
        notAnswer.SetActive(true);

        // 持ち時間ゲージのバーを減らす動き
        yield return new WaitForSeconds(0.5f);
        float pinkBarStartPos = pinkTimeBar.fillAmount;                                     // ピンクゲージが減る前の位置
        float pinkBarStartTime = Time.time;                                                 // スタートの時間を入れる
        while (true)
        {
            float pinkBarEndTime = Mathf.Min(Time.time - pinkBarStartTime, 0.5f) / 0.5f;    // 0.0～1.0が入る     
            pinkTimeBar.fillAmount = Mathf.Lerp(pinkBarStartPos, timeBar.fillAmount, pinkBarEndTime);
            if (pinkBarEndTime == 1)
                break;
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        KeyBoardReset();
        notAnswer.SetActive(false);
        questionText.SetText("");
        timeText.SetText("");
        serifText.SetText("");
        missText.SetText("");
        catAnim.SetBool("move", false);
        yield return new WaitForSeconds(1);
    }

    // 正解した時の動き
    IEnumerator SuccessMove()
    {
        // 押した速さによっての猫のコメント振り分け
        if (keyTime < 1)
            serifText.SetText(catSrif[0]);
        else if (keyTime >= 1 && keyTime < 3)
            serifText.SetText(catSrif[1]);
        else if (keyTime >= 3)
            serifText.SetText(catSrif[2]);

        catAnim.SetBool("move", true);
        timeText.SetText(keyTime.ToString("F2") + "秒");
        answer.SetActive(true);

        // 持ち時間ゲージのバーを減らす動き
        yield return new WaitForSeconds(0.5f);
        float pinkBarStartPos = pinkTimeBar.fillAmount;                                     // ピンクゲージが減る前の位置
        float pinkBarStartTime = Time.time;                                                 // スタートの時間を入れる
        while (true)
        {
            float pinkBarEndTime = Mathf.Min(Time.time - pinkBarStartTime, 0.5f) / 0.5f;    // 0.0～1.0が入る     
            pinkTimeBar.fillAmount = Mathf.Lerp(pinkBarStartPos, timeBar.fillAmount, pinkBarEndTime);
            if (pinkBarEndTime == 1)
                break;
            yield return null;
        }

        yield return new WaitForSeconds(1);

        KeyBoardReset();
        answer.SetActive(false);
        questionText.SetText("");
        timeText.SetText("");
        serifText.SetText("");
        catAnim.SetBool("move", false);
        yield return new WaitForSeconds(1);
    }

    //持ち時間終了後スコア表示へ
    IEnumerator TimeUp(bool marubatsu)
    {
        timeUpText.SetText("TimeUp!");
        audio.PlayOneShot(se[2]);
        timeText.SetText(keyTime.ToString("F2") + "秒");

        if (marubatsu)                                                                      // 正解して時間がなくなったら
            answer.SetActive(true);
        else
            notAnswer.SetActive(true);                                                      // 間違えて時間がなくなったら

        // 持ち時間ゲージのバーを減らす動き
        yield return new WaitForSeconds(0.5f);
        float pinkBarStartPos = pinkTimeBar.fillAmount;                                     // ピンクゲージが減る前の位置
        float pinkBarStartTime = Time.time;                                                 // スタートの時間を入れる
        while (true)
        {
            float pinkBarEndTime = Mathf.Min(Time.time - pinkBarStartTime, 0.5f) / 0.5f;    // 0.0～1.0が入る     
            pinkTimeBar.fillAmount = Mathf.Lerp(pinkBarStartPos, timeBar.fillAmount, pinkBarEndTime);
            if (pinkBarEndTime == 1)
                break;
            yield return null;
        }

        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("ScoreScene_Ranking");
    }
}
