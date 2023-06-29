using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �R�}���h���͂ŏ����𓭂�����@�\��ǉ��i�Ȃ������̊��p���@�͖���j
/// </summary>
public class Command : MonoBehaviour
{
    private List<KeyCode> inputBuffer = new List<KeyCode>();
    private Dictionary<string, List<KeyCode>> commandList = new Dictionary<string, List<KeyCode>>();
    private int lastInputFrame = -1;

    // �R�}���h���X�g��������
    private void InitializeCommandList()
    {
        // Hadouken�R�}���h��o�^
        List<KeyCode> hadouken = new List<KeyCode>();
        hadouken.Add(KeyCode.DownArrow);
        hadouken.Add(KeyCode.RightArrow);
        hadouken.Add(KeyCode.F);
        commandList.Add("Hadouken", hadouken);

        // Shoryuken�R�}���h��o�^
        List<KeyCode> shoryuken = new List<KeyCode>();
        shoryuken.Add(KeyCode.RightArrow);
        shoryuken.Add(KeyCode.DownArrow);
        shoryuken.Add(KeyCode.RightArrow);
        shoryuken.Add(KeyCode.F);
        commandList.Add("Shoryuken", shoryuken);
    }

    private void Start()
    {
        InitializeCommandList();
    }

    private void Update()
    {
        CheckInput(); // �L�[���͂̏���
        CheckCommand(); // �R�}���h�̏���
        RemoveExcessInput(); // ���̓o�b�t�@�̍폜
        OutputCommandHistory(); // �R�}���h�����̏o��
    }

    // �L�[���͂̏���
    private void CheckInput()
    {
        bool isInput = false; // �L�[���͂����邩�ǂ����������t���O

        // ���͂��ꂽ�L�[���o�b�t�@�ɒǉ�
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                inputBuffer.Add(keyCode);
                isInput = true;
            }
        }

        if (isInput)
        {
            lastInputFrame = Time.frameCount;
        }
    }

    // �R�}���h�̏���
    private void CheckCommand()
    {
        // �o�b�t�@���R�}���h���X�g�̂����ꂩ�ƈ�v����ꍇ�A�R�}���h�����s
        foreach (KeyValuePair<string, List<KeyCode>> command in commandList)
        {
            if (inputBuffer.Count < command.Value.Count)
            {
                continue;
            }

            bool isMatch = true;
            for (int i = 0; i < command.Value.Count; i++)
            {
                if (inputBuffer[inputBuffer.Count - command.Value.Count + i] != command.Value[i])
                {
                    isMatch = false;
                    break;
                }
            }

            if (isMatch)
            {
                Debug.Log("Command: " + command.Key);
                inputBuffer.Clear();
                break;
            }
        }
    }

    // ���̓o�b�t�@�̍폜
    private void RemoveExcessInput()
    {
        int currentFrame = Time.frameCount;
        if (currentFrame - lastInputFrame > 10)
        {
            inputBuffer.Clear();
            lastInputFrame = -1;
        }

        if (inputBuffer.Count > 10)
        {
            inputBuffer.RemoveAt(0);
        }
    }

    // �R�}���h�����̏o��
    private void OutputCommandHistory()
    {
        string commandHistoryText = "";
        foreach (KeyCode keycode in inputBuffer)
        {
            commandHistoryText += keycode.ToString() + ",";
        }
        Debug.Log("Command History: " + commandHistoryText);
    }

}
