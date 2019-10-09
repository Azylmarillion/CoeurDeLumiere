using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PAF_Fruit : MonoBehaviour
{
    #region Events
    private event Action OnOverlap = null;
    #endregion

    #region Fields / Properties

    #region Static & Constants
    /// <summary>
    /// All fruits 
    /// </summary>
    public static PAF_Fruit[] fruits = new PAF_Fruit[] { };
    #endregion

    #region Parameters
    /// <summary>
    /// Collider of the object.
    /// </summary>
    [SerializeField] private new Collider collider = null;

    /// <summary>
    /// Weight of the fruit, influencing its movement force.
    /// </summary>
    [SerializeField, Min(.01f)] private float weight = 1;

    /// <summary>
    /// Get the fruit weight.
    /// </summary>
    public float Weight { get { return weight; } }

    /// <summary>Backing field for <see cref="Velocity"/>.</summary>
    [SerializeField] private Vector3 velocity = new Vector3();

    /// <summary>
    /// Velocity vector of the fruit movement.
    /// </summary>
    public Vector3 Velocity
    {
        get { return velocity; }
        set
        {
            velocity = value;
            if ((velocity != Vector3.zero) && (applyForceCoroutine != null))
            {
                applyForceCoroutine = StartCoroutine(ApplyForce());
            }
        }
    }
    #endregion

    #region Coroutines
    /// <summary>
    /// Current reference of <see cref="ApplyForce(Vector3)"/> coroutine.
    /// </summary>
    private Coroutine applyForceCoroutine = null;
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Adds a force to the fruit, making it go in this direction.
    /// </summary>
    /// <param name="_force">Force to apply.</param>
    public void AddForce(Vector3 _force)
    {
        velocity += _force;
        applyForceCoroutine = StartCoroutine(ApplyForce());
    }

    /// <summary>
    /// Apply a force to the fruit.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ApplyForce()
    {
        Vector3 _newPos;

        while (velocity != Vector3.zero)
        {
            _newPos = transform.position + velocity;



            yield return new WaitForFixedUpdate();
        }

        applyForceCoroutine = null;
    }

    /// <summary>
    /// Method called when starting to collide with another collider.
    /// </summary>
    /// <param name="_other"></param>
    private void OnColliderEnter(Collider _collider)
    {

    }
    #endregion

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        if (!collider)
        {
            collider = GetComponent<Collider>();
            if (!collider) Debug.LogError($"The Fruit \"{name}\" don't have collider attached to it !");
        }

    }
    #endregion

    #endregion
}
