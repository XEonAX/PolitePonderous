using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody _rigidbody;
    public TrailRenderer _trailRenderer;
    private int doRecycle;

    public Explosion explosion;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        _Awake();
    }
    public void _Awake()
    {
        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }
    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        StartCoroutine(Clean());

        doRecycle = -1;
    }
    public virtual void Shoot(float force)
    {
        _rigidbody.MovePosition(transform.position);
        _rigidbody.MoveRotation(transform.rotation);
        _rigidbody.isKinematic = false;
        _rigidbody.detectCollisions = true;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        _rigidbody.velocity = SpaceShipManager.Instance.FocusedSpaceship.rb.GetPointVelocity(transform.position);
        _rigidbody.AddForce(transform.forward * force, ForceMode.VelocityChange);
        _trailRenderer.Clear();
        _trailRenderer.emitting = true;
    }

    public virtual IEnumerator Clean()
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(5);
        Recycle();
    }

    void OnCollisionEnter(Collision collision)
    {
        // ExplosionVFX.Instance.Add(collision.GetContact(0).point);
        if (doRecycle <= 0)
            doRecycle = 2;
        var asteroid = collision.gameObject.GetComponent<Asteroid>();
        if (asteroid != null)
        {
            AsteroidField.Instance.Asteroids.Remove(asteroid);
            asteroid.gameObject.SetActive(false);
            ObjectPool.Spawn<Explosion>(explosion, ObjectPool.instance.transform, transform.position, transform.rotation).Initialize(transform.position, 1);
        }
    }

    void Recycle()
    {
        _trailRenderer.emitting = false;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        _rigidbody.isKinematic = true;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        ObjectPool.Recycle(gameObject);
    }
    private void FixedUpdate()
    {
        if (doRecycle > 0)
        {
            doRecycle--;
            if (doRecycle == 0)
                Recycle();
        }
    }
}
