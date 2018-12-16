using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Yoba : MonoBehaviour
{
    Vector3 _destinationScale = new Vector3(0.0f, 0.0f, 0.0f);
    public Settings settings;

    float _rotationTimer;
    bool _isRotationActive = false;
    bool _haveContactWithFloor = false;

    Rigidbody2D _rigidbody;

    Vector3 _savedScale;
    float _touchTimer;

    public void ScaleSmooth(Vector3 DestinationScale)
    {
        _destinationScale = DestinationScale;
    }

    private void Awake()
    {
        _rotationTimer = settings.DelayYobaRotation;
        _rigidbody = GetComponent<Rigidbody2D>();
    }

	// Update is called once per frame
	void Update ()
    {
        // Handle start scaling
        if (_destinationScale.x != 0.0f && transform.localScale != _destinationScale)
        {
            UpdateSmoothScaling();
        }

        if (_rotationTimer > 0.0f)
        {
            if (_rotationTimer < Time.deltaTime)
            {
                _rotationTimer = 0.0f;
                _isRotationActive = _haveContactWithFloor;
            }
            else
            {
                _rotationTimer -= Time.deltaTime;
            }
        }

        // rotation animation
        if (_isRotationActive)
        {
            if (transform.localEulerAngles.z > settings.RotationDegreeBorder && transform.localEulerAngles.z < 180.0f + settings.RotationDegreeBorder)
            {
                float newAngle = Mathf.Lerp(transform.localEulerAngles.z, 0.0f, Time.deltaTime * 4.0f);
                transform.Rotate(new Vector3(0.0f, 0.0f, -(transform.localEulerAngles.z - newAngle)));
            }

            if (transform.localEulerAngles.z > 180.0f + settings.RotationDegreeBorder)
            {
                float newAngle = Mathf.Lerp(transform.localEulerAngles.z, 361.0f, Time.deltaTime * 4.0f);
                transform.Rotate(new Vector3(0.0f, 0.0f, (newAngle - transform.localEulerAngles.z)));
            }
        }

        // touch animation
        if (_touchTimer > 0.0f)
        {
            if (_touchTimer > Time.deltaTime)
            {
                float cosTime = Mathf.Cos(Time.time * 5.0f);
                cosTime /= 3.0f;
                float InvAnimPos = _touchTimer / settings.TouchAnimationDuration;
                float scaleFactor = cosTime * InvAnimPos;
                transform.localScale = new Vector3(_savedScale.x + (scaleFactor * _savedScale.x), _savedScale.y + (scaleFactor * _savedScale.y), 1.0f);
                _touchTimer -= Time.deltaTime;
            }
            else
            {
                _touchTimer = 0.0f;
                transform.localScale = _savedScale;
            }
        }
    }

    private void OnMouseDown()
    {
        // Play touch animation
        _savedScale = transform.localScale;
        _touchTimer = settings.TouchAnimationDuration;

        // Push yoba to random upper direction
        float startRange = Mathf.PI / 6.0f;
        float endRange = (Mathf.PI * 5.0f) / 6.0f;

        // Shift radians from side to upper range
        startRange -= Mathf.PI / 2.0f;
        endRange -= Mathf.PI / 2.0f;

        float randUpperRadian = Random.Range(startRange, endRange);
        Vector2 randPushVector = new Vector2(Mathf.Sin(randUpperRadian), Mathf.Cos(randUpperRadian));

        // Scale to random force
        float force = Random.Range(settings.MinPushForce, settings.MaxPushForce);
        randPushVector.Scale(new Vector2(force, force));

        _rigidbody.AddForce(randPushVector, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Floor"))
        {
            _rotationTimer = settings.DelayYobaRotation;
            _haveContactWithFloor = true;
        }
    }


    private int GetNumContactsWithCollider(Collision2D collision)
    {
        int ContactsWithFloor = 0;
        ContactPoint2D[] contacts = collision.contacts;
        foreach (ContactPoint2D contact in contacts)
        {
            if (contact.otherRigidbody == _rigidbody) ContactsWithFloor++;
        }
        return ContactsWithFloor;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Floor"))
        {
            int ContactsWithFloor = GetNumContactsWithCollider(collision);

            if (ContactsWithFloor == 0)
            {
                _rotationTimer = settings.DelayYobaRotation;
                _haveContactWithFloor = false;
            }
        }
    }

    void UpdateSmoothScaling()
    {
        Vector3 NewSize = Vector3.Lerp(transform.localScale, _destinationScale, Time.deltaTime * 4.0f);

        // if we almost at the end, don't interpolate further, just apply target value
        if ((_destinationScale.sqrMagnitude - NewSize.sqrMagnitude) < 0.05f)
        {
            transform.localScale = _destinationScale;
        }
        else
        {
            transform.localScale = NewSize;
        }
    }
}
