using System.Collections.Generic;
using UnityEngine;

public class PAF_Flower : MonoBehaviour 
{
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
    private float[] jointsAngle = new float[] { };

    [SerializeField] private Transform m_mouthTransform = null; 
    public Transform MouthTransform { get { return m_mouthTransform; } }

    private FlowerState m_currentState = FlowerState.Searching; 
    [SerializeField] private PAF_Fruit m_followedFruit = null;

    private bool doFollowFruit = false;

    [SerializeField] AudioSource audiosource = null;

    [SerializeField]  private bool isLeft = true;
    public static PAF_Flower[] Flowers = new PAF_Flower[2];

    private readonly int behaviour_Hash = Animator.StringToHash("BehaviourState");
    private readonly int reset_Hash = Animator.StringToHash("ResetSearch");
    #endregion

    #region Methods

    #region Original Methods
    private bool doPlaySound = false;
    private float playSoundVar = 0;

    private bool doSearchFruit = false;

    private void Update()
    {
        // Sound delay.
        if (doPlaySound)
        {
            playSoundVar -= Time.deltaTime;
            if (playSoundVar <= 0)
            {
                doPlaySound = false;
                audiosource.PlayOneShot(PAF_GameManager.Instance.SoundDatas.GetflowerGloups(), .8f);
            }
        }

        // Try get fruit.
        if (doSearchFruit)
        {
            // Player hit.
            bool _doHit = false;
            for (int _i = 0; _i < 2; _i++)
            {
                if (Vector3.Distance(m_mouthTransform.position, PAF_Player.Players[_i].transform.position) <= m_eatingPlayerRange)
                {
                    _doHit = true;
                    PAF_Player.Players[_i].Recoil(transform.position);
                }
            }
            if (_doHit)
            {
                doSearchFruit = false;

                doFollowFruit = false;
                m_followedFruit = null;
                m_currentState = FlowerState.Eating;
                m_animator.SetInteger(behaviour_Hash, (int)m_currentState);
            }
            // Try to eat fruits.
            else if (PAF_Fruit.ArenaFruits.Count > 0)
            {
                List<PAF_Fruit> _fruits = PAF_Fruit.ArenaFruits;

                int _index = 0;
                float _distance = Vector3.Distance(transform.position, _fruits[0].transform.position);

                for (int _i = 1; _i < _fruits.Count; _i++)
                {
                    float _otherDistance = Vector3.Distance(transform.position, _fruits[_i].transform.position);
                    if (_otherDistance < _distance)
                    {
                        _distance = _otherDistance;
                        _index = _i;
                    }
                }

                if ((_distance <= m_detectionRange) &&
                    (Vector3.Angle(transform.forward, _fruits[_index].transform.position - transform.position) < (m_fieldOfView / 2)))
                {
                    doFollowFruit = true;
                    m_followedFruit = _fruits[_index];

                    if (Vector3.Distance(transform.position, m_followedFruit.transform.position) <= m_eatingRange)
                    {
                        doSearchFruit = false;
                        EatFruit();
                    }
                    else
                    {
                        m_currentState = FlowerState.Following;
                        m_animator.SetInteger(behaviour_Hash, (int)m_currentState);
                    }
                }
            }
        }

        // Follow fruit.
        if (doFollowFruit)
        {
            if (Vector3.Angle(transform.forward, m_followedFruit.transform.position - transform.position) > (m_fieldOfView / 2))
            {
                doFollowFruit = false;
                m_followedFruit = null;

                m_currentState = FlowerState.Searching;
                m_animator.SetInteger(behaviour_Hash, (int)m_currentState);
            }
            else if (Vector3.Distance(transform.position, m_followedFruit.transform.position) <= m_eatingRange)
            {
                m_currentState = FlowerState.Searching;
                m_animator.SetInteger(behaviour_Hash, (int)m_currentState);

                EatFruit();
            }
            else if (PAF_Fruit.ArenaFruits.Count > 0)
            {
                List<PAF_Fruit> _fruits = PAF_Fruit.ArenaFruits;

                int _index = 0;
                float _distance = Vector3.Distance(transform.position, _fruits[0].transform.position);

                for (int _i = 1; _i < _fruits.Count; _i++)
                {
                    float _otherDistance = Vector3.Distance(transform.position, _fruits[_i].transform.position);
                    if (_otherDistance < _distance)
                    {
                        _distance = _otherDistance;
                        _index = _i;
                    }
                }

                if ((_distance <= m_detectionRange) &&
                    (Vector3.Angle(transform.forward, _fruits[_index].transform.position - transform.position) < (m_fieldOfView / 2)))
                {
                    m_followedFruit = _fruits[_index];

                    Vector3 _targetedPosition = transform.position + (m_followedFruit.transform.position - transform.position).normalized * m_eatingRange;

                    for (int _i = 0; _i < m_joints.Length; _i++)
                        jointsAngle[_i] = m_joints[_i].BaseTransform.localRotation.eulerAngles.y;

                    PAF_ProceduralAnimationHelper.InverseKinematics(_targetedPosition, m_joints, jointsAngle, .1f);
                }
            }
            else
            {
                doFollowFruit = false;
                m_followedFruit = null;

                m_currentState = FlowerState.Searching;
                m_animator.SetInteger(behaviour_Hash, (int)m_currentState);
            }

            // Player hit.
            bool _doHit = false;
            for (int _i = 0; _i < 2; _i++)
            {
                if (Vector3.Distance(m_mouthTransform.position, PAF_Player.Players[_i].transform.position) <= m_eatingPlayerRange)
                {
                    _doHit = true;
                    PAF_Player.Players[_i].Recoil(transform.position);
                }
            }
            if (_doHit)
            {
                doFollowFruit = false;
                m_followedFruit = null;

                m_currentState = FlowerState.Eating;
                m_animator.SetInteger(behaviour_Hash, (int)m_currentState);
            }
        }
    }
    
    public void DoFollowFruit(bool _doFollow) => doFollowFruit = _doFollow;

    public void DoSearchFruit(bool _doSearch) => doSearchFruit = _doSearch;

    /// <summary>
    /// Set to the eating state
    /// </summary>
    public void EatFruit()
    {
        if (doFollowFruit)
        {
            m_followedFruit.StartToEat(m_mouthTransform);

            m_currentState = FlowerState.Eating;
            m_animator.SetInteger(behaviour_Hash, (int)m_currentState);

            doPlaySound = true;
            playSoundVar = .5f;
        }
    }

    /// <summary>
    /// Eat the fruit and reset the state
    /// </summary>
    public void Chomp()
    {
        if (m_followedFruit)
        {
            // EAT THE FRUIT
            m_followedFruit.Eat();
            audiosource.PlayOneShot(PAF_GameManager.Instance.SoundDatas.GetPlantEating());

            doFollowFruit = false;
            m_followedFruit = null;
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
        m_animator.SetTrigger(reset_Hash);
        m_animator.SetInteger(behaviour_Hash, (int)m_currentState);
    }

    public static void RemoveFruit(PAF_Fruit _fruit)
    {
        if (!areFlowers)
            return;

        for (int _i = 0; _i < 2; _i++)
        {
            if (Flowers[_i].m_followedFruit && Flowers[_i].m_followedFruit.GetInstanceID() == _fruit.GetInstanceID())
            {
                if (Flowers[_i].doFollowFruit)
                {
                    Flowers[_i].doFollowFruit = false;
                    Flowers[_i].m_followedFruit = null;

                    Flowers[_i].m_currentState = FlowerState.Searching;
                    Flowers[_i].m_animator.SetInteger(Flowers[_i].behaviour_Hash, (int)Flowers[_i].m_currentState);
                }
            }
        }
    }
    #endregion

    #region Unity Methods
    private static bool areFlowers = false;

    private void Awake()
    {
        areFlowers = true;
        Flowers[isLeft ? 0 : 1] = this;

        for (int _i = 0; _i < m_joints.Length; _i++)
            m_joints[_i].Init();

        jointsAngle = new float[m_joints.Length];
    }

    private void OnDestroy()
    {
        areFlowers = false;
        Flowers[isLeft ? 0 : 1] = null;
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
