using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteMove : MonoBehaviour {
    public float speed = 1;
    int length = 50;
    float right;
    float left;
    float up;
    float down;
    Vector3 target ;
    private void Start()
    {
        target = transform.position;
        Vector3 self = transform.position;
        right = self.x + length;
        left = self.x - length;
        up = self.y + length;
        down =  self.y - length;
    }
    void Update ()
    {
        if (Vector3.Distance(target,transform.position)<0.1f)
        {
            target = new Vector3(Random.Range(left, right), Random.Range(down, up), transform.position.z);
        }
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * speed);
	}
}
