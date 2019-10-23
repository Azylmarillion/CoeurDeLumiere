using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FruitData", menuName = "ScriptableObjects/FruitDataScriptableObject", order = 1)]
public class FruitData : ScriptableObject
{
    [SerializeField] GameObject[] fruits;


    public GameObject[] GetRandomFruit(int _number)
    {
        List<GameObject> _fruits = new List<GameObject>();
        for (int i = 0; i < _number; i++)
        {
            _fruits.Add(fruits[Random.Range(0, fruits.Length)]);
        }
        return _fruits.ToArray();
    }
}
