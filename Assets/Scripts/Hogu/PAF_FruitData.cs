using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FruitData", menuName = "ScriptableObjects/FruitDataScriptableObject", order = 1)]
public class PAF_FruitData : ScriptableObject
{
    [SerializeField] GameObject[] fruits;
    [SerializeField] GameObject[] goldenFruits;
    public GameObject GetGoldenFruit
    {
        get
        {
            if ((goldenFruits == null) || (goldenFruits.Length == 0)) return null;
            return goldenFruits[Random.Range(0, goldenFruits.Length)];
        }
    }

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
