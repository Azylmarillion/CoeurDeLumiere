using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Player : MonoBehaviour
{
    [SerializeField] Renderer playerRenderer = null;
    [SerializeField] bool stunned = false;

    void Update()
    {
        Move();
    }

    void Move()
    {
        if(!stunned) transform.position += (new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))) * .1f;
    }

    public void GetStunnedFam(float _stunTime)
    {
        if (!playerRenderer) return;
        StartCoroutine(StunFlashTimer(_stunTime));
        InvokeRepeating("Flash", 0, .1f);
    }
    
    void Flash()
    {
        playerRenderer.enabled = !playerRenderer.enabled;
    }

    IEnumerator StunFlashTimer(float _stunTime)
    {
        stunned = true;
        yield return new WaitForSeconds(_stunTime);
        CancelInvoke("Flash");
        playerRenderer.enabled = true;
        stunned = false;
    }
    
}
