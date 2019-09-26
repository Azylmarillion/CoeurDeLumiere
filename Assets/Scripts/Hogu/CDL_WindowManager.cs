using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class CDL_WindowManager : MonoBehaviour
{
    public static CDL_WindowManager I { get; private set; }

    [SerializeField] List<CDL_Window> allWindows = new List<CDL_Window>();

    [SerializeField] bool deliveryOpen = false;
    [SerializeField] bool tpAlreadyUp = false;

    [SerializeField, Range(0, 10)] float globalStunTime = 2;

    [SerializeField, Range(0, 10)] float minChangeWindowTime = 1; 
    [SerializeField, Range(0, 10)] float maxChangeWindowTime = 5;


    #region DeliveryWindow
    [SerializeField] float deliveryWindowOpenTime = 5;
    [SerializeField] CDL_Window deliveryWindowDesignated = null;
    [SerializeField] Coroutine deliveryLooseCoroutine = null;
    #endregion

    #region DEBUG
    [SerializeField] bool hasObjectKey = false;
    #endregion


    private void Awake()
    {
        I = this;
    }

    private void Start()
    {
        for (int i = 0; i < allWindows.Count; i++)
        {
            StartWindow(allWindows[i]);
        }
    }

    private void Update()
    {
        //      DEBUG
        if (Input.GetKeyDown(KeyCode.Space)) SwitchDelivery(hasObjectKey);
    }

    public void SwitchDelivery(bool _state)
    {
        // DEBUG
        hasObjectKey = !hasObjectKey;
        _state = hasObjectKey;
        //

        if(_state)
        {
            if (deliveryLooseCoroutine != null)
            {
                StopCoroutine(deliveryLooseCoroutine);
                CancelInvoke("Flash");
                deliveryLooseCoroutine = null;
                deliveryWindowDesignated.GetComponent<Renderer>().material.color = Color.blue;
            }
            if (deliveryWindowDesignated) return;
            List<CDL_Window> _freeWindows = allWindows.Where(w => w.CurrentWindowType == WindowType.None).ToList();
            int _rnd = Random.Range(0, _freeWindows.Count);
            deliveryWindowDesignated = allWindows[_rnd];
            if (deliveryWindowDesignated) deliveryWindowDesignated.ChangeWindow(WindowType.Delivery);
        }
        else
        {
            deliveryLooseCoroutine = StartCoroutine(FlashDeliveryWindow());
            InvokeRepeating("Flash", 0, .1f);
        }
    }

    void Flash()
    {
        if (!deliveryWindowDesignated) return;
        deliveryWindowDesignated.GetComponent<Renderer>().material.color = deliveryWindowDesignated.GetComponent<Renderer>().material.color == Color.blue ? Color.red : Color.blue;
    }
    
    IEnumerator FlashDeliveryWindow()
    {
        yield return new WaitForSeconds(deliveryWindowOpenTime);
        if (deliveryWindowDesignated)
        {
            deliveryWindowDesignated.ChangeWindow(WindowType.None);
            deliveryWindowDesignated = null;
        }
        if (deliveryLooseCoroutine != null) deliveryLooseCoroutine = null;
        CancelInvoke("Flash");
    }

    public void StartWindow(CDL_Window _window)
    {
        float _t = Random.Range(minChangeWindowTime, maxChangeWindowTime);
        WindowType _type = _window.CurrentWindowType;
        if (_type == WindowType.None) _type = (WindowType)Random.Range(0, Enum.GetNames(typeof(WindowType)).Length);
        if (_type == WindowType.Delivery)
        {
            if (hasObjectKey)
            {
                if (deliveryOpen) _type = WindowType.None;
                deliveryOpen = true;
            }
            else _type = WindowType.None;
        }
        if (_window) _window.ChangeWindow(_type);
        if (_type == WindowType.Presents) _window.PresentsBehaviour();
        if (_type != WindowType.Delivery && _type != WindowType.Presents) StartCoroutine(ChangeWindow(_t, _window));
    }


    IEnumerator ChangeWindow(float _t, CDL_Window _window)
    {
        yield return new WaitForSeconds(_t);
        //if (_window.CurrentWindowType == WindowType.TP) tpAlreadyUp = false;
        _window.ChangeWindow(WindowType.None);
        yield return new WaitForSeconds(_t);
        StartWindow(_window);
    }


}
