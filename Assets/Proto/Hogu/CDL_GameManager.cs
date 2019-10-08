using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDL_GameManager : MonoBehaviour
{
    public static CDL_GameManager I { get; private set; }

    [SerializeField] CDL_Player playerOne = null;
    [SerializeField] CDL_Player playerTwo = null;

    [SerializeField] float timer = 120;


    private void Awake()
    {
        I = this;
    }

    private void Update()
    {
        Timer();
    }


    public void Win(CDL_Player _pl)
    {

    }


    void Timer()
    {
        timer -= Time.deltaTime;
    }
}
