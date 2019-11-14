using System.Collections;
using System.Collections.Generic; 
using System.Linq; 
using UnityEngine;

public class PAF_Flower : MonoBehaviour 
{
    /* PAF_Flower :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[PURPOSE]
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	[TO DO]
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[DATE]
	 *	Author :		[NAME]
	 *
	 *	Changes :
	 *
	 *	[CHANGES]
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties
    [SerializeField] private Animator m_animator = null; 

    [SerializeField, Range(.1f, 10.0f)]
    private float m_eatingRange = 1.0f;
    [SerializeField, Range(5.0f, 50.0f)]
    private float m_detectionRange = 2.0f;
    [SerializeField, Range(.1f, 5.0f)]
    private float m_eatingPlayerRange = 1.0f;

    [SerializeField, Range(1, 360)]
    private int m_fieldOfView = 60;

    [SerializeField] private PAF_FlowerJoint[] m_joints = new PAF_FlowerJoint[] { };

    [SerializeField] private Transform m_mouthTransform = null; 
    public Transform MouthTransform { get { return m_mouthTransform; } }

    private FlowerState m_currentState = FlowerState.Searching; 
    private PAF_Fruit m_followedFruit = null;
    public bool HasFruitToFollow { get { return m_followedFruit != null;  } }

    [SerializeField] AudioSource audiosource = null;

    public static List<PAF_Flower> Flowers = new List<PAF_Flower>();

    private bool m_hasEat = false;
    #endregion

    #region Methods

    #region Original Methods

    #region IEnumerator
    public IEnumerator FollowTarget()
    {
        Vector3 _targetedPosition = Vector3.zero; 
        while (m_followedFruit != null)
        {
            if(Vector3.Angle(transform.forward, m_followedFruit.transform.position - transform.position) > (m_fieldOfView/2))
            {
                m_currentState = FlowerState.Searching;
                m_animator.SetInteger("BehaviourState", (int)m_currentState);
                m_followedFruit = null;
                yield break; 
            }

            if(Vector3.Distance(transform.position, m_followedFruit.transform.position) <= m_eatingRange)
            {
                EatFruit(); 
                yield break;
            }

            if (PAF_Fruit.ArenaFruits.Length > 0)
            {
                PAF_Fruit[] _fruits = PAF_Fruit.ArenaFruits.ToList().Where(f => Vector3.Distance(transform.position, f.transform.position) <= m_detectionRange
                                                             && Vector3.Angle(transform.forward, f.transform.position - transform.position) < (m_fieldOfView / 2)).ToArray();

                if (_fruits.Length > 0)
                    m_followedFruit = _fruits.OrderBy(f => Vector3.Distance(transform.position, f.transform.position)).FirstOrDefault();
            }

            if(PAF_Player.Players.Any(p => Vector3.Distance(m_mouthTransform.position, p.transform.position) <= m_eatingPlayerRange))
            {
                PAF_Player.Players.Where(p => Vector3.Distance(transform.position, p.transform.position) <= m_eatingRange).ToList().ForEach(p => p.Recoil(transform.position)); 
                m_followedFruit = null;
                m_currentState = FlowerState.Eating;
                m_animator.SetInteger("BehaviourState", (int)m_currentState);
                yield break; 
            }

            _targetedPosition = transform.position + (m_followedFruit.transform.position - transform.position).normalized * m_eatingRange;
            if(m_joints != null && m_joints.Length > 0)
            {
                float[] _angles = m_joints.ToList().Select(j => j.BaseTransform.localRotation.eulerAngles.y).ToArray();
                PAF_ProceduralAnimationHelper.InverseKinematics(_targetedPosition, m_joints, _angles, .1f);
            }
            yield return null; 
        }
        m_currentState = FlowerState.Searching;
        m_animator.SetInteger("BehaviourState", (int)m_currentState);
        yield break;
    }

    public IEnumerator GetClosestFruit()
    {
        while (m_followedFruit == null)
        {
            yield return null;
            if (PAF_Player.Players.Any(p => Vector3.Distance(m_mouthTransform.position, p.transform.position) <= m_eatingPlayerRange))
            {
                PAF_Player.Players.Where(p => Vector3.Distance(transform.position, p.transform.position) <= m_eatingRange).ToList().ForEach(p => p.Recoil(transform.position));
                m_followedFruit = null;
                m_currentState = FlowerState.Eating;
                m_animator.SetInteger("BehaviourState", (int)m_currentState);
                yield break;
            }
            if (PAF_Fruit.ArenaFruits.Length == 0)
            {
                continue;
            }
            PAF_Fruit[] _fruits = PAF_Fruit.ArenaFruits.ToList().Where(f => Vector3.Distance(transform.position, f.transform.position) <= m_detectionRange
                                                                         && Vector3.Angle(transform.forward, f.transform.position - transform.position) < (m_fieldOfView / 2)).ToArray();
            if (_fruits.Length == 0)
            {
                continue;
            }
            m_followedFruit = _fruits.OrderBy(f => Vector3.Distance(transform.position, f.transform.position)).FirstOrDefault(); 
            if(Vector3.Distance(transform.position, m_followedFruit.transform.position) <= m_eatingRange)
            {
                EatFruit();
                yield break; 
            }
        }
        m_currentState = FlowerState.Following;
        m_animator.SetInteger("BehaviourState", (int)m_currentState);
    }

    IEnumerator DelayGloupsSound()
    {
        if (m_hasEat) yield break;
        m_hasEat = true;
        yield return new WaitForSeconds(.5f);
        AudioClip _clip = PAF_GameManager.Instance?.SoundDatas.GetflowerGloups();
        if (_clip) audiosource.PlayOneShot(_clip, .8f);
        m_hasEat = false;
    }
    #endregion

    #region Void
    /// <summary>
    /// Set to the eating state
    /// </summary>
    public void EatFruit()
    {
        if (m_followedFruit != null)
        {
            m_followedFruit.StartToEat(m_mouthTransform);

            m_currentState = FlowerState.Eating;
            m_animator.SetInteger("BehaviourState", (int)m_currentState);

            StartCoroutine(DelayGloupsSound());

            return; 
        }
    }

    /// <summary>
    /// Eat the fruit and reset the state
    /// </summary>
    public void Chomp()
    {
        if(m_followedFruit != null)
        {
            // EAT THE FRUIT
            m_followedFruit.Eat();
            if (audiosource)
            {
                AudioClip _clip = PAF_GameManager.Instance?.SoundDatas.GetPlantEating();
                if (_clip) audiosource.PlayOneShot(_clip);
            }
        }
        //RESET THE STATE
        ResetState(); 
    }

    /// <summary>
    /// Reset the state to Searching
    /// </summary>
    public void ResetState()
    {
        m_currentState = FlowerState.Searching;
        m_animator.SetTrigger("ResetSearch");
        m_animator.SetInteger("BehaviourState", (int)m_currentState);
    }
    #endregion

    #endregion

    #region Unity Methods
    private void Awake()
    {
        m_joints.ToList().ForEach(j => j.Init());
        Flowers.Add(this); 
    }

    private void OnDestroy()
    {
        Flowers.Remove(this); 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, .5f); 

        for (int i = 0; i < m_joints.Length - 1; i++)
        {
            Gizmos.DrawSphere(m_joints[i].BaseTransform.position, .25f); 
            Gizmos.DrawLine(m_joints[i].BaseTransform.position, m_joints[i + 1].BaseTransform.position); 
        }

        if(m_mouthTransform != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(m_mouthTransform.position, .15f);
            Gizmos.color = new Color(0, 0, 1, .5f); 
            Gizmos.DrawSphere(m_mouthTransform.position, m_eatingPlayerRange); 
        }
        
        if(m_followedFruit != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(m_mouthTransform.position, m_followedFruit.transform.position); 
        }
    }
    #endregion

    #endregion
}
