using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using TMPro;

public class JsonScoreSave : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;        // �Z�[�u����X�R�A
    [SerializeField] TMP_Text nameText;         // �Z�[�u���閼�O


    // �t�@�C����
    string path = "score";

    // �ǂݏ�������f�[�^�`��
    public struct Data           // struct�͍\����
    {
        public int score;        // �����^�̍���(�X�R�A)
        public string name;      // ������^�̍���(���O)
        public int ather;        // �����^�̍���(���_�Ґ�)

        // �R���X�g���N�^
        public Data(int score, string name,int ather)
        {
            this.score = score;  // this��t�����瓯���ϐ����ł�struct���̕����w��
            this.name = name;
            this.ather = ather;
        }
    }

    // json�^�ŕۑ�������
    string SavePath(string path) // ������path�̏��ɕۑ���̃t�@�C�����ɂȂ�
        => $"{Application.persistentDataPath}/{path}.json";

    public void SaveCall(int atherCount)
    {
        // �������݃f�[�^�����
        int scoreData = int.Parse(scoreText.text);
        string scoreName = nameText.text;
        int scoreAther = atherCount;
        Data saveData = new Data(scoreData, scoreName, scoreAther);

        // ��������
        Save(saveData, path);
        Debug.Log(SavePath(path));
        // �������݃f�[�^�̊m�F
        Debug.Log($"�ۑ��������O:{scoreName}  �ۑ������X�R�A:{scoreData} �ۑ��������_�Ґ�:{scoreAther}");
    }

    public Data LoadCall()
    {
        // �ǂݍ���
        Data loadData = Load(path);

        // �ǂݍ��݃f�[�^�̊m�F
        Debug.Log($"����a�F{loadData.score} �@����b�F{loadData.name} ����c�F{loadData.ather}");

        return loadData;
    }

    void Save(Data data, string path)
    {
        using (StreamWriter sw = new StreamWriter(SavePath(path), false))
        {
            string jsonstr = JsonUtility.ToJson(data, true);
            sw.Write(jsonstr);
            sw.Flush();
        }
    }

    Data Load(string path)
    {
        if (File.Exists(SavePath(path)))  // �f�[�^�����݂���ꍇ�͕Ԃ�
        {
            using (StreamReader sr = new StreamReader(SavePath(path)))
            {
                string datastr = sr.ReadToEnd();
                return JsonUtility.FromJson<Data>(datastr);
            }
        }

        // ���݂��Ȃ��ꍇ��default��ԋp
        return default;
    }
}
