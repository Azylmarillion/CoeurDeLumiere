using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDL_Punch : MonoBehaviour
{
    [SerializeField] Collider hitbox = null;
    [SerializeField, Range(0,5)] float punchStunTime = .5f;
     
    private void OnTriggerEnter(Collider other)
    {
        CDL_Player _pl = other.GetComponent<CDL_Player>();
        if (_pl) _pl.Stun(punchStunTime);
    }


    public void Punch()
    {
        if (!hitbox) return;
        hitbox.enabled = true;
        StartCoroutine(DelayPunch());
    }

    IEnumerator DelayPunch()
    {
        yield return new WaitForSeconds(.1f);
        hitbox.enabled = false;
    }

}
