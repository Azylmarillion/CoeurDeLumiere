using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAF_ConfusedStarsBehaviour : MonoBehaviour
{
    private void OnDestroy()
    {
        GameObject _parent = GetComponentInParent<Transform>().gameObject;
        if (_parent) Destroy(_parent);
    }
}
