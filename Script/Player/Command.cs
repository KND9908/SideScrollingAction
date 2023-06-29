using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// コマンド入力で処理を働かせる機能を追加（なお処理の活用方法は未定）
/// </summary>
public class Command : MonoBehaviour
{
    private List<KeyCode> inputBuffer = new List<KeyCode>();
    private Dictionary<string, List<KeyCode>> commandList = new Dictionary<string, List<KeyCode>>();
    private int lastInputFrame = -1;

    // コマンドリストを初期化
    private void InitializeCommandList()
    {
        // Hadoukenコマンドを登録
        List<KeyCode> hadouken = new List<KeyCode>();
        hadouken.Add(KeyCode.DownArrow);
        hadouken.Add(KeyCode.RightArrow);
        hadouken.Add(KeyCode.F);
        commandList.Add("Hadouken", hadouken);

        // Shoryukenコマンドを登録
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
        CheckInput(); // キー入力の処理
        CheckCommand(); // コマンドの処理
        RemoveExcessInput(); // 入力バッファの削除
        OutputCommandHistory(); // コマンド履歴の出力
    }

    // キー入力の処理
    private void CheckInput()
    {
        bool isInput = false; // キー入力があるかどうかを示すフラグ

        // 入力されたキーをバッファに追加
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

    // コマンドの処理
    private void CheckCommand()
    {
        // バッファがコマンドリストのいずれかと一致する場合、コマンドを実行
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

    // 入力バッファの削除
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

    // コマンド履歴の出力
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
