using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    ShadowProjection[] projectionScripts;
    bool isRotating = false;

    [SerializeField]
    private float rotatingTime = 1f;

    private void Awake() {
        projectionScripts = (ShadowProjection[])FindObjectsOfType(typeof(ShadowProjection));
        transform.hasChanged = false;
    }

    void Update() {
        if (transform.hasChanged) {
            foreach (ShadowProjection script in projectionScripts) {
                print("It's updating");
                script.UpdateShadow(this.gameObject);
                transform.hasChanged = false;
            }
        }
    }

    public void RotateClockwise() {
        if (!isRotating) {
            isRotating = true;
            StartCoroutine(Rotation(transform, new Vector3(0, 90, 0), rotatingTime));
        }
    }

    public void RotateAntiClockwise() {
        if (!isRotating){
            isRotating = true;
            StartCoroutine(Rotation(transform, new Vector3(0, -90, 0), rotatingTime));
        }
    }

    public IEnumerator Rotation(Transform thisTransform, Vector3 degrees, float time)
    {
        Quaternion startRotation = thisTransform.rotation;
        Quaternion endRotation = thisTransform.rotation * Quaternion.Euler(degrees);
        float rate = 1.0f / time;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            thisTransform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }
        isRotating = false;
    }
}
