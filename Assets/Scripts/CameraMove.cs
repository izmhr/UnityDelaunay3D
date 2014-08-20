using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {

	public float r = 4.0f;
	public float rad = 0.4f;
	public float height = 3.0f;
	public Vector3 lookAt = new Vector3(0, 0.5f, 0);
	float angle = 0.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		angle += rad * Time.deltaTime;
		transform.position = new Vector3( r * Mathf.Cos (angle), height, r * Mathf.Sin(angle) );
		transform.LookAt(lookAt);
	}
}
