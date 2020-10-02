using UnityEngine;

public class PAF_PlayerAnimation : MonoBehaviour
{
    [SerializeField] private new AudioSource audio = null;
    [SerializeField] private GameObject m_vfxObject = null;
    [SerializeField] private PAF_Player m_player = null;

    public void StepSounds()
    {
        // Run FX
        GameObject _system = PAF_GameManager.Instance.VFXDatas.StepSmokeFX.gameObject;
        Transform _fx = Instantiate(_system, transform.position, Quaternion.identity).transform;

        if (transform.rotation.eulerAngles.y > 180)
            _fx.localScale = new Vector3(_fx.localScale.x * -1, _fx.localScale.y, _fx.localScale.z);

        audio.PlayOneShot(PAF_GameManager.Instance.SoundDatas.GetStepsPlayer(), .25f);
    }

    public void CastTrail() => m_vfxObject.SetActive(!m_vfxObject.activeInHierarchy);

    public void Interact() => m_player.Interact();
}
