using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaCensor : MonoBehaviour
{
    [SerializeField]
    private GameObject TextField;

    [SerializeField]
    private GameObject GameManager;

    [SerializeField]
    private GameObject JsonReader;
    [SerializeField]
    int CallCodeNum = 0;
    private Gamemanager ScrGamemanager => GameManager.GetComponent<Gamemanager>();
    private LabelController ScrLabelController => TextField.GetComponent<LabelController>();
    private JsonReader ScrJsonReader => JsonReader.GetComponent<JsonReader>();

    [Tooltip("強制イベントか否か")]
    [SerializeField]
    private bool Constrain = false;

    [Tooltip("イベント後に遷移させたいシーンがあるならここにシーン名を記述する")]
    [SerializeField]
    private string NextScene = "";

    //イベントを読んだか判定するフラグ
    private bool ReadFlag = false;
    private void CallEvent()
    {
        if (ScrLabelController.ActionNow)
        {
            if (Constrain)
            {
                ScrLabelController.InterruptEvent = true;
                //台詞表示処理が生じているならそれが終わるまでWaitする
                StartCoroutine(WaitCallevent());
            }
            else
            {
                ScrGamemanager.StackWords = CallCodeNum;
            }
        }
        else
        {
            if (Constrain)
            {
                //JsonReaderにCallする台詞の範囲をチェックさせる
                ScrGamemanager.StackWords = 0;
                global::JsonReader.Singleton.CensorCallevent(CallCodeNum, NextScene, Constrain);
            }
            else
            {
                ScrGamemanager.StackWords = CallCodeNum;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!ReadFlag)
            {
                ReadFlag = true;
                CallEvent();
            }
        }
    }

    IEnumerator WaitCallevent()
    {
        while (ScrLabelController.ActionNow)
        {
            yield return null;
        }
        ScrJsonReader.CensorCallevent(CallCodeNum, NextScene, Constrain);
        ScrGamemanager.StackWords = 0;
        yield break;
    }
}
