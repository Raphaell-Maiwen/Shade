using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public bool canBeHeld;
    public bool isASurface;
    public bool canBeStacked;

    [HideInInspector]
    public bool hasAnObjectOn = false;

    void Awake() {
        if (this.GetComponent<ParticleSystem>()) {
            GetComponent<ParticleSystem>().Clear();
            GetComponent<ParticleSystem>().Stop();
        }
    }
}