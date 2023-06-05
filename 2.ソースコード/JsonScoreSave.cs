using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using TMPro;

public class JsonScoreSave : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;        // セーブするスコア
    [SerializeField] TMP_Text nameText;         // セーブする名前


    // ファイル名
    string path = "score";

    // 読み書きするデータ形式
    public struct Data           // structは構造体
    {
        public int score;        // 整数型の項目(スコア)
        public string name;      // 文字列型の項目(名前)
        public int ather;        // 整数型の項目(同点者数)

        // コンストラクタ
        public Data(int score, string name,int ather)
        {
            this.score = score;  // thisを付けたら同じ変数名でもstruct内の方を指す
            this.name = name;
            this.ather = ather;
        }
    }

    // json型で保存先を作る
    string SavePath(string path) // 引数のpathの所に保存先のファイル名になる
        => $"{Application.persistentDataPath}/{path}.json";

    public void SaveCall(int atherCount)
    {
        // 書き込みデータを作る
        int scoreData = int.Parse(scoreText.text);
        string scoreName = nameText.text;
        int scoreAther = atherCount;
        Data saveData = new Data(scoreData, scoreName, scoreAther);

        // 書き込み
        Save(saveData, path);
        Debug.Log(SavePath(path));
        // 書き込みデータの確認
        Debug.Log($"保存した名前:{scoreName}  保存したスコア:{scoreData} 保存した同点者数:{scoreAther}");
    }

    public Data LoadCall()
    {
        // 読み込み
        Data loadData = Load(path);

        // 読み込みデータの確認
        Debug.Log($"項目a：{loadData.score} 　項目b：{loadData.name} 項目c：{loadData.ather}");

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
        if (File.Exists(SavePath(path)))  // データが存在する場合は返す
        {
            using (StreamReader sr = new StreamReader(SavePath(path)))
            {
                string datastr = sr.ReadToEnd();
                return JsonUtility.FromJson<Data>(datastr);
            }
        }

        // 存在しない場合はdefaultを返却
        return default;
    }
}
