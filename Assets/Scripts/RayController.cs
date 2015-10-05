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

	public GameObject inRay = null;
	public int inRayNumberOfFrames;

	public int inRayThreshold;

	public int distanceThreshold;
	
	// Use this for initialization
	void Start () {

		this.width = 0.3f;
		this.distance = 100.0f;
		this.heightOffset = 0.5f;
		this.inRayThreshold = 60;
		this.inRayNumberOfFrames = 0;
		this.distanceThreshold = 12;

		// Init the ray
		Vector3 pos = this.camera.transform.position;
		Vector3 dir = this.camera.transform.forward * distance + pos;
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
		Vector3 dir = this.camera.transform.forward * this.distance + pos;
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

		this.ray.transform.up = offset;

		// Check all the positions of the 
		CubeController cc = UnityEngine.Object.FindObjectOfType<CubeController>();

		GameObject toRemove = null;
		GameObject inRayTemp = null;

		foreach (GameObject sphere in cc.spheres) {
			Vector3 position = sphere.transform.position;
			float dist = Vector3.Cross(offset, position - pos).magnitude;
			if(dist < distanceThreshold){ // Distance chosen by fair diceroll with 3 dice

				inRayTemp = sphere;
			}
		}
		
		if(inRayTemp != null)
			inRayTemp.GetComponent<Renderer>().material.color = Color.red;
		else if (inRayTemp != inRay && inRay != null)
			inRay.GetComponent<Renderer>().material.color = Color.green;

		if (inRayTemp == inRay && inRayTemp != null) {
			inRayNumberOfFrames ++;
			if (inRayNumberOfFrames >= this.inRayThreshold) {
				cc.spheres.Remove (inRay);
				Destroy (inRay);
				cc.generateNewCube ();
			}
		} else {
			inRay = inRayTemp;
			inRayNumberOfFrames = 0;
		}
	}

	void OnGUI() {
		Event e = Event.current;

		if (e.isKey && e.type == EventType.keyDown){
			int dir = e.shift?-1:1;

			switch(e.keyCode){
				case KeyCode.D:
					this.distanceThreshold = Math.Max(0, distanceThreshold + dir );
					break;
				case KeyCode.L:
					this.latencySize = Math.Max(0, latencySize + dir );
					break;
				case KeyCode.T:
					this.inRayThreshold = Math.Max(0, inRayThreshold + dir );
					break;
			}

			Debug.Log ("latencySize is now: " + latencySize);
			Debug.Log ("inRayThreshold is now " + inRayThreshold);
		}

		// Uncomment the next line for drawing a crosshair on the creen, wich can be used to see the center,
		// note that the beam actually does _NOT_ look at the center
		// GUI.Box(new Rect(Screen.width/2,Screen.height/2, 10, 10), "");
	}

}
