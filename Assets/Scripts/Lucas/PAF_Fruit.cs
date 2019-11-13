using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAF_Fruit : MonoBehaviour
{
    /*
     * TO DO :
     *      • Améliorer l'accélération / décélération
    */

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
    /// Transform of the fruit renderer.
    /// </summary>
    [SerializeField] private new Transform renderer = null;

    /// <summary>
    /// Collider of the object.
    /// </summary>
    [SerializeField] private new SphereCollider collider = null;

    /// <summary>
    /// Indicates if the object is doomed or still interactable.
    /// </summary>
    private bool isDoomed = false;

    /// <summary>
    /// Indicates if freezing object rotation around the X axis.
    /// </summary>
    [SerializeField] private bool doFreezeXRotation = false;

    /// <summary>
    /// Is the fruit above ground or falling down ?
    /// </summary>
    [SerializeField] private bool isFalling = false;

    /// <summary>
    /// Original height of the renderer pivot.
    /// </summary>
    private float originalPivotHeight = 0;

    /// <summary>
    /// Weight of the fruit, influencing its movement force.
    /// </summary>
    [SerializeField, Min(.01f)] private float weight = 1;

    /// <summary>
    /// Get the fruit weight.
    /// </summary>
    public float Weight { get { return weight; } }

    /// <summary>
    /// Index of the autom aim curve array indicating current progress (0 is no curve used).
    /// </summary>
    [SerializeField] private int autoAimCurveIndex = 0;

    /// <summary>
    /// Score value of the fruit, determining by how many a players score is increased when this fruit is eaten.
    /// </summary>
    [SerializeField] private int fruitScore = 100;

    /// <summary>
    /// Ground layer mask.
    /// </summary>
    [SerializeField] private LayerMask whatIsGround = new LayerMask();

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

    /// <summary>
    /// Original size of the fruit, before modification.
    /// </summary>
    private Vector3 originalSize = Vector3.one;

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
            autoAimCurveIndex = 0;

            if (applyForceCoroutine != null) StopCoroutine(applyForceCoroutine);
            if (value != Vector3.zero)
            {
                applyForceCoroutine = StartCoroutine(ApplyForce());
            }
        }
    }

    /// <summary>
    /// All positions used for the auto aim Bézier curve.
    /// </summary>
    private Vector3[] autoAimCurve = new Vector3[16];
    #endregion

    #region Sounds
    /// <summary>
    /// Audio source of the object.
    /// </summary>
    [SerializeField] AudioSource audioSource = null;
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

    /// <summary>
    /// Position used to debug spheres collision raycast.
    /// </summary>
    [SerializeField] private Vector3 sphereDebug = new Vector3();
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
    public void AddForce(Vector3 _force)
    {
        Velocity += _force;
    }

    /// <summary>
    /// Adds a force to the fruit, making it go in this direction.
    /// </summary>
    /// <param name="_force">Force to apply.</param>
    public void AddForce(Vector3 _force, PAF_Player _player)
    {
        if (isDoomed) return;

        pointsOwner = _player;
        AddForce(_force);

        // Auto aim closest flower in near enough
        foreach (PAF_Flower _flower in PAF_Flower.Flowers)
        {
            if ((Vector3.Angle(collider.bounds.center + flatVelocity, collider.bounds.center + (_flower.MouthTransform.position - collider.bounds.center)) < 35) && ((_flower.MouthTransform.position - transform.position).magnitude < 10))
            {
                TargetPosition(_flower.MouthTransform.position);
                break;
            }
        }
    }

    /// <summary>
    /// Apply a force to the fruit.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ApplyForce()
    {
        Vector3 _newPosition;
        Vector3 _newScale;
        Vector3 _flatVelocity;
        Vector3 _normal;
        Vector3 _nFlatVelocity;
        Vector3[] _raycastPos = new Vector3[3];
        RaycastHit[] _hits = new RaycastHit[3];
        RaycastHit _finalHit = new RaycastHit();
        int _nearestHitIndex = 0;

        while (velocity != Vector3.zero)
        {
            yield return new WaitForFixedUpdate();

            _flatVelocity = flatVelocity * Time.fixedDeltaTime;

            if (_flatVelocity != Vector3.zero)
            {
                // If following a curve, recalculate path
                if (autoAimCurveIndex > 0)
                {
                    //Debug.Log("Curve => " + autoAimCurveIndex);
                    float _force = _flatVelocity.magnitude;
                    float _distance;
                    Vector3 _lastPos = collider.bounds.center;
                    while (autoAimCurveIndex > 0)
                    {
                        _distance = (autoAimCurve[autoAimCurveIndex] - _lastPos).magnitude;
                        if (_distance < _force)
                        {
                            if (autoAimCurveIndex < 15)
                            {
                                _force -= _distance;
                                _lastPos = autoAimCurve[autoAimCurveIndex];

                                autoAimCurveIndex++;
                            }
                            else autoAimCurveIndex = 0;
                        }
                        else break;
                    }

                    if (autoAimCurveIndex > 0)
                    {
                        // Set new velocity
                        Vector3 _newVelocity = (autoAimCurve[autoAimCurveIndex] - collider.bounds.center).normalized * flatVelocity.magnitude;
                        velocity = new Vector3(_newVelocity.x, velocity.y, _newVelocity.z);

                        _flatVelocity = _newVelocity * Time.fixedDeltaTime;
                    }
                }

                // Calculate normal & raycasts position
                _nFlatVelocity = _flatVelocity.normalized;
                _normal = new Vector3(_nFlatVelocity.z, 0, -_nFlatVelocity.x);
                _raycastPos = new Vector3[] { collider.bounds.center + (_nFlatVelocity * (collider.bounds.extents.x - .01f)), collider.bounds.center + (_normal * (collider.bounds.extents.x - .01f)), collider.bounds.center - (_normal * (collider.bounds.extents.x - .01f)) };

                // Raycast from side extrem points and front center, and get closest touched object if one
                _nearestHitIndex = 0;
                if (!Physics.Raycast(_raycastPos[0], _nFlatVelocity, out _hits[0], _flatVelocity.magnitude + .01f, whatCollide, QueryTriggerInteraction.Ignore))
                {
                    _hits[0].distance = _flatVelocity.magnitude + collider.bounds.extents.x + .01f;
                }
                else if (_hits[0].collider is BoxCollider) _hits[0].distance += collider.bounds.extents.x;

                for (int _i = 1; _i < 3; _i++)
                {
                    if (Physics.Raycast(_raycastPos[_i], _nFlatVelocity, out _hits[_i], _flatVelocity.magnitude + collider.bounds.extents.x + .01f, whatCollide, QueryTriggerInteraction.Ignore))
                    {
                        //Debug.Log("HIIIIT => Distance : " + _hits[_i].distance + " | 0 : " + _hits[0].distance);
                        if (_hits[_i].distance < _hits[_nearestHitIndex].distance)
                        {
                            //Debug.Log("Good");
                            _nearestHitIndex = _i;
                        }
                    }
                }

                // If hit something, bounce on it and recalculate trajectory
                if (_hits[_nearestHitIndex].collider)
                {
                    //Debug.Log("Hit => " + _hits[_nearestHitIndex].transform.name + " | From => " + (_nearestHitIndex == 0 ? "Center" : _nearestHitIndex == 1 ? "Right" : "Left"));

                    // Play bounce Sound
                    if (audioSource)
                    {
                        AudioClip _clip = PAF_GameManager.Instance?.SoundDatas?.GetFruitBounce();
                        if (_clip) audioSource.PlayOneShot(_clip);
                    }

                    // Bounce on a Sphere collider
                    if (_hits[_nearestHitIndex].collider is SphereCollider _sphere)
                    {
                        int _side = ((_hits[_nearestHitIndex].point - (_sphere.bounds.center + (_sphere.bounds.extents.x * _normal))).magnitude < (_hits[_nearestHitIndex].point - (_sphere.bounds.center - (_sphere.bounds.extents.x * _normal))).magnitude) ? 1 : -1;

                        Vector3 _edge;
                        if (_nearestHitIndex == 0)
                        {
                            _edge = _sphere.bounds.center;
                        }
                        else
                        {
                            _edge = _sphere.bounds.center + (_normal * _side * _sphere.bounds.extents.x);
                        }

                        Vector3 _intersection = _sphere.bounds.center + (Mathf.Abs(Mathf.Cos(Vector3.Angle(-_normal, (_hits[_nearestHitIndex].point - _sphere.bounds.center).normalized) * Mathf.Deg2Rad)) * _normal * _side * _sphere.bounds.extents.x);
                        Vector3 _rayPos = _edge - ((_edge - _intersection) / 2);

                        #if UNITY_EDITOR
                        sphereDebug = _rayPos;
                        #endif

                        if (!Physics.Raycast(_rayPos, -_nFlatVelocity, out _finalHit, _hits[_nearestHitIndex].distance + _sphere.bounds.extents.x, whatCollide, QueryTriggerInteraction.Ignore) || !Physics.Raycast(_finalHit.point, _nFlatVelocity, out _finalHit, _hits[_nearestHitIndex].distance, whatCollide, QueryTriggerInteraction.Ignore))
                        {
                            //Debug.Log("Lost Sphere " + _side);
                            _finalHit = _hits[_nearestHitIndex];
                            _finalHit.distance = 0;
                        }
                        //else Debug.Log("Bounce Sphere " + _side);
                    }
                    // Bounce on a Box Collider
                    else
                    {
                        // Raycast from the point that should hit other collider to get distance between
                        if (!Physics.Raycast(new Vector3(collider.bounds.center.x - (_hits[_nearestHitIndex].normal.normalized.x * (collider.bounds.extents.x - .01f)), collider.bounds.center.y, collider.bounds.center.z - (_hits[_nearestHitIndex].normal.normalized.z * (collider.bounds.extents.x - .01f))), _nFlatVelocity, out _finalHit, collider.bounds.extents.x + _flatVelocity.magnitude + .01f, whatCollide, QueryTriggerInteraction.Ignore))
                        {
                            //Debug.Log("Lost Box");
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

                    #if UNITY_EDITOR
                    yield return new WaitForFixedUpdate();
                    #endif

                    // Interact with touched collider
                    InteractWith(_finalHit.collider);

                    // Calculate new position
                    _newPosition = transform.position + _nFlatVelocity * (Mathf.Min(_finalHit.distance, _flatVelocity.magnitude) - .015f);

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
                            if (Physics.Raycast(_newPosition, _direction, out _otherHit, collider.bounds.extents.x + Mathf.Max(_collider.bounds.extents.x, _collider.bounds.extents.z), whatCollide, QueryTriggerInteraction.Ignore))
                            {
                                _distance = _newPosition - _otherHit.point;
                                _distance = new Vector3(Mathf.Abs(_distance.x), 0, Mathf.Abs(_distance.z));

                                _newPosition += Vector3.Scale(_otherHit.normal, new Vector3(collider.bounds.extents.x - _distance.x, 0, collider.bounds.extents.z - _distance.z));
                            }

                            if (_collider != _finalHit.collider) InteractWith(_collider);
                        }
                    }

                    // Set new position & velocity
                    transform.position = _newPosition;

                    _flatVelocity = Vector3.Reflect(_flatVelocity, _finalHit.normal) * .8f;
                    velocity = new Vector3(_flatVelocity.x / Time.fixedDeltaTime, velocity.y, _flatVelocity.z / Time.fixedDeltaTime);

                    // Stop following curve if doing so
                    autoAimCurveIndex = 0;

                    #if UNITY_EDITOR
                    AddCollisionPoint();

                    Physics.SyncTransforms();
                    lastHitRay[0] = _raycastPos[_nearestHitIndex];
                    lastHitRay[1] = _raycastPos[_nearestHitIndex] + (_nFlatVelocity * (Mathf.Min(_finalHit.distance, _flatVelocity.magnitude) - .015f));
                    #endif
                }
                else
                {
                    transform.position += _flatVelocity;

                    if (_flatVelocity.magnitude < .5f) _flatVelocity *= .975f;
                    velocity = new Vector3((_flatVelocity.x * .99f) / Time.fixedDeltaTime, velocity.y, (_flatVelocity.z * .99f) / Time.fixedDeltaTime);

                    renderer.Rotate(_normal, Time.fixedDeltaTime * (_flatVelocity.magnitude / Time.fixedDeltaTime) * 75);

                    if (doFreezeXRotation)
                    {
                        renderer.rotation = Quaternion.Euler(90, renderer.eulerAngles.y, renderer.eulerAngles.z);
                    }
                }
            }
            // Verticality calculs
            if (velocity.y != 0)
            {
                renderer.position = new Vector3(renderer.position.x, renderer.position.y + (velocity.y * Time.fixedDeltaTime * 5), renderer.position.z);

                if (velocity.y > 0)
                {
                    if (renderer.position.y > 10f)
                    {
                        // This is the sky
                        velocity.y *= -.75f;
                        continue;
                    }
                    _newScale = renderer.localScale * (1 + (velocity.y / 100));
                    if (velocity.y < .5f) velocity.y *= .875f;
                    else velocity.y *= .925f;

                    if (velocity.y < .2f) velocity.y *= -1;
                }
                else
                {
                    if (renderer.position.y < -7.5f)
                    {
                        // Instantiate plouf FX
                        Destroy(gameObject);
                        yield break;
                    }
                    if (!isFalling && (renderer.position.y < originalPivotHeight))
                    {
                        renderer.position = new Vector3(renderer.position.x, renderer.position.y + (originalPivotHeight - renderer.position.y), renderer.position.z);
                        renderer.localScale = originalSize;
                        velocity.y = 0;

                        continue;
                    }

                    _newScale = renderer.localScale / (-1 + (velocity.y / 100));
                    if (velocity.y > -100)
                    {
                        if (velocity.y > -1) velocity.y *= 1.1f;
                        else velocity.y *= 1.05f;
                    }
                }
                renderer.localScale = new Vector3(Mathf.Abs(_newScale.x), Mathf.Abs(_newScale.y), Mathf.Abs(_newScale.z));
            }
        }

        applyForceCoroutine = null;
    }

    /// <summary>
    /// Checks if the object is stll on ground.
    /// </summary>
    public void CheckGround()
    {
        Vector3[] _raycastPos = new Vector3[] { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };

        for (int _i = 0; _i < 4; _i++)
        {
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + .05f, transform.position.z) + (_raycastPos[_i] * collider.bounds.extents.x), Vector3.down, 1, whatIsGround, QueryTriggerInteraction.Ignore))
            {
                if (isFalling)
                {
                    Velocity = new Vector3(velocity.x, 0, velocity.z);
                    if (renderer.position.y < originalPivotHeight) renderer.position = new Vector3(renderer.position.x, renderer.position.y + (originalPivotHeight - renderer.position.y), renderer.position.z);
                    renderer.localScale = originalSize;

                    isFalling = false;
                    CancelInvoke("DoomFruit");
                }
                return;
            }
        }
        
        if (!isFalling)
        {
            isFalling = true;
            Invoke("DoomFruit", 1f);

            if (velocity.y == 0) Velocity -= new Vector3(0, .0001f, 0);
        }
        else if (flatVelocity.magnitude < .3f)
        {
            Velocity -= new Vector3(0, .01f, 0);

            CancelInvoke("DoomFruit");
            DoomFruit();
        }
    }

    /// <summary>
    /// The fruit become doomed.
    /// </summary>
    private void DoomFruit()
    {
        collider.enabled = false;
        autoAimCurveIndex = 0;
        isDoomed = true;
    }

    /// <summary>
    /// Makes the fruit be eaten.
    /// </summary>
    public void Eat()
    {
        // Eat FX
        ParticleSystem _system = PAF_GameManager.Instance?.VFXDatas?.FruitFX;
        if (_system) Instantiate(_system.gameObject, transform.position, Quaternion.identity);

        if (pointsOwner) OnFruitEaten?.Invoke(pointsOwner.IsPlayerOne, fruitScore);
        Destroy(gameObject);
    }

    /// <summary>
    /// Follows a given transform.
    /// </summary>
    /// <param name="_toFollow">Transform of to follow.</param>
    /// <returns>IEnumerator, baby.</returns>
    private IEnumerator FollowPlant(Transform _toFollow)
    {
        if (velocity.y > 0) velocity.y = -1;
        while (true)
        {
            if (transform.localScale.x > .5f)
            {
                transform.localScale *= .95f;
            }

            Vector3 _newVelocity = _toFollow.position - renderer.position;
            float _magnitude = flatVelocity.magnitude;
            if (_magnitude > 25) _magnitude *= .975f;
            else if ((_magnitude < 5f) && (_newVelocity.magnitude > .25f)) _magnitude = 5f;

            Velocity = _newVelocity.normalized * _magnitude;

            yield return null;
        }
    }

    /// <summary>
    /// Interact with a collider, with specific behaviour for Frutis & Players.
    /// </summary>
    /// <param name="_collider"></param>
    private void InteractWith(Collider _collider)
    {
        if (!_collider) return;

        // Push the touched fruit if one, or stun a player if hit one
        PAF_Fruit _fruit = _collider.GetComponent<PAF_Fruit>();
        if (_fruit) _fruit.AddForce(flatVelocity * .8f);
        /*else if (velocity.magnitude > 10f)
        {
            PAF_Player _player = _collider.GetComponent<PAF_Player>();
            if (_player && !_player.Equals(pointsOwner)) _player.Stun(transform.position);
        }*/
    }

    /// <summary>
    /// Use this when the plant start eating the fruit to make it the plant mouth.
    /// </summary>
    /// <param name="_toFollow">Transform of the plant mouth to follow.</param>
    public void StartToEat(Transform _toFollow)
    {
        if (isDoomed) return;

        DoomFruit();
        StartCoroutine(FollowPlant(_toFollow));
    }

    /// <summary>
    /// Targets a position for modifying trajectory to get there with a Bézier curve.
    /// </summary>
    private void TargetPosition(Vector3 _position)
    {
        Vector3 _p0 = collider.bounds.center;
        Vector3 _p2 = _position;
        Vector3 _p1 = _p0 + (flatVelocity.normalized * ((_p2 - _p0).magnitude / 1.25f) * (1 + (flatVelocity.magnitude * Time.fixedDeltaTime * .5f)));
        float _t;

        autoAimCurve[0] = _p0;
        autoAimCurve[15] = _p2;

        for (int _i = 1; _i < 15; _i++)
        {
            _t = _i / 15f;
            autoAimCurve[_i] = (Mathf.Pow(1f - _t, 2f) * _p0) + (2f * (1f - _t) * _t * _p1) + ((_t * _t) * _p2);
            autoAimCurve[_i].y = 0;
        }

        autoAimCurveIndex = 1;
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
        if (!renderer)
        {
            renderer = transform.GetChild(0);
            if (!renderer) Debug.LogError($"The Fruit \"{name}\" don't have a renderer attached to it !");
            else Debug.LogWarning($"The Fruit \"{name}\" don't have renderer linked to it !");
        }
        #endif

        // Get original size
        originalSize = renderer.localScale;
        originalPivotHeight = renderer.position.y;


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

        if (sphereDebug != Vector3.zero)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawCube(sphereDebug, Vector3.one * .1f);
        }

        if (collider)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(collider.bounds.center + (flatVelocity.normalized * collider.bounds.extents.x), .1f);
            Gizmos.DrawSphere(collider.bounds.center + (new Vector3(velocity.z, 0, -velocity.x).normalized * collider.bounds.extents.x), .1f);
            Gizmos.DrawSphere(collider.bounds.center - (new Vector3(velocity.z, 0, -velocity.x).normalized * collider.bounds.extents.x), .1f);

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(collider.bounds.center + (flatVelocity.normalized * collider.bounds.extents.x) + (flatVelocity * Time.fixedDeltaTime), .1f);
            Gizmos.DrawLine(collider.bounds.center + (flatVelocity.normalized * collider.bounds.extents.x), collider.bounds.center + (flatVelocity.normalized * collider.bounds.extents.x) + (flatVelocity * Time.fixedDeltaTime));

            Gizmos.DrawSphere(collider.bounds.center + (new Vector3(velocity.z, 0, -velocity.x).normalized * collider.bounds.extents.x) + (flatVelocity * Time.fixedDeltaTime) + (flatVelocity.normalized * collider.bounds.extents.x), .1f);
            Gizmos.DrawLine(collider.bounds.center + (new Vector3(velocity.z, 0, -velocity.x).normalized * collider.bounds.extents.x), collider.bounds.center + (new Vector3(velocity.z, 0, -velocity.x).normalized * collider.bounds.extents.x) + (flatVelocity * Time.fixedDeltaTime) + (flatVelocity.normalized * collider.bounds.extents.x));

            Gizmos.DrawSphere(collider.bounds.center - (new Vector3(velocity.z, 0, -velocity.x).normalized * collider.bounds.extents.x) + (flatVelocity * Time.fixedDeltaTime) + (flatVelocity.normalized * collider.bounds.extents.x), .1f);
            Gizmos.DrawLine(collider.bounds.center - (new Vector3(velocity.z, 0, -velocity.x).normalized * collider.bounds.extents.x), collider.bounds.center - (new Vector3(velocity.z, 0, -velocity.x).normalized * collider.bounds.extents.x) + (flatVelocity * Time.fixedDeltaTime) + (flatVelocity.normalized * collider.bounds.extents.x));
        }
    }
    #endif

    // Destroying the attached Behaviour will result in the game or Scene receiving OnDestroy
    private void OnDestroy()
    {
        // Remove this fruit from the arena list when destroyed
        if (arenaFruits.Contains(this)) arenaFruits.Remove(this);
    }

    // Update is called every frame, if the MonoBehaviour is enabled
    private void Update()
    {
        if (!isDoomed) CheckGround();
    }
    #endregion

    #endregion
}
