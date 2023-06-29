using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    public Player Player { get; set; }

    private Vector3 _BasePos;
    private Vector3 _ShotToEnd;
    // Start is called before the first frame update
    int cnt = 1;
    void Start()
    {
        _ShotToEnd = new Vector3(Player.transform.position.x - transform.position.x, Player.transform.position.y - transform.position.y,0);
        _BasePos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        cnt++;
        if (cnt < 80)
        {
            transform.position = new Vector3(_BasePos.x + (_ShotToEnd.x / 50f) * cnt , _BasePos.y + (_ShotToEnd.y / 50f) * cnt , 0);
        }else
            Destroy(this.gameObject);
    }
}
