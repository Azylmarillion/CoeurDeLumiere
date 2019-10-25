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

    [SerializeField, Range(1, 360)]
    private int m_fieldOfView = 60;

    [SerializeField] private PAF_FlowerJoint[] m_joints = new PAF_FlowerJoint[] { }; 

    private FlowerState m_currentState = FlowerState.Searching; 
    private PAF_Fruit m_followedFruit = null;

    [SerializeField] AudioSource audiosource = null;
    [SerializeField] PAF_SoundData soundData = null;
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
                m_followedFruit = null;
                m_currentState = FlowerState.Searching;
                m_animator.SetInteger("BehaviourState", (int)m_currentState); 
                yield break; 
            }

            if(Vector3.Distance(transform.position, m_followedFruit.transform.position) <= m_eatingRange)
            {
                m_currentState = FlowerState.Eating;
                m_animator.SetInteger("BehaviourState", (int)m_currentState);
                yield break;
            }

            if (PAF_Fruit.ArenaFruits.Length > 0)
            {
                PAF_Fruit[] _fruits = PAF_Fruit.ArenaFruits.ToList().Where(f => Vector3.Distance(transform.position, f.transform.position) <= m_detectionRange
                                                             && Vector3.Angle(transform.forward, f.transform.position - transform.position) < (m_fieldOfView / 2)).ToArray();

                if (_fruits.Length > 0)
                    m_followedFruit = _fruits.OrderBy(f => Vector3.Distance(transform.position, f.transform.position)).FirstOrDefault();
            }

            _targetedPosition = transform.position + (m_followedFruit.transform.position - transform.position).normalized * m_eatingRange;
            float[] _angles = m_joints.ToList().Select(j => j.BaseTransform.localRotation.eulerAngles.y).ToArray();
            PAF_ProceduralAnimationHelper.InverseKinematics(_targetedPosition, m_joints, _angles, .1f);
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
            if (PAF_Fruit.ArenaFruits.Length == 0) continue;
            PAF_Fruit[] _fruits = PAF_Fruit.ArenaFruits.ToList().Where(f => Vector3.Distance(transform.position, f.transform.position) <= m_detectionRange
                                                                         && Vector3.Angle(transform.forward, f.transform.position - transform.position) < (m_fieldOfView / 2)).ToArray();
            if (_fruits.Length == 0) continue;
            m_followedFruit = _fruits.OrderBy(f => Vector3.Distance(transform.position, f.transform.position)).FirstOrDefault(); 
            if(Vector3.Distance(transform.position, m_followedFruit.transform.position) <= m_eatingRange)
            {
                m_currentState = FlowerState.Eating;
                m_animator.SetInteger("BehaviourState", (int)m_currentState);
                yield break; 
            }
        }
        m_currentState = FlowerState.Following;
        m_animator.SetInteger("BehaviourState", (int)m_currentState);
    }
    #endregion

    #region Void
    public void EatFruit()
    {
        if (m_followedFruit)
        {
            m_currentState = FlowerState.Eating;
            m_animator.SetInteger("BehaviourState", (int)m_currentState);

            m_followedFruit.Velocity = Vector3.zero;
            m_followedFruit.Velocity = (transform.position - m_followedFruit.transform.position).normalized * .1f;
        }
        //RESET THE STATE
        m_currentState = FlowerState.Searching;
        m_animator.SetInteger("BehaviourState", (int)m_currentState);
    }

    public void Chomp()
    {
        if(m_followedFruit)
        {
            // EAT THE FRUIT
            m_followedFruit.Eat();
            if (soundData && audiosource)
            {
                AudioClip _clip = soundData.GetPlantEating();
                if (_clip) audiosource.PlayOneShot(_clip);
            }
            // CALL VFX AND SOUND HERE
        }
    }
    #endregion

    #endregion

    #region Unity Methods
    private void Awake()
    {
        m_joints.ToList().ForEach(j => j.Init()); 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, .5f); 

        Gizmos.color = Color.red;
        for (int i = 0; i < m_joints.Length - 1; i++)
        {
            Gizmos.DrawSphere(m_joints[i].BaseTransform.position, .25f); 
            Gizmos.DrawLine(m_joints[i].BaseTransform.position, m_joints[i + 1].BaseTransform.position); 
        }
    }
    #endregion

    #endregion
}
