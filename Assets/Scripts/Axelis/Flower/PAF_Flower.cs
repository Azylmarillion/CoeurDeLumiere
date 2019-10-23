﻿using System.Collections;
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

    [SerializeField, Range(.1f, 5.0f)]
    private float m_eatingRange = 1.0f;
    [SerializeField, Range(5.0f, 50.0f)]
    private float m_detectionRange = 2.0f;

    [SerializeField, Range(1, 360)]
    private int m_fieldOfView = 60;

    [SerializeField, Range(.1f, 5.0f)]
    private float m_speed = 2.0f; 

    private FlowerState m_currentState = FlowerState.Searching; 
    private PAF_Fruit m_followedFruit = null;

    [SerializeField] AudioSource audiosource = null;
    [SerializeField] SoundData soundData = null;
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
            _targetedPosition = transform.position + (m_followedFruit.transform.position - transform.position).normalized * m_eatingRange; 
            m_IKTransform.position = Vector3.MoveTowards(m_IKTransform.position, _targetedPosition, Time.deltaTime * m_speed); 
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
            // EAT THE FRUIT
            m_followedFruit.Eat(); 
            if(soundData && audiosource)
            {
                AudioClip _clip = soundData.GetPlantEating();
                if (_clip) audiosource.PlayOneShot(_clip);
            }
            // CALL VFX AND SOUND HERE

        }
        //RESET THE STATE
        m_currentState = FlowerState.Searching;
        m_animator.SetInteger("BehaviourState", (int)m_currentState);
    }
    #endregion

    #endregion

    #region Unity Methods
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(m_IKTransform.position, .25f); 
    }
    #endregion

    #endregion
}
