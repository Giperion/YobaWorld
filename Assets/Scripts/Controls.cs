using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Controls : MonoBehaviour {

    Camera cachedCamera;
    public Settings settings;

    // Use this for initialization
    void Start ()
    {
        cachedCamera = GetComponent<Camera>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // Create Yoba in selected location
            Vector3 SpawnPos = cachedCamera.ScreenToWorldPoint(Input.mousePosition);
            SpawnPos.z = 0.0f;

            // Don't create if we hit existed Yoba
            if (!Physics2D.OverlapCircle(SpawnPos, 1.0f))
            {
                GameObject AnotherYoba = GameObject.Instantiate(settings.Yoba, SpawnPos, Quaternion.identity);
                AnotherYoba.layer = 8 + Mathf.FloorToInt (Random.Range(0.0f, 2.1f));
                AnotherYoba.transform.localScale = new Vector3(0.05f, 0.05f, 1.0f);

                Yoba yobaSoul = AnotherYoba.GetComponent<Yoba>();
                if (yobaSoul != null)
                {
                    float TheScaleOfYoba = Mathf.Lerp(settings.MinYobaSize, settings.MaxYobaSize, Random.value);
                    yobaSoul.ScaleSmooth(new Vector3(TheScaleOfYoba, TheScaleOfYoba, 1.0f));
                }
                else
                {
                    // missing component
                    Debug.LogError("YOBA BEZ DUSHI, ALLO!");
                }
            }
        }
    }
}
