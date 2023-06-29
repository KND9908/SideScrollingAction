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

    [Tooltip("�����C�x���g���ۂ�")]
    [SerializeField]
    private bool _Constrain = false;

    [Tooltip("�C�x���g��ɑJ�ڂ��������V�[��������Ȃ炱���ɃV�[�������L�q����")]
    [SerializeField]
    private string _NextScene = "";

    //�C�x���g��ǂ񂾂����肷��t���O
    private bool _ReadFlag = false;
    private void _CallEvent()
    {
        if (_CompLabelController.isAction)
        {
            if (_Constrain)
            {
                _CompLabelController.InterruptEvent = true;
                //�䎌�\�������������Ă���Ȃ炻�ꂪ�I���܂�Wait����
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
                //JsonReader��Call����䎌�͈̔͂��`�F�b�N������
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
        //�䎌�\�������������Ă���Ȃ炻�ꂪ�I���܂�Wait����
        while (_CompLabelController.isAction)
        {
            yield return null;
        }
        _CompJsonReader.Callevent(CallCodeNum, _NextScene, _Constrain);
        _CompGamemanager.StackWords = 0;
        yield break;
    }
}
