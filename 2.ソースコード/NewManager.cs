using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NCMB;

public class NewManager : MonoBehaviour
{
    #region//�C���X�y�N�^�[��̊e�e�L�X�g
    [SerializeField] TMP_Text levelText;                                   // ���݂̃��x��
    [SerializeField] TMP_Text questionText;                                // ���
    [SerializeField] TMP_Text missText;                                    // �����ԈႢ��
    [SerializeField] TMP_Text timeText;                                    // ����������
    [SerializeField] TMP_Text timeLimitText;                               // �c�莞��
    [SerializeField] TMP_Text explanationText;                             // �ŏ��ɏo�Ă���V�ѕ�
    [SerializeField] TMP_Text serifText;                                   // �E��̃Z���t
    [SerializeField] TMP_Text timeUpText;                                  // ���Ԑ؂ꎞ
    [SerializeField] TMP_Text startText;                                   // �Q�[���X�^�[�g
    #endregion

    #region//�C���X�y�N�^�[��̊e�Q�[���I�u�W�F�N�g
    [SerializeField] GameObject answer;                                    // �������́Z�C���X�g
    [SerializeField] GameObject notAnswer;                                 // �s�������́~�C���X�g
    [SerializeField] GameObject serifCat;                                  // �E��̃R�����g�L���b�g�̃C���X�g
    [SerializeField] GameObject[] keyBoardChilds = new GameObject[26];     // �e�L�[
    #endregion

    #region//�C���X�y�N�^�[��̊e�摜�n
    [SerializeField] Image timeBar;                                        // �������Ԃ̗΂̕�
    [SerializeField] Image pinkTimeBar;                                    // �������ԍ��ꂽ���̃s���N�̕�
    #endregion

    // ���ʉ�
    [SerializeField] AudioClip[] se = new AudioClip[3];

    #region//�v���C�x�[�g�ϐ�
    string[] catSrif 
        = { "�͂₢�j���I", "�܂��܂����j��", "�̂�܂��j��" };            // ������ł������̑����ɉ������Z���t
    int currentLevel = 0;                                                  // ��背�x���̐��l
    string level = "LEVEL ";
    float keyTime;
    float startTime;
    float keyTimeSum = 10;

    Animator catAnim;
    AudioSource audio;

    SpriteRenderer[] spriteRenderer = new SpriteRenderer[26];

    // ���x�����Ƃɑ������L�[
    KeyCode[][] levelRelease = new KeyCode[][]
    {
        new KeyCode[]{KeyCode.J},                                                               // ���x��1
        new KeyCode[]{KeyCode.F},                                                               // ���x��2
        new KeyCode[]{KeyCode.K,KeyCode.D},                                                     // ���x��3
        new KeyCode[]{KeyCode.G,KeyCode.H,KeyCode.A,KeyCode.S,KeyCode.L},                       // ���x��4
        new KeyCode[]{KeyCode.Q,KeyCode.W,KeyCode.P,KeyCode.O,KeyCode.E},                       // ���x��5
        new KeyCode[]{KeyCode.R,KeyCode.T,KeyCode.I,KeyCode.U,KeyCode.Y},                       // ���x��6
        new KeyCode[]{KeyCode.Z,KeyCode.X,KeyCode.M,KeyCode.N,KeyCode.B,KeyCode.C,KeyCode.V}    // ���x��7
    };

    List<KeyCode> useKeyCode = new List<KeyCode>();                        // ���Ɏg����p�����X�g
    #endregion

    // �X�R�A�����L���O�p
    public static int wordCount;                                           // �łĂ�������

    private void Awake()
    {
        // �L�[�{�[�h�̎擾
        for (int i = 0; i < keyBoardChilds.Length; i++)
        {
            spriteRenderer[i] = keyBoardChilds[i].GetComponent<SpriteRenderer>();
        }

        // �ŏ��̃��x���𓱓�
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

    // ���x���ɂ���Ďg����L�[�����点��
    void KeyBoardColor()
    {
        foreach (KeyCode keyCord in useKeyCode)
        {
            int i = (int)keyCord - (int)KeyCode.A;
            spriteRenderer[i].color = new Color32(255, 255, 255, 255);
        }
    }

    // �L�[�J���[���Z�b�g
    void KeyBoardReset()
    {
        for (int i = 0; i < spriteRenderer.Length; i++)
        {
            spriteRenderer[i].color = new Color32(255, 255, 255, 60);
        }
    }

    // ��ԍŏ������̉��o�A�V�ѕ��̃e�L�X�g
    IEnumerator RuleTextMove()
    {
        RectTransform rectTransform;
        rectTransform = explanationText.GetComponent<RectTransform>();
        yield return Slide(rectTransform,0);
        yield return FirstTextCheck();
        yield return Slide(rectTransform,1600);
        StartCoroutine(LevelTextMove());
    }

    // ���݂̃��x���̕\���̓���
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

    // ���x���Ŕ̓���
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

    // �e�L�X�g�X���C�h�����铮��
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

    // �ŏ��̗V�ѕ��e�L�X�g�̎��փ{�^��
    IEnumerator FirstTextCheck()
    {
        startText.SetText("���X�^�[�g�I");
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

    // ���̕\��
    IEnumerator Display(float delay)
    {
        yield return new WaitForSeconds(delay);
        int i = Random.Range(0, useKeyCode.Count);                          // �ǂ̉p�����o���������_���ɑI��
        questionText.SetText(useKeyCode[i].ToString());                     // ��肪�\�������
        audio.Stop();
        startTime = Time.time;                                              // ��肪�\������Ă�����͂����܂ł̎��Ԃ��v���J�n
    }

    // ���̕\���A�\�����ꂽ���Ƃ̃v���C���[�̓��̓`�F�b�N
    IEnumerator Question()
    {
        StartCoroutine(Display(Random.Range(1f, 3f)));
        startTime = 0;

        // �o�肳��Ă���v���C���[���������L�[�{�[�h�Ɠ����������Ă��邩�̃`�F�b�N
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
                        // �������L�[�����鏈��
                        int n = (int)j - (int)KeyCode.A;
                        spriteRenderer[n].color = new Color32(0, 255, 255, 255);
                    }
                    else if (questionText.text == "")
                    {

                    }
                    else
                    {
                        isOther = true;
                        // �������L�[�����鏈��
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

        // ���ƈႤ�L�[���������Ƃ�
        if (isOther)
        {
            audio.PlayOneShot(se[1]);
            audio.Play();

            // �������Ԃ̊Ǘ�
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

        // �����̃L�[���������Ƃ�
        else if (isHit)
        {
            audio.Play();
            audio.PlayOneShot(se[0]);
            wordCount++;                                                        // ���������������𑫂�
            if (currentLevel < levelRelease.Length)                             // ���x��MAX�łȂ���΃��x����1����
            {
                useKeyCode.AddRange(levelRelease[currentLevel]);
                currentLevel++;
            }

            // �������Ԃ̊Ǘ�
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

    // �s�������������̓���
    IEnumerator UnsuccessfulMove()
    {
        missText.SetText("MISS�I");
        serifText.SetText("���Ă��j���I");
        catAnim.SetBool("move", true);
        timeText.SetText(keyTime.ToString("F2") + "�b");
        notAnswer.SetActive(true);

        // �������ԃQ�[�W�̃o�[�����炷����
        yield return new WaitForSeconds(0.5f);
        float pinkBarStartPos = pinkTimeBar.fillAmount;                                     // �s���N�Q�[�W������O�̈ʒu
        float pinkBarStartTime = Time.time;                                                 // �X�^�[�g�̎��Ԃ�����
        while (true)
        {
            float pinkBarEndTime = Mathf.Min(Time.time - pinkBarStartTime, 0.5f) / 0.5f;    // 0.0�`1.0������     
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

    // �����������̓���
    IEnumerator SuccessMove()
    {
        // �����������ɂ���Ă̔L�̃R�����g�U�蕪��
        if (keyTime < 1)
            serifText.SetText(catSrif[0]);
        else if (keyTime >= 1 && keyTime < 3)
            serifText.SetText(catSrif[1]);
        else if (keyTime >= 3)
            serifText.SetText(catSrif[2]);

        catAnim.SetBool("move", true);
        timeText.SetText(keyTime.ToString("F2") + "�b");
        answer.SetActive(true);

        // �������ԃQ�[�W�̃o�[�����炷����
        yield return new WaitForSeconds(0.5f);
        float pinkBarStartPos = pinkTimeBar.fillAmount;                                     // �s���N�Q�[�W������O�̈ʒu
        float pinkBarStartTime = Time.time;                                                 // �X�^�[�g�̎��Ԃ�����
        while (true)
        {
            float pinkBarEndTime = Mathf.Min(Time.time - pinkBarStartTime, 0.5f) / 0.5f;    // 0.0�`1.0������     
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

    //�������ԏI����X�R�A�\����
    IEnumerator TimeUp(bool marubatsu)
    {
        timeUpText.SetText("TimeUp!");
        audio.PlayOneShot(se[2]);
        timeText.SetText(keyTime.ToString("F2") + "�b");

        if (marubatsu)                                                                      // �������Ď��Ԃ��Ȃ��Ȃ�����
            answer.SetActive(true);
        else
            notAnswer.SetActive(true);                                                      // �ԈႦ�Ď��Ԃ��Ȃ��Ȃ�����

        // �������ԃQ�[�W�̃o�[�����炷����
        yield return new WaitForSeconds(0.5f);
        float pinkBarStartPos = pinkTimeBar.fillAmount;                                     // �s���N�Q�[�W������O�̈ʒu
        float pinkBarStartTime = Time.time;                                                 // �X�^�[�g�̎��Ԃ�����
        while (true)
        {
            float pinkBarEndTime = Mathf.Min(Time.time - pinkBarStartTime, 0.5f) / 0.5f;    // 0.0�`1.0������     
            pinkTimeBar.fillAmount = Mathf.Lerp(pinkBarStartPos, timeBar.fillAmount, pinkBarEndTime);
            if (pinkBarEndTime == 1)
                break;
            yield return null;
        }

        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("ScoreScene_Ranking");
    }
}
