using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CDL_WindowManager : MonoBehaviour
{
    public static CDL_WindowManager I { get; private set; }

    [SerializeField] List<CDL_Window> allWindows = new List<CDL_Window>();

    [SerializeField] bool deliveryOpen = false;
    [SerializeField] bool tpAlreadyUp = false;

    [SerializeField, Range(0, 10)] float globalStunTime = 2;

    [SerializeField, Range(0, 10)] float minChangeWindowTime = 1; 
    [SerializeField, Range(0, 10)] float maxChangeWindowTime = 5;

    [SerializeField] int maxWindowsOpened = 2;
    int currentOpenedWindows { get { return allWindows.Where(w => w.CurrentWindowType != WindowType.None /*&& w.CurrentWindowType != WindowType.Delivery*/).ToList().Count; } }


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
        //for (int i = 0; i < allWindows.Count; i++)
        //{
        //    StartWindow(allWindows[i]);
        //}
    }

    private void Update()
    {
        //      DEBUG
        //if (Input.GetKeyDown(KeyCode.Space)) SwitchDelivery(hasObjectKey);
        //

        if (currentOpenedWindows < maxWindowsOpened) ChooseWindow();
    }

    //public void SwitchDelivery(bool _state)
    //{
    //    // DEBUG
    //    hasObjectKey = !hasObjectKey;
    //    _state = hasObjectKey;
    //    //

    //    if(_state)
    //    {
    //        if (deliveryLooseCoroutine != null)
    //        {
    //            StopCoroutine(deliveryLooseCoroutine);
    //            CancelInvoke("Flash");
    //            deliveryLooseCoroutine = null;
    //            deliveryWindowDesignated.GetComponent<Renderer>().material.color = Color.blue;
    //        }
    //        if (deliveryWindowDesignated) return;
    //        List<CDL_Window> _freeWindows = allWindows.Where(w => w.CurrentWindowType == WindowType.None).ToList();
    //        int _rnd = Random.Range(0, _freeWindows.Count);
    //        deliveryWindowDesignated = _freeWindows[_rnd];
    //        if (deliveryWindowDesignated) deliveryWindowDesignated.ChangeWindow(WindowType.Delivery);
    //    }
    //    else
    //    {
    //        deliveryLooseCoroutine = StartCoroutine(FlashDeliveryWindow());
    //        InvokeRepeating("Flash", 0, .1f);
    //    }
    //}

    //void Flash()
    //{
    //    if (!deliveryWindowDesignated) return;
    //    deliveryWindowDesignated.GetComponent<Renderer>().material.color = deliveryWindowDesignated.GetComponent<Renderer>().material.color == Color.blue ? Color.red : Color.blue;
    //}
    
    //IEnumerator FlashDeliveryWindow()
    //{
    //    yield return new WaitForSeconds(deliveryWindowOpenTime);
    //    if (deliveryWindowDesignated)
    //    {
    //        deliveryWindowDesignated.CloseWindow();
    //        deliveryWindowDesignated = null;
    //    }
    //    if (deliveryLooseCoroutine != null) deliveryLooseCoroutine = null;
    //    CancelInvoke("Flash");
    //}


    void SetWindow(CDL_Window _window)
    {
        float _time = Random.Range(minChangeWindowTime, maxChangeWindowTime);
        WindowType _type = (WindowType)Random.Range(0, System.Enum.GetNames(typeof(WindowType)).Length);
        CDL_Window _tpWindow = null;
        if (_type == WindowType.TP)
        {
            List<CDL_Window> _freeWindows = allWindows.Where(w => w.CurrentWindowType == WindowType.None && w != _window).ToList();
            int _rnd = Random.Range(0, _freeWindows.Count);
            _tpWindow = _freeWindows[_rnd];
            if (_tpWindow)
            {
                _tpWindow.ChangeWindow(_type, _time, _window);
                StartCoroutine(DelayChooseWindow(_time, _tpWindow));
            }
        }
        _window.ChangeWindow(_type, _time, _tpWindow);
        if(_type != WindowType.Presents) StartCoroutine(DelayChooseWindow(_time, _window));
    }

    void ChooseWindow()
    {
        List<CDL_Window> _freeWindows = allWindows.Where(w => w.CurrentWindowType == WindowType.None).ToList();
        int _rnd = Random.Range(0, _freeWindows.Count);
        if (_freeWindows.Count > _rnd) SetWindow(_freeWindows[_rnd]);
    }

    IEnumerator DelayChooseWindow(float _t, CDL_Window _window)
    {
        yield return new WaitForSeconds(_t);
        _window.CloseWindow();
    }

}
