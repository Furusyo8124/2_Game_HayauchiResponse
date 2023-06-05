using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    #region//�C���X�y�N�^�[��ɕR�Â������ϐ�
    [SerializeField] TMP_Text title_1;
    [SerializeField] TMP_Text title_2;
    [SerializeField] TMP_Text ruleText;
    [SerializeField] TMP_Text startText;
    [SerializeField] GameObject keyBoard;
    [SerializeField] GameObject ruleBook;
    #endregion

    // �v���C�x�[�g�ϐ�
    Animator titleAnim_1;
    Animator titleAnim_2;

    private void Awake()
    {
        titleAnim_1 = title_1.GetComponent<Animator>();
        titleAnim_2 = title_2.GetComponent<Animator>();
    }

    void Start()
    {
        StartCoroutine(TitleSlide());
    }

    void Update()
    {
        // F�L�[�Ő�������\���E��\��
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!ruleBook.activeSelf)
            {
                SetActiveFalse();
                ruleBook.SetActive(true);
            }
            else if (ruleBook.activeSelf)
            {
                SetActiveTrue();
                ruleBook.SetActive(false);
            }
        }

        // J�L�[�ŃQ�[���X�^�[�g
        if (Input.GetKeyDown(KeyCode.J))
        {
            if(!ruleBook.activeSelf)
                SceneManager.LoadScene("GameScene_FullScreen"/*"GameScene"*/);
        }
    }

    // ��������\��
    void SetActiveTrue()
    {
        title_1.enabled = true;
        title_2.enabled = true;
        ruleText.enabled = true;
        startText.enabled = true;
        keyBoard.SetActive(true);
    }

    // ���������\��
    void SetActiveFalse()
    {
        title_1.enabled = false;
        title_2.enabled = false;
        ruleText.enabled = false;
        startText.enabled = false;
        keyBoard.SetActive(false);
    }

    // �^�C�g���̓���
    IEnumerator TitleSlide()
    {
        titleAnim_1.SetBool("move", true);
        yield return new WaitForSeconds(1);
        titleAnim_2.SetBool("slide", true);
        yield return new WaitForSeconds(1.5f);
        keyBoard.SetActive(true);
        ruleText.SetText("�V�ѕ���");
        startText.SetText("���X�^�[�g");
    }

}
