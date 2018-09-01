using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour {
    public float time = 2f;
    private void Start()
    {
        Destroy(gameObject, time);
    }
}
