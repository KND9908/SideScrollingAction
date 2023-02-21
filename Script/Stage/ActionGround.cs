using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;
public class ActionGround : MonoBehaviour
{
    [SerializeField]
    [Tooltip("è∞ÇÃë¨ìx")]
    private float MoveSpeed;

    [SerializeField]
    private float GoalPosition;

    public Ease Ease_Type;

    public float Amount_of_movement = 2.0f;

    private int movevec = 1;

    private Rigidbody rb;

    [Tooltip("èâä˙à íu")]
    Vector3 deftransform;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        deftransform = transform.position;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(new Vector3(transform.position.x, deftransform.y + (Mathf.PingPong(Time.time,MoveSpeed) * movevec), transform.position.z));
    }
}
