using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]private GameObject pauseMenuUI;      // �|�[�Y���j���[��UI
    [SerializeField]private GameObject quitDialogUI;     // �I���m�F�_�C�A���O��UI

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)    // �Q�[�����Đ����ł����
            {
                Pause();    // �|�[�Y�������s��
            }
            else
            {
                Resume();   // �ĊJ�������s��
            }
        }
    }

    void Pause()
    {
        Time.timeScale = 0;     // �Q�[�����~����
        pauseMenuUI.SetActive(true);    // �|�[�Y���j���[��UI��\������
    }

    public void Resume()
    {
        Time.timeScale = 1;     // �Q�[�����Đ�����
        pauseMenuUI.SetActive(false);   // �|�[�Y���j���[��UI���\���ɂ���
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;     // �Q�[�����Đ�����
        SceneManager.LoadScene("MainMenu");     // ���C�����j���[��ʂɑJ�ڂ���
    }

    public void OpenQuitDialog()
    {
        quitDialogUI.SetActive(true);   // �I���m�F�_�C�A���O��UI��\������
    }

    public void CloseQuitDialog()
    {
        quitDialogUI.SetActive(false);  // �I���m�F�_�C�A���O��UI���\���ɂ���
    }

    public void QuitGame()
    {
        quitDialogUI.SetActive(true);   // �I���m�F�_�C�A���O��UI��\������
    }

    public void ConfirmQuitGame()
    {
        Application.Quit();     // �A�v���P�[�V�������I������
    }

    public void CancelQuitGame()
    {
        quitDialogUI.SetActive(false);  // �I���m�F�_�C�A���O��UI���\���ɂ���
    }
}