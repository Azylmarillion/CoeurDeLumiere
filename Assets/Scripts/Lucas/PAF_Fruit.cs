using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PAF_Fruit : MonoBehaviour
{
    #region Events

    #endregion

    #region Fields / Properties

    #region Static & Constants
    /// <summary>
    /// Reference list of all fruits currently on the arena.
    /// </summary>
    private static List<PAF_Fruit> arenaFruits = new List<PAF_Fruit>();

    /// <summary>
    /// All fruits currently on the arena !
    /// </summary>
    public static PAF_Fruit[] ArenaFruits { get { return arenaFruits.ToArray(); } }
    #endregion

    #region Parameters
    /// <summary>
    /// Collider of the object.
    /// </summary>
    [SerializeField] private new SphereCollider collider = null;

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
        Vector3 _normal = new Vector3(velocity.z, velocity.y, -velocity.x);
        Vector3 _raycastPos;
        RaycastHit _hit;

        while (velocity != Vector3.zero)
        {
            // Take closest
            if (RaycastVelocityMovement(_raycastPos = collider.bounds.center, out _hit) ||
                RaycastVelocityMovement(_raycastPos += _normal * collider.radius, out _hit) ||
                RaycastVelocityMovement(_raycastPos -= _normal * collider.radius * 2, out _hit))
            {
                transform.position = transform.position + (velocity * _hit.distance);

                // Set velocity after collision
                velocity = new Vector3(Mathf.Abs(velocity.x) * Mathf.Sign(_hit.normal.x) * .5f, Mathf.Abs(velocity.y) * Mathf.Sign(_hit.normal.y) * .5f, Mathf.Abs(velocity.z) * Mathf.Sign(_hit.normal.z) * .5f);

                // Push the touched fruit if one
                PAF_Fruit _fruit = _hit.collider.GetComponent<PAF_Fruit>();
                
                //if (_fruit) _fruit.AddForce(???);
            }

            yield return new WaitForFixedUpdate();
        }

        applyForceCoroutine = null;
    }

    private bool RaycastVelocityMovement(Vector3 _raycastPos, out RaycastHit _hit)
    {
        return Physics.Raycast(_raycastPos, velocity, out _hit, (collider.bounds.center + velocity).magnitude);
    }
    #endregion

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        #if UNITY_EDITOR
        if (!collider)
        {
            collider = GetComponent<SphereCollider>();
            if (!collider) Debug.LogError($"The Fruit \"{name}\" don't have collider attached to it !");
            else Debug.LogWarning($"The Fruit \"{name}\" don't have collider linked to it !");
        }
        #endif

        // Add this fruit to the arena list on start !
        arenaFruits.Add(this);
    }

    private void OnDestroy()
    {
        // Remove this fruit from the arena list when destroyed
        if (arenaFruits.Contains(this)) arenaFruits.Remove(this);
    }
    #endregion

    #endregion
}
