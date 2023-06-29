using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeOut : MonoBehaviour
{
    [SerializeField]
    string GotoScene = "";
    Image image;
    public bool NowFading = false;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        //現在のシーンがタイトルの場合
        if (SceneManager.GetActiveScene().name == "Title")
        {
            //タイトル画面でBを押したらフェードアウト
            if (Input.GetButtonDown("Jump"))
            {
                FadeOutCall(GotoScene);
            }
        }
    }
    public void FadeOutCall(string str)
    {
        GotoScene = str;
        image.enabled = true; //オフにしていたPanelのImageコンポーネントをオンに変更
        StartCoroutine(DoFadeOut());
    }
    IEnumerator DoFadeOut()
    {
        //Debug.Log(image.color.a);
        while (image.color.a < 1.0f)
        {
            image.color += new Color(0, 0, 0, 1.0f / 120f); //Imageのカラーを変更
            yield return null;
        }
        if (GotoScene != "")
        {
            SceneManager.LoadScene(GotoScene);
        }
        yield break;
    }
    public void FadeInCall()
    {
        image.enabled = true; //オフにしていたPanelのImageコンポーネントをオンに変更
        StartCoroutine(DoFadeOut());
    }
    IEnumerator DoFadeIn()
    {
        Debug.Log(image.color.a);
        while (image.color.a > 0)
        {
            image.color -= new Color(0, 0, 0, 1.0f / 120f); //Imageのカラーを変更
            yield return null;
        }
        yield break;
    }

}
