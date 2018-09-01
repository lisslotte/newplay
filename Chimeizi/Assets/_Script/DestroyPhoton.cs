using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPhoton : MonoBehaviour {

    public float time = 2f;
    private void Start()
    {
        Invoke("Des", time);
    }
    private void Des()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
