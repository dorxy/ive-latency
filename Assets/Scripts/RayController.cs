using UnityEngine;
using System.Collections.Generic;
using System;

public class RayController : MonoBehaviour {
	
	public Camera camera;
	public float cameraDistance; //0 is random
	public GameObject cubePrefabObject;
	private List<Vector3> OffsetList = new List<Vector3>();
	private int latencySize = 0;
	public GameObject ray;
	public float width;
	public float heightOffset;
	public float distance;
	
	// Use this for initialization
	void Start () {

		this.width = 0.3f;
		this.distance = 100.0f;
		this.heightOffset = 0.5f;

		// Init the ray
		Vector3 pos = this.camera.transform.position;
		Vector3 dir = this.camera.transform.forward * distance;
		Vector3 offsetH = this.camera.transform.up.normalized * heightOffset;
		pos += offsetH;
		dir += offsetH;

		var offset = dir - pos;
		var scale = new Vector3 (this.width, offset.magnitude / 2.0f, this.width);

		this.ray = (GameObject) Instantiate (this.cubePrefabObject, pos, Quaternion.identity);
		this.ray.transform.up = offset;
		this.ray.transform.localScale = scale;
		this.ray.GetComponent<Renderer> ().enabled = true;

	}
	
	// Update is called once per frame
	void Update () {
		
		// Calculate the current offset and scale and add them to the latencylists
		Vector3 pos = this.camera.transform.position;
		Vector3 dir = this.camera.transform.forward * this.distance;
		Vector3 offsetH = this.camera.transform.up.normalized * heightOffset;
		pos += offsetH;
		dir += offsetH;
		
		Vector3 offset = dir - pos;
		this.OffsetList.Add (offset);
		
		if (this.OffsetList.Count <= this.latencySize)
			return;

		// Shift off variables for the current latency shit
		// If this becomes too slow we can change the list to an array with moving pointer
		// but then the latency is bounded to a certain maximum.
		// The odd removal is required for the changing of the latency
		offset = this.OffsetList [0];
		this.OffsetList.RemoveRange (0, this.OffsetList.Count - this.latencySize);
//		this.OffsetList.RemoveAt (0);

		this.ray.transform.up = offset;
	}

	void OnGUI() {
		Event e = Event.current;

		if (e.isKey){
			if(e.keyCode == KeyCode.Equals)
				latencySize++;
			else if(e.keyCode == KeyCode.Minus)
				latencySize = Math.Max(0, latencySize - 1); // Prevent latencySize from becoming negative

			Debug.Log ("latencySize is now: " + latencySize);
		}
	}

}
