using System.Collections;
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
    [SerializeField] private Transform m_IKTransform = null;
    [SerializeField] private Animator m_animator = null; 

    [SerializeField] private float m_eatingRange = 1f;
    [SerializeField] private float m_detectionRange = 2f;

    [SerializeField] private int m_fieldOfView = 60;

    [SerializeField] private float m_speed = 2f; 

    private FlowerState m_currentState = FlowerState.Searching; 
    [SerializeField] private PAF_Fruit m_followedFruit = null;
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
                m_currentState = FlowerState.Searching;
                m_animator.SetInteger("BehaviourState", (int)m_currentState);
                yield break;
            }
            _targetedPosition = m_followedFruit.transform.position; 
            m_IKTransform.position = Vector3.MoveTowards(m_IKTransform.position, _targetedPosition, Time.deltaTime * m_speed); 
            yield return null; 
        }


    }

    public IEnumerator GetClosestFruit()
    {
        while (m_followedFruit == null)
        {
            yield return null;
            Debug.Log("Search");
            PAF_Fruit[] _fruits = PAF_Fruit.fruits.ToList().Where(f => Vector3.Distance(transform.position, f.transform.position) <= m_detectionRange
                                              && Vector3.Angle(transform.forward, m_followedFruit.transform.position - transform.position) > (m_fieldOfView / 2)).ToArray();
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

    public void EatFruit()
    {
        if (!m_followedFruit) return;
        // EAT THE FRUIT
        Destroy(m_followedFruit.gameObject);
        m_currentState = FlowerState.Searching;
        m_animator.SetInteger("BehaviourState", (int)m_currentState); 
    }
    #endregion

    #region Void

    #endregion

    #endregion

    #region Unity Methods
    private void Start()
    {
    }
    #endregion

    #endregion
}
