using UnityEngine;

public class PAF_Dalle : MonoBehaviour
{
    [SerializeField] bool isShifting = true;
    [SerializeField] bool randomColor = true;

    public bool IsShifting => isShifting;
    public bool RandomColor => randomColor;

    public void Fall() => gameObject.SetActive(false);
}
