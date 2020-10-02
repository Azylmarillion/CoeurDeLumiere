using System;
using UnityEngine;
using UnityEngine.Events; 

[Serializable]
public class PAF_Event 
{
	#region Events
    [SerializeField] private UnityEvent m_event = null;
    #endregion

    #region Fields / Properties
    [SerializeField, Range(0.0f, 120.0f) ] private int m_callingTime = 0;  
    public int CallingTime { get { return m_callingTime; } }
    public bool HasBeenCalled { get; private set; } = false; 
	#endregion

	#region Methods

	#region Original Methods
    public void CallEvent()
    {
        HasBeenCalled = true;
        m_event.Invoke(); 
    }
	#endregion

	#endregion
}
