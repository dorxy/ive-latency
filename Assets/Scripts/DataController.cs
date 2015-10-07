using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class DataController : MonoBehaviour {

	private GUIStyle labelStyle;
	private SphereController sphereController;
	private RayController rayController;
	private StreamWriter logFile;
	private Int32 timestamp;
	private GameObject lastSphere;
	private Int64 lastTime;
	private int sphereCount;

	// Use this for initialization
	void Start () {
		labelStyle = new GUIStyle();
		labelStyle.alignment = TextAnchor.UpperRight;
		labelStyle.fontSize = Screen.height / 25;
		labelStyle.normal.textColor = new Color (1.0f, 1.0f, 1.0f, 1.0f); //white

		sphereController = (SphereController) FindObjectOfType (typeof(SphereController));
		rayController = (RayController) FindObjectOfType (typeof(RayController));

		sphereCount = 0;
		lastTime = 0;

		Directory.CreateDirectory ("Data");
		createNewLogFile();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//show any updates on the GUI
	void OnGUI()
	{
		Rect rect = new Rect(0, 0, Screen.width, Screen.height / 25);

		//distance of spheres
		String spheres = (sphereController.cameraDistance == 0 ? "var" : "fixed") + " S\n";
		//latency of the ray
		String latency = rayController.getLatency ().ToString() + " L\n";
		//threshold of the ray
		String threshold = rayController.inRayThreshold.ToString () + " T\n";
		//distance of the intersect
		String distance = rayController.distanceThreshold.ToString () + " D\n";

		GUI.Label(rect, timestamp.ToString() + " /  " + sphereCount + "\n" + spheres + latency + threshold + distance, labelStyle);


		Event e = Event.current;
		
		if (e.isKey && e.type == EventType.keyDown) {
			switch (e.keyCode) {
			case KeyCode.N:
				this.createNewLogFile();
				break;
			}
		}
	}

	void OnApplicationQuit(){
		logFile.Close ();
	}

	static Int32 getTimeStamp(){
		return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
	}
	static Int64 getTimeStampWithMilliseconds(){
		return (Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
	}


	void createNewLogFile(){
		if(logFile != null){
			logFile.Close ();
		}
		this.timestamp = getTimeStamp ();
		logFile = new StreamWriter("Data/"+ this.timestamp.ToString() +".txt");
		logFile.AutoFlush = true;
		logFile.Write("Latency: " + rayController.getLatency().ToString() + ", ");
		logFile.Write("Spheres: " + (sphereController.cameraDistance == 0 ? "variable" : "fixed") + ", ");
		logFile.Write("Intersect duration: " + rayController.inRayThreshold.ToString() + ", ");
		logFile.WriteLine("Intersect distance: " + rayController.distanceThreshold);
		logFile.WriteLine ("timestamp, x, y, z, distance, time");
		Destroy (lastSphere);
	}

	public void writeSphereToLog(GameObject sphere){
		Int64 timestamp = getTimeStampWithMilliseconds ();
		logFile.Write(timestamp + ", ");
		logFile.Write (sphere.transform.position.x + ", " + sphere.transform.position.y + ", " + sphere.transform.position.z);

		float distance = 0;
		if (lastSphere != null) {
			distance = Vector3.Distance(lastSphere.transform.position, sphere.transform.position);
		}
		if (lastTime == 0)
			lastTime = timestamp;
		logFile.WriteLine (", " + distance + ", " + (timestamp - lastTime));

		lastTime = timestamp;

		Destroy (lastSphere);
		lastSphere = Instantiate(sphere);
		lastSphere.GetComponent<Renderer> ().enabled = false;

		sphereCount++;
	}


}
