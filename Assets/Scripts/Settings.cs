using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Settings : ScriptableObject
{
    public float MinYobaSize = 0.2f;
    public float MaxYobaSize = 3.5f;

    public float DelayYobaRotation = 3.0f;
    public float RotationDegreeBorder = 35.0f;

    public float MinPushForce = 2.0f;
    public float MaxPushForce = 15.0f;

    public float TouchAnimationDuration = 2.0f;
    public GameObject Yoba;
}
