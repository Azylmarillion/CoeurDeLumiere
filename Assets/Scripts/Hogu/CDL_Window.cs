using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class CDL_Window : MonoBehaviour
{
    public WindowType CurrentWindowType = WindowType.None;
    [SerializeField] SphereCollider windowDetectionCollider = null;
    [SerializeField] Renderer windowRend = null;
    Coroutine closeWindowCoroutine = null;
    #region WindowType
    [Header("Tentacle Slap")]
    [SerializeField, Range(0, 10)] float slapRange = 2;
    [SerializeField, Range(0, 10)] float slapStunTime = .5f;
    [Space, Header("Presents")]
    [SerializeField, Range(0, 10)] float presentsActivationTime = 3;
    [Space, Header("TP")]
    [SerializeField, Range(0, 10)] float tpRange = 2;
    [SerializeField] CDL_Window linkedWindow = null;
    [Space, Header("Boost")]
    [SerializeField, Range(0, 10)] float boostRange = 2;
    [SerializeField, Range(0, 10)] float boostDuration = 2;
    [Space, Header("Void")]
    [SerializeField, Range(0, 10)] float voidRange = 2;
    [SerializeField, Range(0, 10)] float voidStunTime = 1;
    [Space, Header("Delivery")]
    [SerializeField, Range(0, 10)] float deliveryRange = 2;
    #endregion

    public bool IsReady => windowDetectionCollider;

    #region DEBUG
    [SerializeField] ParticleSystem presentFart = null;

    private void Start()
    {
        //ChangeWindow(CurrentWindowType, 10000, linkedWindow);
    }

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (!IsReady) return;
        CDL_Player _pl = other.GetComponent<CDL_Player>();
        if (_pl)
        {
            switch (CurrentWindowType)
            {
                case WindowType.TentacleSlap:
                    TentacleSlapBehaviour(_pl);
                    break;
                case WindowType.Void:
                    VoidBehaviour(_pl);
                    break;
                case WindowType.TP:
                    TPBehaviour(_pl);
                    break;
                case WindowType.Boost:
                    BoostBehaviour(_pl);
                    break;
                //case WindowType.Delivery:
                //    DeliveryBehaviour(_pl);
                    //break;
                default:
                    break;
            }
        }
    }
    

    private void OnDrawGizmos()
    {
        switch (CurrentWindowType)
        {
            //case WindowType.Delivery:
            //    Gizmos.color = Color.blue;
            //    Gizmos.DrawSphere(transform.position, .25f);
            //    break;
            case WindowType.None:
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(transform.position, .25f);
                break;
            case WindowType.TentacleSlap:
                Gizmos.color = Color.red - new Color(0, 0, 0, .60f);
                Gizmos.DrawSphere(transform.position, slapRange);
                break;
            case WindowType.Void:
                Gizmos.color = Color.black - new Color(0, 0, 0, .60f);
                Gizmos.DrawSphere(transform.position, voidRange);
                break;
            case WindowType.Presents:
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(transform.position, .25f);
                break;
            case WindowType.TP:
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(transform.position, .25f);
                if(linkedWindow) Gizmos.DrawLine(transform.position, linkedWindow.transform.position);
                break;
            case WindowType.Boost:
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(transform.position, .25f);
                break;
            default:
                break;
        }
        Gizmos.color = Color.white;
    }

    public void ChangeWindow(WindowType _type, float _time, CDL_Window _linkedWindow = null)
    {
        if (!IsReady) return;
        CurrentWindowType = _type;
        switch (CurrentWindowType)
        {
            case WindowType.None:
                windowRend.material.color = Color.gray;
                return;
            case WindowType.TentacleSlap:
                windowDetectionCollider.radius = slapRange / 2f;
                windowRend.material.color = Color.red;
                break;
            case WindowType.Void:
                windowDetectionCollider.radius = voidRange / 2f;
                windowRend.material.color = Color.black;
                break;
            case WindowType.TP:
                if (!_linkedWindow) return;
                windowDetectionCollider.radius = tpRange / 2f;
                windowRend.material.color = Color.magenta;
                linkedWindow = _linkedWindow;
                break;
            case WindowType.Boost:
                windowDetectionCollider.radius = boostRange / 2f;
                windowRend.material.color = Color.yellow;
                break;
            //case WindowType.Delivery:
            //    windowDetectionCollider.radius = deliveryRange / 2f;
            //    windowRend.material.color = Color.blue;
            //    return;
            case WindowType.Presents:
                windowRend.material.color = Color.green;
                PresentsBehaviour();
                return;
        }
        closeWindowCoroutine = StartCoroutine(DelayCloseWindow(_time));
    }

    IEnumerator DelayCloseWindow(float _t)
    {
        yield return new WaitForSeconds(_t);
        CloseWindow();
    }

    public void CloseWindow()
    {
        if (closeWindowCoroutine != null)
        {
            StopCoroutine(closeWindowCoroutine);
            closeWindowCoroutine = null;
        }
        CurrentWindowType = WindowType.None;
        windowRend.material.color = Color.gray;
    }

    void TentacleSlapBehaviour(CDL_Player _pl)
    {
        if (!IsReady) return;
        _pl.Stun(slapStunTime);
        CloseWindow();
    }

    void VoidBehaviour(CDL_Player _pl)
    {
        if (!IsReady) return;
        _pl.Stun(voidStunTime);
        CloseWindow();
    }

    public void PresentsBehaviour()
    {
        if (!IsReady) return;
        StartCoroutine(DelayClosePresent());
    }

    IEnumerator DelayClosePresent()
    {
        yield return new WaitForSeconds(2);
        Instantiate(presentFart, transform.position + Vector3.up, transform.rotation);
        yield return new WaitForSeconds(2);
        CloseWindow();
    }

    void TPBehaviour(CDL_Player _pl)
    {
        if (!IsReady) return;
        _pl.transform.position = new Vector3(linkedWindow.transform.position.x, _pl.transform.position.y, linkedWindow.transform.position.z);
        linkedWindow.CloseWindow();
        CloseWindow();
    }

    void BoostBehaviour(CDL_Player _pl)
    {
        if (!IsReady) return;

    }

    void DeliveryBehaviour(CDL_Player _pl)
    {
        if (!IsReady) return;

    }



}

public enum WindowType
{
    //Delivery,
    None,
    TentacleSlap,
    Void,
    Presents,
    TP,
    Boost
}
