using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAF_Fruit : MonoBehaviour
{
    #region Events
    /// <summary>
    /// Event called when a fruit is eaten, with as parameters :
    /// • Boolean indicating player increasing score, true if it's the player #1, false if #2 ;
    /// • Int of the fruit points value, by how many the player score is increased.
    /// </summary>
    public static event Action<bool, int> OnFruitEaten = null;
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
    /// Score value of the fruit, determining by how many a players score is increased when this fruit is eaten.
    /// </summary>
    [SerializeField] private int fruitScore = 100;

    /// <summary>
    /// Layer mask of what the fruits should collide on.
    /// </summary>
    [SerializeField] private LayerMask whatCollide = new LayerMask();

    /// <summary>
    /// Indicates to which player this fruit should increases score points when ate.
    /// </summary>
    [SerializeField] private PAF_Player pointsOwner = null;

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
            value = new Vector3(Mathf.Clamp(value.x, -999, 999), Mathf.Clamp(value.y, -999, 999), Mathf.Clamp(value.z, -999, 999));

            velocity = value;
            if (applyForceCoroutine != null) StopCoroutine(applyForceCoroutine);
            if (value != Vector3.zero)
            {
                applyForceCoroutine = StartCoroutine(ApplyForce());
            }
        }
    }

    #endregion

    #region Sounds
    /// <summary>
    /// Audio source of the object.
    /// </summary>
    [SerializeField] AudioSource audioSource = null;

    /// <summary>
    /// Scriptable Object used for sound datas. (To remove from here.)
    /// </summary>
    [SerializeField] PAF_SoundData soundData = null;
    #endregion

    #region Coroutines
    /// <summary>
    /// Current reference of <see cref="ApplyForce(Vector3)"/> coroutine.
    /// </summary>
    private Coroutine applyForceCoroutine = null;
    #endregion

    #region Editor
    #if UNITY_EDITOR
    /// <summary>
    /// Indicates if drawing the last shortest raycast hitting something or not.
    /// </summary>
    [SerializeField] private bool doDrawLastRaycastHit = true;

    /// <summary>
    /// Indicates if drawing raycasts points and direction or not.
    /// </summary>
    [SerializeField] private bool doDrawRaycasts = true;

    /// <summary>
    /// Color used to draw gizmos on this script.
    /// </summary>
    [SerializeField] private Color gizmosColor = Color.cyan;

    /// <summary>
    /// All positions where the fruit hit something to bounce on it.
    /// </summary>
    [SerializeField] private List<Vector3> collisionPos = new List<Vector3>();

    /// <summary>
    /// Array of last hitting ray positions, start at index, and hit point at 1.
    /// </summary>
    [SerializeField] private Vector3[] lastHitRay = new Vector3[2];
    #endif
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    #if UNITY_EDITOR
    /// <summary>
    /// Adds a new collision point for debug & gizmos purpose.
    /// </summary>
    private void AddCollisionPoint()
    {
        Physics.SyncTransforms();
        if (collisionPos.Count > 15) collisionPos.RemoveAt(0);
        collisionPos.Add(collider.bounds.center);
    }
    #endif

    /// <summary>
    /// Adds a force to the fruit, making it go in this direction.
    /// </summary>
    /// <param name="_force">Force to apply.</param>
    public void AddForce(Vector3 _force) => Velocity += _force;

    /// <summary>
    /// Adds a force to the fruit, making it go in this direction.
    /// </summary>
    /// <param name="_force">Force to apply.</param>
    public void AddForce(Vector3 _force, PAF_Player _player)
    {
        pointsOwner = _player;
        AddForce(_force);
    }

    /// <summary>
    /// Apply a force to the fruit.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ApplyForce()
    {
        Vector3 _flatVelocity;
        Vector3 _normal;
        Vector3 _nFlatVelocity;
        Vector3[] _raycastPos = new Vector3[3];
        RaycastHit[] _hits = new RaycastHit[3];
        RaycastHit _finalHit = new RaycastHit();
        int _nearestHitIndex = 0;

        while ((_flatVelocity = flatVelocity) != Vector3.zero)
        {
            yield return new WaitForFixedUpdate();
            
            // Calculate normal & raycasts position
            _nFlatVelocity = _flatVelocity.normalized;
            _normal = new Vector3(_nFlatVelocity.z, 0, -_nFlatVelocity.x);
            _raycastPos = new Vector3[] { collider.bounds.center + (_nFlatVelocity * (collider.bounds.extents.x - .01f)), collider.bounds.center + (_normal * (collider.bounds.extents.x - .01f)), collider.bounds.center - (_normal * (collider.bounds.extents.x - .01f)) };

            // Raycast from side extrem points and front center, and get closest touched object if one
            _nearestHitIndex = 0;
            if (!Physics.Raycast(_raycastPos[0], _nFlatVelocity, out _hits[0], _flatVelocity.magnitude + .01f, whatCollide))
            {
                _hits[0].distance = _flatVelocity.magnitude + collider.bounds.extents.x + .01f;
            }
            else _hits[0].distance += collider.bounds.extents.x;

            for (int _i = 1; _i < 3; _i++)
            {
                if (Physics.Raycast(_raycastPos[_i], _nFlatVelocity, out _hits[_i], _flatVelocity.magnitude + collider.bounds.extents.x + .01f, whatCollide))
                {
                    Debug.Log("HIIIIT => Distance : " + _hits[_i].distance + " | 0 : " + _hits[0].distance);
                    if (_hits[_i].distance < _hits[_nearestHitIndex].distance)
                    {
                        Debug.Log("Good");
                        _nearestHitIndex = _i;
                    }
                }
            }

            // If hit something, bounce on it and recalculate trajectory
            if (_hits[_nearestHitIndex].collider)
            {
                Debug.Log("Hit => " + _hits[_nearestHitIndex].transform.name + " | From => " + (_nearestHitIndex == 0 ? "Center" : _nearestHitIndex == 1 ? "Right" : "Left"));

                // Play bounce Sound
                if(audioSource && soundData)
                {
                    AudioClip _clip = soundData.GetFruitBounce();
                    if (_clip) audioSource.PlayOneShot(_clip);
                }

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
                        _edge = _sphere.bounds.center + (_normal * (_nearestHitIndex == 1 ? -1 : 1) * _sphere.bounds.extents.x);
                    }

                    Vector3 _intersection = _sphere.bounds.center + (Mathf.Cos(Vector3.Angle(_edge, _hits[_nearestHitIndex].point) * Mathf.Deg2Rad) * _normal * _sphere.bounds.extents.x);
                    Vector3 _rayPos = _edge - ((_edge - _intersection) / 2);

                    if (!Physics.Raycast(_rayPos, -_nFlatVelocity, out _finalHit, _hits[_nearestHitIndex].distance + _sphere.bounds.extents.x, whatCollide) || !Physics.Raycast(_finalHit.point, _nFlatVelocity, out _finalHit, _hits[_nearestHitIndex].distance, whatCollide))
                    {
                        Debug.Log("Lost Sphere");
                        _finalHit = _hits[_nearestHitIndex];
                        _finalHit.distance = 0;
                    }
                    else Debug.Log("Bounce Sphere");
                }
                // Bounce on a Box Collider
                else
                {
                    // Raycast from the point that should hit other collider to get distance between
                    if (!Physics.Raycast(new Vector3(collider.bounds.center.x - (_hits[_nearestHitIndex].normal.normalized.x * (collider.bounds.extents.x - .01f)), collider.bounds.center.y, collider.bounds.center.z - (_hits[_nearestHitIndex].normal.normalized.z * (collider.bounds.extents.x - .01f))), _nFlatVelocity, out _finalHit, collider.bounds.extents.x + _flatVelocity.magnitude + .01f, whatCollide))
                    {
                        Debug.Log("Lost Box");
                        _finalHit = _hits[_nearestHitIndex];
                        _finalHit.distance = 0;
                    }
                    #if UNITY_EDITOR
                    else
                    {
                        _raycastPos[_nearestHitIndex] = new Vector3(collider.bounds.center.x - (_hits[_nearestHitIndex].normal.normalized.x * (collider.bounds.extents.x - .01f)), collider.bounds.center.y, collider.bounds.center.z - (_hits[_nearestHitIndex].normal.normalized.z * (collider.bounds.extents.x - .01f)));
                    }
                    #endif
                }

                // Push the touched fruit if one, or stun a player if hit one
                PAF_Fruit _fruit = _finalHit.collider.GetComponent<PAF_Fruit>();
                if (_fruit) _fruit.AddForce(_flatVelocity);
                else if (velocity.magnitude > .1f)
                {
                    PAF_Player _player = _finalHit.collider.GetComponent<PAF_Player>();
                    if (_player) _player.Stun(transform.position);
                }

                #if UNITY_EDITOR
                yield return new WaitForFixedUpdate();
                #endif

                // Calculate new position
                Vector3 _newPosition = transform.position + _nFlatVelocity * (Mathf.Min(_finalHit.distance, _flatVelocity.magnitude) - .015f);

                Collider[] _obstacles = Physics.OverlapSphere(_newPosition, collider.bounds.extents.x, whatCollide);
                if (_obstacles.Length > 0)
                {
                    RaycastHit _otherHit;
                    Vector3 _direction;
                    Vector3 _distance;

                    foreach (Collider _collider in _obstacles)
                    {
                        if (_collider == collider) continue;

                        _direction = _collider.bounds.center - _newPosition;
                        if (Physics.Raycast(_newPosition, _direction, out _otherHit, collider.bounds.extents.x + Mathf.Max(_collider.bounds.extents.x, _collider.bounds.extents.z), whatCollide))
                        {
                            _distance = _newPosition - _otherHit.point;
                            _distance = new Vector3(Mathf.Abs(_distance.x), 0, Mathf.Abs(_distance.z));

                            _newPosition += Vector3.Scale(_otherHit.normal, new Vector3(collider.bounds.extents.x - _distance.x, 0, collider.bounds.extents.z - _distance.z));
                        }
                    }
                }

                // Set new position & velocity
                transform.position = _newPosition;

                _flatVelocity = Vector3.Reflect(_flatVelocity, _finalHit.normal) * .8f;
                velocity = new Vector3(_flatVelocity.x, velocity.y, _flatVelocity.z);

                #if UNITY_EDITOR
                AddCollisionPoint();

                Physics.SyncTransforms();
                lastHitRay[0] = _raycastPos[_nearestHitIndex];
                lastHitRay[1] = _raycastPos[_nearestHitIndex] + (_nFlatVelocity * (Mathf.Min(_finalHit.distance, _flatVelocity.magnitude) - .015f));
                #endif
            }
            else
            {
                _MoveToVelocity();
            }
        }

        applyForceCoroutine = null;
        
        // Makes the fruit moves like if no obstacle is in its way
        void _MoveToVelocity()
        {
            transform.position += _flatVelocity;

            if (_flatVelocity.magnitude < .5f) _flatVelocity *= .975f;
            velocity = new Vector3(_flatVelocity.x * .9875f, velocity.y/* - (Physics.gravity.magnitude * (weight / 10) * Time.fixedDeltaTime)*/, _flatVelocity.z * .95f);
        }
    }

    /// <summary>
    /// Makes the fruit be eaten.
    /// </summary>
    public void Eat()
    {
        if (pointsOwner) OnFruitEaten?.Invoke(pointsOwner.IsPlayerOne, fruitScore);
        Destroy(gameObject);
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

        #if UNITY_EDITOR
        collisionPos.Add(collider.bounds.center);
        #endif
    }

    #if UNITY_EDITOR
    // Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn
    private void OnDrawGizmos()
    {
        Physics.SyncTransforms();
        Gizmos.color = gizmosColor;

        // Draw gizmos tracking the fruit collisions path
        if (collisionPos.Count > 0)
        {
            Gizmos.DrawSphere(collisionPos[0], collider.bounds.extents.x);

            for (int _i = 1; _i < collisionPos.Count; _i++)
            {
                Gizmos.DrawSphere(collisionPos[_i], collider.bounds.extents.x);
                Gizmos.DrawLine(collisionPos[_i - 1], collisionPos[_i]);
            }

            Gizmos.color = new Color(1 - gizmosColor.r, 1 - gizmosColor.g, 1 - gizmosColor.b);
            Gizmos.DrawLine(collisionPos[collisionPos.Count - 1], collider.bounds.center);
        }

        if (lastHitRay[0] != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(lastHitRay[0], .1f);

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(lastHitRay[0], lastHitRay[1]);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(lastHitRay[1], .1f);
        }

        if (collider)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(collider.bounds.center + (flatVelocity.normalized * collider.bounds.extents.x), .1f);
            Gizmos.DrawSphere(collider.bounds.center + (new Vector3(velocity.z, 0, -velocity.x).normalized * collider.bounds.extents.x), .1f);
            Gizmos.DrawSphere(collider.bounds.center - (new Vector3(velocity.z, 0, -velocity.x).normalized * collider.bounds.extents.x), .1f);

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(collider.bounds.center + (flatVelocity.normalized * collider.bounds.extents.x) + flatVelocity, .1f);
            Gizmos.DrawLine(collider.bounds.center + (flatVelocity.normalized * collider.bounds.extents.x), collider.bounds.center + (flatVelocity.normalized * collider.bounds.extents.x) + flatVelocity);

            Gizmos.DrawSphere(collider.bounds.center + (new Vector3(velocity.z, 0, -velocity.x).normalized * collider.bounds.extents.x) + flatVelocity + (flatVelocity.normalized * collider.bounds.extents.x), .1f);
            Gizmos.DrawLine(collider.bounds.center + (new Vector3(velocity.z, 0, -velocity.x).normalized * collider.bounds.extents.x), collider.bounds.center + (new Vector3(velocity.z, 0, -velocity.x).normalized * collider.bounds.extents.x) + flatVelocity + (flatVelocity.normalized * collider.bounds.extents.x));

            Gizmos.DrawSphere(collider.bounds.center - (new Vector3(velocity.z, 0, -velocity.x).normalized * collider.bounds.extents.x) + flatVelocity + (flatVelocity.normalized * collider.bounds.extents.x), .1f);
            Gizmos.DrawLine(collider.bounds.center - (new Vector3(velocity.z, 0, -velocity.x).normalized * collider.bounds.extents.x), collider.bounds.center - (new Vector3(velocity.z, 0, -velocity.x).normalized * collider.bounds.extents.x) + flatVelocity + (flatVelocity.normalized * collider.bounds.extents.x));
        }
    }
    #endif

    // Destroying the attached Behaviour will result in the game or Scene receiving OnDestroy
    private void OnDestroy()
    {
        // Remove this fruit from the arena list when destroyed
        if (arenaFruits.Contains(this)) arenaFruits.Remove(this);
    }
    #endregion

    #endregion
}
