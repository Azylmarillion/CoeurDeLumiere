using UnityEngine;

[CreateAssetMenu(fileName = "FruitData", menuName = "ScriptableObjects/FruitDataScriptableObject", order = 1)]
public class PAF_FruitData : ScriptableObject
{
    [SerializeField] private GameObject[] fruits = null;
    [SerializeField] private GameObject[] goldenFruits = null;

    private static readonly GameObject[] spawnFruits = new GameObject[16];

    public GameObject GetGoldenFruit => goldenFruits[Random.Range(0, goldenFruits.Length)];

    public GameObject[] GetRandomFruit(int _number)
    {
        for (int _i = 0; _i < _number; _i++)
            spawnFruits[_i] = fruits[Random.Range(0, fruits.Length)];

        return spawnFruits;
    }
}
