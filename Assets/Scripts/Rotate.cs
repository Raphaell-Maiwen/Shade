using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    bool isRotating = false;
    bool isResetting = false;

    Quaternion originalRotation;

    [SerializeField]
    private float rotatingTime = 1f;

    public enum RotationDirection {
        xPositive,
        xNegative,
        yPositive,
        yNegative,
        zPositive,
        zNegative
    }

    public RotationDirection[] rotationCycle;
    int rotationIndex = 0;

    void Awake() {
        originalRotation = transform.rotation;
    }

    public void RotateClockwise() {
        if (!isRotating) {
            isRotating = true;
            StartCoroutine(Rotation(transform, GetVector(), rotatingTime));
        }
    }

    public void RotateAntiClockwise() {
        if (!isRotating){
            isRotating = true;
            StartCoroutine(Rotation(transform, GetVector(), rotatingTime));
        }
    }

    Quaternion GetVector() {
        Quaternion vector = Quaternion.identity;
        //For gyms/ objects where the rotation is not implemented
        if (rotationCycle.Length == 0) {
            return vector;
        }

        if (rotationIndex == rotationCycle.Length) {
            rotationIndex = 0;
            print("Initial position");
            return originalRotation;
        }

        switch (rotationCycle[rotationIndex]) {
            case RotationDirection.xPositive:
                vector = Quaternion.Euler(new Vector3(90, 0, 0));
                break;

            case RotationDirection.xNegative:
                vector = Quaternion.Euler(new Vector3(-90, 0, 0));
                break;


            case RotationDirection.yPositive:
                vector = Quaternion.Euler(new Vector3(0, 90, 0));
                break;

            case RotationDirection.yNegative:
                vector = Quaternion.Euler(new Vector3(0, 90, 0));
                break;

            case RotationDirection.zPositive:
                vector = Quaternion.Euler(new Vector3(0, 0, 90));
                break;

            case RotationDirection.zNegative:
                vector = Quaternion.Euler(new Vector3(0, 0, -90));
                break;
        }

        rotationIndex++;
        return vector;
    }

    public IEnumerator Rotation(Transform thisTransform, Quaternion degrees, float time)
    {
        Quaternion startRotation = thisTransform.rotation;
        Quaternion endRotation;
        if (rotationIndex == 0) {
            endRotation = degrees;
        }
        else {
            endRotation = thisTransform.rotation * degrees;
        }
        
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
