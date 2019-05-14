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

    [HideInInspector]
    public bool isFalling = false;

    private List<GameObject> children;

    Rigidbody rb;

    void Awake() {
        if (this.GetComponent<ParticleSystem>()) {
            GetComponent<ParticleSystem>().Clear();
            GetComponent<ParticleSystem>().Stop();
        }

        rb = GetComponent<Rigidbody>();

        children = new List<GameObject>();

        foreach (Transform child in transform) {
            if (child.GetComponent<BoxCollider>() && !child.GetComponent<BoxCollider>().isTrigger) {
                children.Add(child.gameObject);
            }
        }
    }

    void OnCollisionEnter(Collision coll) {
        if (isFalling && coll.gameObject.tag != "Player") {
            isFalling = false;
            //rb.constraints = ~RigidbodyConstraints.FreezePositionY;
            rb.isKinematic = true;
            print("tg");
            /*for (int i = 0; i < children.Count; i++) {
                children[i].GetComponent<BoxCollider>().isTrigger = false;
            }*/
        }
    }

    public void isMoving(bool isTrigger) {
        for (int i = 0; i < children.Count; i++) {
            children[i].GetComponent<BoxCollider>().isTrigger = isTrigger;
        }

        GetComponent<Rigidbody>().isKinematic = isTrigger;
        print(isTrigger);

        GetComponent<Rigidbody>().useGravity = !isTrigger;
    }
}