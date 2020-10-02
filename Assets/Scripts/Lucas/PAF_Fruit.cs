using System.Collections.Generic;
using UnityEngine;

public class PAF_Fruit : MonoBehaviour
{
    #region Fields / Properties
    /// <summary>
    /// Reference list of all fruits currently on the arena.
    /// </summary>
    public static List<PAF_Fruit> ArenaFruits = new List<PAF_Fruit>();

    #region Parameters
    [SerializeField] private new Transform renderer = null;

    [SerializeField] private new Rigidbody rigidbody = null;
    [SerializeField] private new SphereCollider collider = null;

    [SerializeField] private AudioSource audioSource = null;

    private bool isDoomed = false;

    [SerializeField] private bool isGolden = false;
    [SerializeField] private bool doFreezeXRotation = false;
    [SerializeField] private bool isFalling = false;

    private float originalPivotHeight = 0;

    [SerializeField] private int autoAimCurveIndex = 0;
    [SerializeField] private int fruitScore = 100;

    [SerializeField] private LayerMask whatIsGround = new LayerMask();
    [SerializeField] private LayerMask whatCollide = new LayerMask();

    private Vector3 flatVelocity { get { return new Vector3(velocity.x, 0, velocity.z); } }
    private Vector3 originalSize = Vector3.one;

    [SerializeField] private Vector3 velocity = new Vector3();
    public Vector3 Velocity
    {
        get { return velocity; }
        set
        {
            if (isGolden)
                value = new Vector3(Mathf.Clamp(value.x, -25, 25), Mathf.Clamp(value.y, -25, 25), Mathf.Clamp(value.z, -25, 25));
            else
                value = new Vector3(Mathf.Clamp(value.x, -150, 150), Mathf.Clamp(value.y, -150, 150), Mathf.Clamp(value.z, -150, 150));

            velocity = value;
            autoAimCurveIndex = 0;
        }
    }

    private Vector3[] autoAimCurve = new Vector3[16];
    #endregion

    #region Editor
    #if UNITY_EDITOR
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

    private int ownerID = -1;
    #endregion

    #region Methods

    #region Original Methods

    #if UNITY_EDITOR
    /// <summary>
    /// Adds a new collision point for debug & gizmos purpose.
    /// </summary>
    private void AddCollisionPoint()
    {
        if (collisionPos.Count > 15)
            collisionPos.RemoveAt(0);

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
        if (isDoomed)
            return;

        AddForce(_force);
        ownerID = _player.IsPlayerOne ? 1 : 2;

        // Auto aim closest flower in near enough
        foreach (PAF_Flower _flower in PAF_Flower.Flowers)
        {
            Vector3 _direction = _flower.MouthTransform.position - collider.bounds.center;
            float _magnitude = _direction.magnitude;
            float _angle = Vector3.Angle(collider.bounds.center + flatVelocity, collider.bounds.center + _direction);

            if ((_angle < 15) || ((_angle < 30) && (_magnitude < 12)) || (_magnitude < 1))
            {
                if (!Physics.Raycast(collider.bounds.center, _direction, _magnitude, whatCollide))
                {
                    TargetPosition(_flower.MouthTransform.position);
                    break;
                }
            }
        }
    }

    private static readonly Vector3[] raycastPos = new Vector3[3];
    private static readonly RaycastHit[] hits = new RaycastHit[3];

    private static readonly Collider[] overlapBuffer = new Collider[8];
    private static readonly Vector3[] raycastDir = new Vector3[] { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };

    private float doomTimer = 0;

    [SerializeField] private float deceleration = 5;

    /// <summary>
    /// Apply a force to the fruit.
    /// </summary>
    private void ApplyForce()
    {
        if (velocity == Vector3.zero)
            return;

        int _nearestHitIndex = 0;
        RaycastHit _finalHit = new RaycastHit();

        Vector3 _flatVelocity = flatVelocity * Time.deltaTime;
        if (_flatVelocity != Vector3.zero)
        {
            // If following a curve, recalculate path
            if (autoAimCurveIndex > 0)
            {
                Vector3 _lastPos = collider.bounds.center;
                float _force = _flatVelocity.magnitude;
                float _distance;
                
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
                    else
                        break;
                }

                if (autoAimCurveIndex > 0)
                {
                    // Set new velocity
                    Vector3 _newVelocity = (autoAimCurve[autoAimCurveIndex] - collider.bounds.center).normalized * flatVelocity.magnitude;
                    velocity.Set(_newVelocity.x, velocity.y, _newVelocity.z);

                    _flatVelocity = _newVelocity * Time.deltaTime;
                }
            }

            // Calculate normal & raycasts position
            Vector3 _nFlatVelocity = _flatVelocity.normalized;
            Vector3 _normal = new Vector3(_nFlatVelocity.z, 0, -_nFlatVelocity.x);

            raycastPos[0] = collider.bounds.center + (_nFlatVelocity * (collider.bounds.extents.x - .01f));
            raycastPos[1] = collider.bounds.center + (_normal * (collider.bounds.extents.x - .01f));
            raycastPos[2] = collider.bounds.center - (_normal * (collider.bounds.extents.x - .01f));

            // Raycast from side extrem points and front center, and get closest touched object if one
            _nearestHitIndex = 0;
            if (!Physics.Raycast(raycastPos[0], _nFlatVelocity, out hits[0], _flatVelocity.magnitude + .01f, whatCollide, QueryTriggerInteraction.Ignore))
            {
                hits[0].distance = _flatVelocity.magnitude + collider.bounds.extents.x + .01f;
            }
            else
            if (hits[0].collider is BoxCollider) hits[0].distance += collider.bounds.extents.x;

            for (int _i = 1; _i < 3; _i++)
            {
                if (Physics.Raycast(raycastPos[_i], _nFlatVelocity, out hits[_i], _flatVelocity.magnitude + collider.bounds.extents.x + .01f, whatCollide, QueryTriggerInteraction.Ignore))
                {
                    if (hits[_i].distance < hits[_nearestHitIndex].distance)
                        _nearestHitIndex = _i;
                }
            }

            // If hit something, bounce on it and recalculate trajectory
            if (hits[_nearestHitIndex].collider)
            {
                // Play bounce Sound
                audioSource.PlayOneShot(PAF_GameManager.Instance.SoundDatas.GetFruitBounce());

                // Bounce on a Sphere collider
                if (hits[_nearestHitIndex].collider is SphereCollider _sphere)
                {
                    int _side = ((hits[_nearestHitIndex].point - (_sphere.bounds.center + (_sphere.bounds.extents.x * _normal))).magnitude < (hits[_nearestHitIndex].point - (_sphere.bounds.center - (_sphere.bounds.extents.x * _normal))).magnitude) ? 1 : -1;

                    Vector3 _edge;
                    if (_nearestHitIndex == 0)
                    {
                        _edge = _sphere.bounds.center;
                    }
                    else
                    {
                        _edge = _sphere.bounds.center + (_normal * _side * _sphere.bounds.extents.x);
                    }

                    Vector3 _intersection = _sphere.bounds.center + (Mathf.Abs(Mathf.Cos(Vector3.Angle(-_normal, (hits[_nearestHitIndex].point - _sphere.bounds.center).normalized) * Mathf.Deg2Rad)) * _normal * _side * _sphere.bounds.extents.x);
                    Vector3 _rayPos = _edge - ((_edge - _intersection) / 2);

                    #if UNITY_EDITOR
                    sphereDebug = _rayPos;
                    #endif

                    if (!Physics.Raycast(_rayPos, -_nFlatVelocity, out _finalHit, hits[_nearestHitIndex].distance + _sphere.bounds.extents.x, whatCollide, QueryTriggerInteraction.Ignore) ||
                        !Physics.Raycast(_finalHit.point, _nFlatVelocity, out _finalHit, hits[_nearestHitIndex].distance, whatCollide, QueryTriggerInteraction.Ignore))
                    {
                        _finalHit = hits[_nearestHitIndex];
                        _finalHit.distance = 0;
                    }
                }
                // Bounce on a Box Collider
                else
                {
                    // Raycast from the point that should hit other collider to get distance between
                    if (!Physics.Raycast(new Vector3(collider.bounds.center.x - (hits[_nearestHitIndex].normal.normalized.x * (collider.bounds.extents.x - .01f)), collider.bounds.center.y, collider.bounds.center.z - (hits[_nearestHitIndex].normal.normalized.z * (collider.bounds.extents.x - .01f))), _nFlatVelocity, out _finalHit, collider.bounds.extents.x + _flatVelocity.magnitude + .01f, whatCollide, QueryTriggerInteraction.Ignore))
                    {
                        _finalHit = hits[_nearestHitIndex];
                        _finalHit.distance = 0;
                    }
                    #if UNITY_EDITOR
                    else
                    {
                        raycastPos[_nearestHitIndex] = new Vector3(collider.bounds.center.x - (hits[_nearestHitIndex].normal.normalized.x * (collider.bounds.extents.x - .01f)), collider.bounds.center.y, collider.bounds.center.z - (hits[_nearestHitIndex].normal.normalized.z * (collider.bounds.extents.x - .01f)));
                    }
                    #endif
                }

                // Interact with touched collider
                InteractWith(_finalHit.collider);

                // Calculate new position
                Vector3 _newPosition = transform.position + _nFlatVelocity * (Mathf.Min(_finalHit.distance, _flatVelocity.magnitude) - .015f);

                int _amount = Physics.OverlapSphereNonAlloc(_newPosition, collider.bounds.extents.x, overlapBuffer, whatCollide);
                if (_amount > 0)
                {
                    RaycastHit _otherHit;
                    Vector3 _direction;
                    Vector3 _distance;

                    for (int _i = 0; _i < _amount; _i++)
                    {
                        Collider _collider = overlapBuffer[_i];

                        if (_collider != collider)
                        {
                            _direction = _collider.bounds.center - _newPosition;
                            if (Physics.Raycast(_newPosition, _direction, out _otherHit, collider.bounds.extents.x + Mathf.Max(_collider.bounds.extents.x, _collider.bounds.extents.z), whatCollide, QueryTriggerInteraction.Ignore))
                            {
                                _distance = _newPosition - _otherHit.point;
                                _distance = new Vector3(Mathf.Abs(_distance.x), 0, Mathf.Abs(_distance.z));

                                _newPosition += Vector3.Scale(_otherHit.normal, new Vector3(collider.bounds.extents.x - _distance.x, 0, collider.bounds.extents.z - _distance.z));
                            }

                            if (_collider != _finalHit.collider)
                                InteractWith(_collider);
                        }
                    }
                }

                // Set new position & velocity
                rigidbody.position = _newPosition;
                transform.position = _newPosition;

                _flatVelocity = Vector3.Reflect(_flatVelocity, _finalHit.normal) * .8f;
                velocity = new Vector3(_flatVelocity.x / Time.deltaTime, velocity.y, _flatVelocity.z / Time.deltaTime);

                // Stop following curve if doing so
                autoAimCurveIndex = 0;

                #if UNITY_EDITOR
                AddCollisionPoint();

                lastHitRay[0] = raycastPos[_nearestHitIndex];
                lastHitRay[1] = raycastPos[_nearestHitIndex] + (_nFlatVelocity * (Mathf.Min(_finalHit.distance, _flatVelocity.magnitude) - .015f));
                #endif
            }
            else
            {
                rigidbody.position += _flatVelocity;
                transform.position += _flatVelocity;

                if (velocity.magnitude < .5f)
                    _flatVelocity.Set(Mathf.MoveTowards(_flatVelocity.x, 0, deceleration * Time.deltaTime * .1f),
                                      Mathf.MoveTowards(_flatVelocity.y, 0, deceleration * Time.deltaTime * .1f),
                                      Mathf.MoveTowards(_flatVelocity.z, 0, deceleration * Time.deltaTime * .1f));

                velocity.Set(Mathf.MoveTowards(_flatVelocity.x / Time.deltaTime, 0, deceleration * Time.deltaTime), velocity.y, Mathf.MoveTowards(_flatVelocity.z / Time.deltaTime, 0, deceleration * Time.deltaTime));

                renderer.Rotate(_normal, Time.deltaTime * (_flatVelocity.magnitude / Time.deltaTime) * 75);
                if (doFreezeXRotation)
                {
                    renderer.rotation = Quaternion.Euler(90, renderer.eulerAngles.y, renderer.eulerAngles.z);
                }
            }
        }

        // Verticality calculs
        Vector3 _newScale;
        if (velocity.y != 0)
        {
            renderer.position = new Vector3(renderer.position.x, renderer.position.y + (velocity.y * Time.deltaTime * 5), renderer.position.z);

            if (velocity.y > 0)
            {
                if (renderer.position.y > 10f)
                {
                    // This is the sky
                    velocity.y *= -.75f;
                    return;
                }

                _newScale = renderer.localScale * (1 + (velocity.y / 100));

                if (velocity.y < .5f)
                    velocity.y *= .875f;
                else
                    velocity.y *= .925f;

                if (velocity.y < .2f)
                    velocity.y *= -1;
            }
            else
            {
                if (renderer.position.y < -7.5f)
                {
                    // Instantiate plouf FX
                    Destroy(gameObject);
                    return;
                }

                if (!isFalling && (renderer.position.y < originalPivotHeight))
                {
                    renderer.position += new Vector3(0, originalPivotHeight - renderer.position.y, 0);

                    renderer.localScale = originalSize;
                    velocity.y = 0;
                    return;
                }

                _newScale = renderer.localScale / (-1 + (velocity.y / 100));

                if (velocity.y > -100)
                    velocity.y *= (velocity.y > -1) ? 1.1f : 1.05f;
            }

            renderer.localScale = new Vector3(Mathf.Abs(_newScale.x), Mathf.Abs(_newScale.y), Mathf.Abs(_newScale.z));
        }
    }

    /// <summary>
    /// Checks if the object is stll on ground.
    /// </summary>
    public void CheckGround()
    {
        for (int _i = 0; _i < 4; _i++)
        {
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + .05f, transform.position.z) + (raycastDir[_i] * collider.bounds.extents.x * .8f), Vector3.down, 1, whatIsGround, QueryTriggerInteraction.Ignore))
            {
                if (isFalling)
                {
                    Velocity = new Vector3(velocity.x, 0, velocity.z);
                    if (renderer.position.y != originalPivotHeight) renderer.position = new Vector3(renderer.position.x, renderer.position.y + (originalPivotHeight - renderer.position.y), renderer.position.z);
                    renderer.localScale = originalSize;

                    isFalling = false;
                    doomTimer = 0;
                }
                return;
            }
        }

        if ((renderer.position.y > originalPivotHeight) && (velocity.y == 0))
        {
            Velocity -= new Vector3(0, .1f, 0);
        }
        
        if (!isFalling)
        {
            isFalling = true;
            doomTimer = 1f;

            if (velocity.y == 0) Velocity -= new Vector3(0, .0001f, 0);
        }
        else if (flatVelocity.magnitude < 2f)
        {
            Velocity -= new Vector3(0, .01f, 0);

            doomTimer = 0;
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
        Instantiate(PAF_GameManager.Instance.VFXDatas.FruitFX.gameObject,
                    new Vector3(collider.bounds.center.x, renderer.position.y, collider.bounds.center.z), Quaternion.identity);

        if (ownerID > 0)
            PAF_GameManager.Instance.IncreasePlayerScore(ownerID == 1 ? true : false, fruitScore);

        Destroy(gameObject);
    }

    private Transform plantTransform = null;
    private bool isFollowingPlant = false;

    /// <summary>
    /// Follows a given transform.
    /// </summary>
    /// <param name="_toFollow">Transform of to follow.</param>
    private void FollowPlant()
    {
        if (transform.localScale.x > .5f)
            transform.localScale *= .95f;

        Vector3 _newVelocity = plantTransform.position - renderer.position;
        float _magnitude = flatVelocity.magnitude;
        if (_magnitude > 25) _magnitude *= .975f;
        else if ((_magnitude < 5f) && (_newVelocity.magnitude > .25f)) _magnitude = 5f;

        Velocity = _newVelocity.normalized * _magnitude;
    }

    /// <summary>
    /// Interact with a collider, with specific behaviour for Frutis & Players.
    /// </summary>
    /// <param name="_collider"></param>
    private void InteractWith(Collider _collider)
    {
        if (_collider.TryGetComponent(out PAF_Fruit _fruit))
            _fruit.AddForce(flatVelocity * .8f);
    }

    /// <summary>
    /// Use this when the plant start eating the fruit to make it the plant mouth.
    /// </summary>
    /// <param name="_toFollow">Transform of the plant mouth to follow.</param>
    public void StartToEat(Transform _toFollow)
    {
        if (isDoomed)
            return;

        DoomFruit();
        isFollowingPlant = true;
        plantTransform = _toFollow;

        if (velocity.y > 0)
            velocity.y = -1;
    }

    /// <summary>
    /// Targets a position for modifying trajectory to get there with a Bézier curve.
    /// </summary>
    private void TargetPosition(Vector3 _position)
    {
        Vector3 _p0 = collider.bounds.center;
        Vector3 _p2 = _position;
        Vector3 _p1 = _p0 + (flatVelocity.normalized * ((_p2 - _p0).magnitude / 1.25f) * (1 + (flatVelocity.magnitude * Time.deltaTime * .5f)));
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
        ArenaFruits.Add(this);

        #if UNITY_EDITOR
        collisionPos.Add(collider.bounds.center);
        #endif
    }

    #if UNITY_EDITOR
    // Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn
    private void OnDrawGizmos()
    {
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
            Gizmos.DrawSphere(collider.bounds.center + (flatVelocity.normalized * collider.bounds.extents.x) + (flatVelocity * Time.deltaTime), .1f);
            Gizmos.DrawLine(collider.bounds.center + (flatVelocity.normalized * collider.bounds.extents.x), collider.bounds.center + (flatVelocity.normalized * collider.bounds.extents.x) + (flatVelocity * Time.deltaTime));

            Gizmos.DrawSphere(collider.bounds.center + (new Vector3(velocity.z, 0, -velocity.x).normalized * collider.bounds.extents.x) + (flatVelocity * Time.deltaTime) + (flatVelocity.normalized * collider.bounds.extents.x), .1f);
            Gizmos.DrawLine(collider.bounds.center + (new Vector3(velocity.z, 0, -velocity.x).normalized * collider.bounds.extents.x), collider.bounds.center + (new Vector3(velocity.z, 0, -velocity.x).normalized * collider.bounds.extents.x) + (flatVelocity * Time.deltaTime) + (flatVelocity.normalized * collider.bounds.extents.x));

            Gizmos.DrawSphere(collider.bounds.center - (new Vector3(velocity.z, 0, -velocity.x).normalized * collider.bounds.extents.x) + (flatVelocity * Time.deltaTime) + (flatVelocity.normalized * collider.bounds.extents.x), .1f);
            Gizmos.DrawLine(collider.bounds.center - (new Vector3(velocity.z, 0, -velocity.x).normalized * collider.bounds.extents.x), collider.bounds.center - (new Vector3(velocity.z, 0, -velocity.x).normalized * collider.bounds.extents.x) + (flatVelocity * Time.deltaTime) + (flatVelocity.normalized * collider.bounds.extents.x));
        }
    }
    #endif

    // Destroying the attached Behaviour will result in the game or Scene receiving OnDestroy
    private void OnDestroy()
    {
        ArenaFruits.Remove(this);
        PAF_Flower.RemoveFruit(this);
    }

    // Update is called every frame, if the MonoBehaviour is enabled
    private void Update()
    {
        ApplyForce();

        if (!isDoomed)
        {
            CheckGround();
            if (doomTimer > 0)
            {
                doomTimer -= Time.deltaTime;
                if (doomTimer <= 0)
                    DoomFruit();
            }
        }
        else if (isFollowingPlant)
            FollowPlant();
    }
    #endregion

    #endregion
}
