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

    /// <summary>
    /// Layer mask of what the fruits should collide on.
    /// </summary>
    [SerializeField] private LayerMask whatCollide = new LayerMask();

    /// <summary>
    /// Velocity without y axis.
    /// </summary>
    private Vector3 flatVelocity { get { return new Vector3(velocity.x, 0, velocity.z); } }

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
            if ((value != Vector3.zero) && (applyForceCoroutine == null))
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
    public void AddForce(Vector3 _force) => Velocity += _force;

    /// <summary>
    /// Apply a force to the fruit.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ApplyForce()
    {
        Vector3 _flatVelocity;
        Vector3 _normal;
        Vector3[] _raycastPos = new Vector3[3];
        RaycastHit[] _hits = new RaycastHit[3];
        RaycastHit _finalHit = new RaycastHit();
        int _nearestHitIndex = 0;

        while ((_flatVelocity = flatVelocity) != Vector3.zero)
        {
            // Calculate normal & raycasts position
            _normal = new Vector3(velocity.z, 0, -velocity.x);
            _raycastPos = new Vector3[] { collider.bounds.center + (_flatVelocity * collider.radius), collider.bounds.center + (_normal * collider.radius), collider.bounds.center - (_normal * collider.radius) };

            // Raycast from side extrem points and front center, and get closest touched object if one
            _nearestHitIndex = 0;
            Physics.Raycast(_raycastPos[0], _flatVelocity, out _hits[0], _flatVelocity.magnitude, whatCollide);

            for (int _i = 1; _i < 3; _i++)
            {
                if (Physics.Raycast(_raycastPos[_i], _flatVelocity, out _hits[_i], _flatVelocity.magnitude, whatCollide)
                    && (_hits[_i].distance < _hits[_i - 1].distance))
                {
                    _nearestHitIndex = _i;
                }
            }

            // If hit something, bounce on it and recalculate trajectory
            if (_hits[_nearestHitIndex].collider)
            {
                // Bounce on a Sphere collider
                if (_hits[_nearestHitIndex].collider is SphereCollider _sphere)
                {
                    Vector3 _edge;
                    if (_nearestHitIndex == 0)
                    {
                        _edge = _sphere.bounds.center;
                    }
                    else
                    {
                        _edge = _sphere.bounds.center + (_normal * (_nearestHitIndex == 1 ? -1 : 1) * _sphere.radius);
                    }

                    Vector3 _intersection = _sphere.bounds.center + (Mathf.Cos(Vector3.Angle(_edge, _hits[_nearestHitIndex].point) * Mathf.Deg2Rad) * _normal);
                    Vector3 _rayPos = _edge - ((_edge - _intersection) / 2);

                    if (!Physics.Raycast(_rayPos, -_flatVelocity, out _finalHit, _flatVelocity.magnitude + _sphere.radius + 1, whatCollide) || !Physics.Raycast(_finalHit.point, _flatVelocity, out _finalHit, _flatVelocity.magnitude, whatCollide)) continue;
                }
                // Bounce on a Box Collider
                else
                {
                    if (!Physics.Raycast(new Vector3(collider.bounds.center.x + (_hits[_nearestHitIndex].normal.z * collider.radius), collider.bounds.center.y, collider.bounds.center.z + (-_hits[_nearestHitIndex].normal.x * collider.radius)), _flatVelocity, out _finalHit, _flatVelocity.magnitude, whatCollide)) continue;
                }

                // Push the touched fruit if one, or stun a player if hit one
                PAF_Fruit _fruit = _finalHit.collider.GetComponent<PAF_Fruit>();
                if (_fruit) _fruit.AddForce(_flatVelocity);
                else
                {
                    PAF_Player _player = _finalHit.collider.GetComponent<PAF_Player>();
                    if (_player) _player.Stun();
                }

                // Set new position & velocity
                transform.position += _flatVelocity * _finalHit.distance;

                _flatVelocity = new Vector3(velocity.x + _finalHit.normal.x, 0, velocity.z + _finalHit.normal.z).normalized * _flatVelocity.magnitude * 1.1f;
                velocity = new Vector3(_flatVelocity.x, velocity.y, _flatVelocity.z);
            }
            else
            {
                transform.position += velocity;

                velocity = new Vector3(velocity.x * .9875f, velocity.y - (Physics.gravity.magnitude * (weight / 10) * Time.fixedDeltaTime), velocity.z * .9875f);
            }

            yield return new WaitForFixedUpdate();
        }

        applyForceCoroutine = null;
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
