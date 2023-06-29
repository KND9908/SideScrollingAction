using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaCensor : MonoBehaviour
{
    [SerializeField]
    private GameObject _ObjLabelController;

    [SerializeField]
    private GameObject _GameManager;

    [SerializeField]
    private GameObject _JsonReader;
    [SerializeField]
    int CallCodeNum = 0;
    private Gamemanager _CompGamemanager => _GameManager.GetComponent<Gamemanager>();
    private LabelController _CompLabelController => _ObjLabelController.GetComponent<LabelController>();
    private JsonReader _CompJsonReader => _JsonReader.GetComponent<JsonReader>();

    [Tooltip("強制イベントか否か")]
    [SerializeField]
    private bool _Constrain = false;

    [Tooltip("イベント後に遷移させたいシーンがあるならここにシーン名を記述する")]
    [SerializeField]
    private string _NextScene = "";

    //イベントを読んだか判定するフラグ
    private bool _ReadFlag = false;
    private void _CallEvent()
    {
        if (_CompLabelController.isAction)
        {
            if (_Constrain)
            {
                _CompLabelController.InterruptEvent = true;
                //台詞表示処理が生じているならそれが終わるまでWaitする
                StartCoroutine(WaitCallevent());
            }
            else
            {
                _CompGamemanager.StackWords = CallCodeNum;
            }
        }
        else
        {
            if (_Constrain)
            {
                //JsonReaderにCallする台詞の範囲をチェックさせる
                _CompGamemanager.StackWords = 0;
                global::JsonReader.Singleton.Callevent(CallCodeNum, _NextScene, _Constrain);
            }
            else
            {
                _CompGamemanager.StackWords = CallCodeNum;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!_ReadFlag)
            {
                _ReadFlag = true;
                _CallEvent();
            }
        }
    }

    IEnumerator WaitCallevent()
    {
        //台詞表示処理が生じているならそれが終わるまでWaitする
        while (_CompLabelController.isAction)
        {
            yield return null;
        }
        _CompJsonReader.Callevent(CallCodeNum, _NextScene, _Constrain);
        _CompGamemanager.StackWords = 0;
        yield break;
    }
}
