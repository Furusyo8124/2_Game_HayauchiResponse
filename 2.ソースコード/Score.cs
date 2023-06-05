using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NCMB;

public class Score : MonoBehaviour
{
    #region // �X�R�A�p�l���p
    [SerializeField] TMP_Text wordCountText;                    // �łĂ�������
    [SerializeField] TMP_Text rankText;                         // �����N
    [SerializeField] TMP_Text commentText;                      // �������ɉ������R�����g
    [SerializeField] GameObject scorePanel;                     // �X�R�A�p�l��
    [SerializeField] Image rankImagePos;                        // �����N�C���[�W�\���ʒu
    [SerializeField] Sprite[] rankSprits = new Sprite[5];       // �����N�C���[�W�X�v���C�g
    [SerializeField] GameObject retryKey;                       // ���g���C�{�^��
    [SerializeField] GameObject titleKey;                       // �^�C�g���o�b�N�{�^��
    [SerializeField] GameObject rankingKey;                     // �����L���O�{�^��
    #endregion

    #region // �n�C�X�R�A�p�l���p
    [SerializeField] JsonScoreSave jsonScoreSave;               // �f�[�^�̓ǂݏ���
    [SerializeField] GameObject hiscorePanel;                   // �n�C�X�R�A�p�l��
    [SerializeField] TMP_Text displayName;                      // �n�C�X�R�A�o�^�\���l�[��
    [SerializeField] TMP_Text displayScore;                     // �n�C�X�R�A�o�^�\��
    [SerializeField] TMP_Text displayAther;                     // �n�C�X�R�A���_�ҕ\��
    [SerializeField] TMP_InputField inputField;                 // ���O����
    int atherNum;                                               // �n�C�X�R�A���_�Ґ�
    bool isCount;
    #endregion

    string[] ranks 
        = { "C�����N", "B�����N", "A�����N", "S�����N", "SS�����N" };

    int wordCount;                                              // �v���C���[���łĂ�������
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
        // �����L���O�p�l�����\������ĂȂ����ɔ�������
        GameObject rankingBoard = GameObject.Find("HiScore");
        if (rankingBoard == null && retryKey.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.R))                // ���g���C
            {
                SceneManager.LoadScene("GameScene_FullScreen");
            }
            else if (Input.GetKeyDown(KeyCode.T))           // �^�C�g���o�b�N
            {
                SceneManager.LoadScene("TitleScene");
            }
            else if (Input.GetKeyDown(KeyCode.E))           // �n�C�X�R�A�\��
            {
                //naichilab.RankingLoader.Instance.SendScoreAndShowRanking(wordCount);
                hiscorePanel.SetActive(true);
            }
        }

        if (hiscorePanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Delete))           // �n�C�X�R�A�p�l�������
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

    // �f�[�^���[�h���̉�ʕ\���X�V
    void DataLoadMove()
    {
        data = jsonScoreSave.LoadCall();
        displayName.SetText(data.name);
        displayScore.SetText(data.score.ToString());
        displayAther.SetText("���_�ґ�" + data.ather + "��");
    }

    // �X�R�A�{�[�h��\��������
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

    // �łĂ��������ɂ���ă����N�̐U�蕪��
    void RankCheck(int count)
    {
        if (count <= 5)                         // C rank
        {
            int nextCount = 6 - count;
            rankText.color = new Color32(150, 70, 0, 255);
            rankText.SetText(ranks[0]);
            commentText.SetText("����" + nextCount + "�Ń����N���i�I");
            rankImagePos.sprite = rankSprits[0];
        }
        else if (count >= 6 && count <= 10)     // B rank
        {
            int nextCount = 11 - count;
            rankText.color = Color.gray;
            rankText.SetText(ranks[1]);
            commentText.SetText("����" + nextCount + "�Ń����N���i�I");
            rankImagePos.sprite = rankSprits[1];
        }
        else if (count >= 11 && count <= 16)    // A rank
        {
            int nextCount = 17 - count;
            rankText.color = Color.yellow;
            rankText.SetText(ranks[2]);
            commentText.SetText("����" + nextCount + "�Ń����N���i�I");
            rankImagePos.sprite = rankSprits[2];
        }
        else if (count >= 17 && count <= 19)    // S rank
        {
            int nextCount = 20 - count;
            rankText.color = new Color32(180, 0, 255, 255);
            rankText.SetText(ranks[3]);
            commentText.SetText("����" + nextCount + "�Ń����N���i�I");
            rankImagePos.sprite = rankSprits[3];
        }
        else if (count >= 20)                   // SS rank
        {
            rankText.color = Color.red;
            rankText.SetText(ranks[4]);
            commentText.SetText("�ō������N�I�_�����X�|���T�[�I");
            rankImagePos.sprite = rankSprits[4];
        }
    }

}
