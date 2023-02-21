using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("フォーカスするプレイヤー")]
    GameObject Player;

    [SerializeField]
    [Tooltip("ライフアイコン")]
    GameObject LifeIcon;
    // ライフアイコンを管理する配列
    GameObject[] LifeIcons;

    [SerializeField]
    [Tooltip("ゲームマネージャー")]
    GameObject GameManager;

    [Tooltip("ゲームマネージャーのコンポーネントのgamemanager")]
    Gamemanager ScrGamemanager => GameManager.GetComponent<Gamemanager>();

    [Tooltip("Playerのコンポーネントのplayer")]
    Player ScrPlayer;

    void Start()
    {
        LifeIcons = new GameObject[10];
        ScrPlayer = Player.GetComponent<Player>();
        ScrPlayer.UpdateHP += UpdateDrawLifeIcon;
        for (int i = 0; i < ScrPlayer.GetSetMaxHP; i++)
        {
            GameObject Life;
            Life = Instantiate(LifeIcon);
            LifeIcons[i] = Life;
            Life.transform.SetParent(this.transform);
            Life.transform.localPosition = new Vector3(-300 + i * 60, 115, 0);
            Life.transform.localScale = new Vector3(1, 1, 1);
        }
    }
    // Update is called once per frame
    void Update()
    {
        VisibleLife();
    }

    //イベント発生中はライフアイコンを非表示にする
    private void VisibleLife()
    {
        for (int i = 0; i < LifeIcons.Length; i++)
        {
            if (LifeIcons[i] != null)
            {
                LifeIcons[i].GetComponent<Image>().enabled = ScrGamemanager.KeyLock ? false : true;
            }
        }
    }

    private void UpdateDrawLifeIcon(int HP)
    {
        for (int i = 0; i < LifeIcons.Length; i++)
        {
            if (LifeIcons[i] != null)
            {
                if (ScrPlayer.GetSetNowLife - 1 >= i && !LifeIcons[i].activeSelf)
                {
                    LifeIcons[i].SetActive(true);
                }
                else if (ScrPlayer.GetSetNowLife - 1 < i && LifeIcons[i].activeSelf)
                {
                    LifeIcons[i].SetActive(false);
                }
            }
        }

    }
}
