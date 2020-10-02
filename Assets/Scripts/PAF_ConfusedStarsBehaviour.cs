using UnityEngine;

public class PAF_ConfusedStarsBehaviour : MonoBehaviour
{
    private void OnDestroy() => Destroy(transform.parent.gameObject);
}
