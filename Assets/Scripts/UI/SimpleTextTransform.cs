using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTextTransform : MonoBehaviour {

    public float SpeedX = 0f;
    public float SpeedY = 1f;
    public float SpeedZ = 0f;
	
	void Update () {

        transform.Translate(SpeedX * Time.deltaTime,
            SpeedY * Time.deltaTime,
            SpeedZ * Time.deltaTime);
	}
}
