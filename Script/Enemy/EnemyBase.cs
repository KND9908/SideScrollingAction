using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEditor;
public class EnemyBase : CreatureBase
{
#if UNITY_EDITOR
    //設定されたパラメータをインスペクター上から見れるようにする
    [CustomEditor(typeof(CreatureBase))]
#endif
    protected Rigidbody _Rigidbody => GetComponent<Rigidbody>();

    [Tooltip("プレイヤー　距離を参照するために使用　インスペクターからの操作は基本デバッグでしか使わない想定")]
    [SerializeField]
    protected GameObject Player;
    public GameObject GetSetPlayer { get { return Player; } set { Player = value; } }

    [Tooltip("ダメージを受けたときに表示する数字のアイコン")]
    [SerializeField]
    protected GameObject DamageText;
    public GameObject GetSetDamageText { get { return GetSetDamageText; } set { GetSetDamageText = value; } }
    protected void VisibleDamage(int num)
    {
        GameObject damagetext;
        damagetext = Instantiate(DamageText);
        GameObject tx = damagetext.transform.GetChild(0).gameObject;
        TextMeshProUGUI txm = tx.GetComponent<TextMeshProUGUI>();
        txm.text = num.ToString();
        txm.color += new Color(0, 0, 0, 0);
        damagetext.transform.position = new Vector3(this.transform.position.x, transform.position.y, 0);
    }

}

