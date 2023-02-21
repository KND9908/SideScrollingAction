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

    [Tooltip("�����C�x���g���ۂ�")]
    [SerializeField]
    private bool Constrain = false;

    [Tooltip("�C�x���g��ɑJ�ڂ��������V�[��������Ȃ炱���ɃV�[�������L�q����")]
    [SerializeField]
    private string NextScene = "";

    //�C�x���g��ǂ񂾂����肷��t���O
    private bool ReadFlag = false;
    private void CallEvent()
    {
        if (ScrLabelController.ActionNow)
        {
            if (Constrain)
            {
                ScrLabelController.InterruptEvent = true;
                //�䎌�\�������������Ă���Ȃ炻�ꂪ�I���܂�Wait����
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
                //JsonReader��Call����䎌�͈̔͂��`�F�b�N������
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
