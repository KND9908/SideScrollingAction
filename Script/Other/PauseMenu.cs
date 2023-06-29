using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]private GameObject pauseMenuUI;      // ポーズメニューのUI
    [SerializeField]private GameObject quitDialogUI;     // 終了確認ダイアログのUI

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)    // ゲームが再生中であれば
            {
                Pause();    // ポーズ処理を行う
            }
            else
            {
                Resume();   // 再開処理を行う
            }
        }
    }

    void Pause()
    {
        Time.timeScale = 0;     // ゲームを停止する
        pauseMenuUI.SetActive(true);    // ポーズメニューのUIを表示する
    }

    public void Resume()
    {
        Time.timeScale = 1;     // ゲームを再生する
        pauseMenuUI.SetActive(false);   // ポーズメニューのUIを非表示にする
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;     // ゲームを再生する
        SceneManager.LoadScene("MainMenu");     // メインメニュー画面に遷移する
    }

    public void OpenQuitDialog()
    {
        quitDialogUI.SetActive(true);   // 終了確認ダイアログのUIを表示する
    }

    public void CloseQuitDialog()
    {
        quitDialogUI.SetActive(false);  // 終了確認ダイアログのUIを非表示にする
    }

    public void QuitGame()
    {
        quitDialogUI.SetActive(true);   // 終了確認ダイアログのUIを表示する
    }

    public void ConfirmQuitGame()
    {
        Application.Quit();     // アプリケーションを終了する
    }

    public void CancelQuitGame()
    {
        quitDialogUI.SetActive(false);  // 終了確認ダイアログのUIを非表示にする
    }
}