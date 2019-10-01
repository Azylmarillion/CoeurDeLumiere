using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDL_GameManager : MonoBehaviour
{
    [SerializeField] CDL_Player playerOne = null;
    [SerializeField] CDL_Player playerTwo = null;

    [SerializeField] float timer = 120;




    private void Update()
    {
        Timer();
    }




    void Timer()
    {
        timer -= Time.deltaTime;
    }
}
