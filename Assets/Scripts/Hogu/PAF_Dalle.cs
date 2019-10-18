﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAF_Dalle : MonoBehaviour
{
    [SerializeField] BoxCollider collider = null;
    [SerializeField, Range(1,10)] float speedDecay = 2;
    bool isFalling = false;
    bool isLeft = false;
    public bool Fell { get; private set; } = false;



    private void Update()
    {
        if (!isFalling) return;
        collider.size = Vector3.MoveTowards(collider.size, new Vector3(0, 0, collider.size.z), Time.deltaTime * speedDecay);
        if (collider.size.x <= 0)
        {
            collider.enabled = false;
            isFalling = false;
            return;
        }
        collider.center = Vector3.MoveTowards(collider.center, collider.center + (isLeft ? Vector3.right : Vector3.left) * .0085f * speedDecay, Time.deltaTime * speedDecay);

    }

    public void Fall(bool _isLeft)
    {
        if (!collider) collider = GetComponent<BoxCollider>();
        if (!collider) return;
        if (GetComponent<Renderer>()) GetComponent<Renderer>().enabled = false;
        isFalling = true;
        isLeft = _isLeft;
        Fell = true;
    }



}
