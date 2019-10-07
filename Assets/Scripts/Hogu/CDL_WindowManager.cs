using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CDL_WindowManager : MonoBehaviour
{
    public static CDL_WindowManager I { get; private set; }
    


    #region DEBUG
    [SerializeField] bool hasObjectKey = false;
    #endregion


    private void Awake()
    {
        I = this;
    }

    void SetWindow(CDL_Window _window, bool _opened)
    {

    }

}
