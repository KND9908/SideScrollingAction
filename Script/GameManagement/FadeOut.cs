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
    public void FadeOutCall(string str)
    {
        GotoScene = str;
        image.enabled = true; //�I�t�ɂ��Ă���Panel��Image�R���|�[�l���g���I���ɕύX
        StartCoroutine(DoFadeOut());
    }
    IEnumerator DoFadeOut()
    {
        //Debug.Log(image.color.a);
        while (image.color.a < 1.0f)
        {
            image.color += new Color(0, 0, 0, 1.0f / 120f); //Image�̃J���[��ύX
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
        image.enabled = true; //�I�t�ɂ��Ă���Panel��Image�R���|�[�l���g���I���ɕύX
        StartCoroutine(DoFadeOut());
    }
    IEnumerator DoFadeIn()
    {
        Debug.Log(image.color.a);
        while (image.color.a > 0)
        {
            image.color -= new Color(0, 0, 0, 1.0f / 120f); //Image�̃J���[��ύX
            yield return null;
        }
        yield break;
    }

}
