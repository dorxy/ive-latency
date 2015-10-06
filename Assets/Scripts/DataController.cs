using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class DataController : MonoBehaviour {

	private GUIStyle labelStyle;
	private CubeController cubeController;
	private RayController rayController;
	private StreamWriter logFile;

	// Use this for initialization
	void Start () {
		labelStyle = new GUIStyle();
		labelStyle.alignment = TextAnchor.UpperRight;
		labelStyle.fontSize = Screen.height / 25;
		labelStyle.normal.textColor = new Color (1.0f, 1.0f, 1.0f, 1.0f); //white

		cubeController = (CubeController) FindObjectOfType (typeof(CubeController));
		rayController = (RayController) FindObjectOfType (typeof(RayController));

		Directory.CreateDirectory ("Data");
		createNewLogFile();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//show any updates on the GUI
	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;
		Rect rect = new Rect(0, 0, w, Screen.height / 25);

		//distance of spheres
		String spheres = (cubeController.cameraDistance == 0 ? "var" : "fixed") + " S\n";
		//latency of the ray
		String latency = rayController.getLatency ().ToString() + " L\n";
		//threshold of the ray
		String threshold = rayController.inRayThreshold.ToString () + " T\n";
		//distance of the intersect
		String distance = rayController.distanceThreshold.ToString () + " D\n";

		GUI.Label(rect, spheres + latency + threshold + distance, labelStyle);
	}

	void OnApplicationQuit(){
		logFile.Close ();
	}

	static Int32 getTimeStamp(){
		return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
	}

	void createNewLogFile(){
		if(logFile != null){
			logFile.Close ();
		}
		logFile = new StreamWriter("Data/"+ getTimeStamp().ToString() +".txt");
		logFile.AutoFlush = true;
		logFile.Write("Latency: " + rayController.getLatency().ToString() + ", ");
		logFile.Write ("Spheres: " + (cubeController.cameraDistance == 0 ? "variable" : "fixed") + ", ");
		logFile.Write("Intersect duration: " + rayController.inRayThreshold.ToString() + ", ");
		logFile.WriteLine("Intersect distance: " + rayController.distanceThreshold.ToString());
	}

	public void writeSphereToLog(GameObject sphere){
		logFile.WriteLine ("Touched sphere");

	}


}
