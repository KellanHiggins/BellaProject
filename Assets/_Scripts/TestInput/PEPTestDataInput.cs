using UnityEngine;
using System.Collections;
public class PEPTestDataInput : MonoBehaviour
{
	public GUIText bellabroadcast_parameter_text;
	public GUIText bellabroadcast_output_text;

	//**** define variable
	private AndroidJavaClass _ActivityClass;
	private AndroidJavaObject _ActivityObject;
	private AndroidJavaClass _MyActivityClass; 
	private AndroidJavaObject _MyActivityObject;
	private PEPDataGenerator pepTestData;

	private float fltPEPData;
	private int intStatus;
	private int intBreathcount;
	private int intRepeatcount;
	
	private int intBreathing;
	private int intRepeat;
	private int intMinPressure;
	private int intMaxPressure;
	private int intMinInhale;
	private int intMaxInhale;
	private int intMinExhale;
	private int intMaxExhale;
	private int intMinRest;
	private int intMaxRest;


	private int maxLength = 160;
	private float nextUpdate;
	private const float delay = 0.4f;//in seconds delay.
	private float xspace = 0.005f;
	private string Platform;
	void Start()
	{
		#if UNITY_EDITOR
		initUnity3DEditorPEPDataGenerator();
		#elif UNITY_ANDROID
		initAndroidPlugin();
		#endif


		// common init
		showParameter ();

		linePoints = new Vector2[maxLength];
		float x = c0x;
		for (int i = 0; i < maxLength; i++) 
		{
			linePoints[i] = new Vector2(x, c0y);
			x = x + xspace;
		}

		nextUpdate = Time.time + delay;
	}

	private void initUnity3DEditorPEPDataGenerator()
	{
		pepTestData = new PEPDataGenerator ();
		Platform = "Unity3D Editor";
		//set parameters
		intStatus = pepTestData.Status;
		intBreathcount = pepTestData.BreathCount;
		intRepeatcount = pepTestData.RepeatCount;

		intBreathing = pepTestData.Breathing;
		intRepeat = pepTestData.Repeat;
		intMinPressure = pepTestData.MinPressure;
		intMaxPressure = pepTestData.MaxPressure;
		intMinInhale = pepTestData.MinInhale;
		intMaxInhale = pepTestData.MaxInhale;
		intMinExhale = pepTestData.MinExhale;
		intMaxExhale = pepTestData.MaxExhale;
		intMinRest = pepTestData.MinRest;
		intMaxRest = pepTestData.MaxRest;
	}

	private void initAndroidPlugin()
	{
		Platform = "Android Device";
		_ActivityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		_ActivityObject = _ActivityClass.GetStatic<AndroidJavaObject>("currentActivity");
		_MyActivityObject = new AndroidJavaObject("com.spotsolutions.bella.plugin.BellaBroadcastReceiver",
		                                          _ActivityObject);
		_MyActivityObject.Call ("registerService");

		intBreathing = int.Parse(_MyActivityObject.Get<string> ("Breathing"));
		intRepeat = int.Parse(_MyActivityObject.Get<string> ("Repeat"));
		intMinPressure = int.Parse(_MyActivityObject.Get<string> ("MinPressure"));
		intMaxPressure = int.Parse(_MyActivityObject.Get<string> ("MaxPressure"));
		intMinInhale = int.Parse(_MyActivityObject.Get<string> ("MinInhale"));
		intMaxInhale = int.Parse(_MyActivityObject.Get<string> ("MaxInhale"));
		intMinExhale = int.Parse(_MyActivityObject.Get<string> ("MinExhale"));
		intMaxExhale = int.Parse(_MyActivityObject.Get<string> ("MaxExhale"));
		intMinRest = int.Parse(_MyActivityObject.Get<string> ("MinRest"));
		intMaxRest = int.Parse(_MyActivityObject.Get<string> ("MaxRest"));

	}

	private void showParameter()
	{
		bellabroadcast_parameter_text.text = Platform
				+ ":   " + intBreathing.ToString()
				+ " : " + intRepeat.ToString()
				+ " : " + intMinPressure.ToString()
				+ " : " + intMaxPressure.ToString()
				+ " : " + intMinInhale.ToString()
				+ " : " + intMaxInhale.ToString()
				+ " : " + intMinExhale.ToString()
				+ " : " + intMaxExhale.ToString()
				+ " : " + intMinRest.ToString()
				+ " : " + intMaxRest.ToString();
	}

	private void getPEPData()
	{
		#if UNITY_EDITOR
		fltPEPData = pepTestData.GetNextValue();
		intStatus = pepTestData.Status;
		intBreathcount = pepTestData.BreathCount;
		intRepeatcount = pepTestData.RepeatCount;
		#elif UNITY_ANDROID
		fltPEPData = float.Parse(_MyActivityObject.Get<string> ("PEPData"));
		intStatus = int.Parse(_MyActivityObject.Get<string> ("Status"));
		intBreathcount = int.Parse(_MyActivityObject.Get<string> ("BreathCount"));
		intRepeatcount =int.Parse(_MyActivityObject.Get<string> ("RepeatCount"));
		#endif
	}

	private string getStatusString()
	{
		string strStatus = "";
		if (intStatus == 0)
			strStatus = "ready";
		else if (intStatus == 1)
			strStatus = "inhale";
		else if (intStatus == 2)
			strStatus = "exhale";
		else if (intStatus == 3)
			strStatus = "rest";
		else if (intStatus == 4)
			strStatus = "finish";
		return strStatus;
	}

	void Update()
	{          
		if (Time.time > nextUpdate) 
		{
			nextUpdate = Time.time + delay;

			// We get the text property of our receiver
			getPEPData();
			bellabroadcast_output_text.text = "PEP Data : " 
					+ fltPEPData.ToString("0.00") 
					+ " : " + getStatusString()
					+ " : breathing: " + intBreathcount.ToString()
					+ " : repeat: " + intRepeatcount.ToString();


			float pepvalue = fltPEPData / 100;
			if( pepvalue > 0.1f)
				pepvalue = pepvalue	+ 0.4f;
			else
				pepvalue = pepvalue	+ 0.2f;

			if (linePoints.Length < maxLength - 1) 
			{
				linePoints [linePoints.Length].y = pepvalue;
			} else 
			{
				for (int i=0; i < maxLength - 1; i++) 
				{
					linePoints [i].y = linePoints [i + 1].y;
				}
				linePoints [maxLength - 1].y = pepvalue;
			}
		}
		//linePoints[0] = new Vector2(0.1f, 0.1f);
		//linePoints[1] = new Vector2(Input.mousePosition.x/Screen.width, Input.mousePosition.y/Screen.height);

		//linePoints[1] = new Vector2(0.9f, 0.9f);

	}

	//*********************************************************************************
	//public int numberOfPoints = 2;
	//public Color lineColor = Color.yellow;
	public int lineWidth = 1;
	public bool drawLines = true;
	
	private Material lineMaterial;
	private Vector2[] linePoints;
	private Camera cam;
	private float c0x = 0.1f, c0y = 0.2f;

	void Awake()
	{
		lineMaterial = new Material( "Shader \"Lines/Colored Blended\" {" +
		                            "SubShader { Pass {" +
		                            "   BindChannels { Bind \"Color\",color }" +
		                            "   Blend SrcAlpha OneMinusSrcAlpha" +
		                            "   ZWrite Off Cull Off Fog { Mode Off }" +
		                            "} } }");
		lineMaterial.hideFlags = HideFlags.HideAndDontSave;
		lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
		cam = camera;
	}

/*
	void OnGUI() {
		int x, y;
		x = (int)(Screen.width * c0x);
		y = (int)(Screen.height * 0.9);
		GUI.Label(new Rect(x, y, 100, 20), "cm");

		x = (int)(Screen.width * c0x);
		y = (int)(Screen.height * 0.9);
		GUI.Label(new Rect(40, 232, 100, 20), "sec.");

		GUI.Label(new Rect(10, 10, 100, 20), y.ToString());
	}
*/
	void OnPostRender()
	{
		
		if (!drawLines)
			return;

		Line (linePoints[0], linePoints[1], Color.yellow);

		//*****************************************************************************
		//**** Coordinate
		Vector2 v1 = new Vector2 (c0x, c0y);
		Vector2 v2 = new Vector2(0.9f, c0y);
		Line (v1, v2, Color.white);
		v1 = new Vector2(c0x, c0y);
		v2 = new Vector2(c0x, 0.9f);
		Line (v1, v2, Color.white);

		// lowbond
		v1 = new Vector2(c0x, 0.5f);
		v2 = new Vector2(0.9f, 0.5f);
		Line (v1, v2, Color.yellow);
		// upbond
		v1 = new Vector2(c0x, 0.6f);
		v2 = new Vector2(0.9f, 0.6f);
		Line (v1, v2, Color.yellow);

		for (int i = 0; i < linePoints.Length - 1; i++) 
		{
			Line (linePoints[i], linePoints[i+1], Color.green);
		}

/*
		if (!drawLines || linePoints == null || linePoints.Length < 2)
			return;
		
		float nearClip = cam.nearClipPlane + 0.00001f;
		int end = linePoints.Length - 1;
		float thisWidth = 1f/Screen.width * lineWidth * 0.5f;
		
		lineMaterial.SetPass(0);
		GL.Color(lineColor);
		
		if (lineWidth == 1)
		{
			GL.Color(Color.red);
			GL.Begin(GL.LINES);
			for (int i = 0; i < end; ++i)
			{
				GL.Vertex(cam.ViewportToWorldPoint(new Vector3(linePoints[i].x, linePoints[i].y, nearClip)));
				GL.Vertex(cam.ViewportToWorldPoint(new Vector3(linePoints[i+1].x, linePoints[i+1].y, nearClip)));
			}
		}
		else
		{
			GL.Color(Color.yellow);
			GL.Begin(GL.QUADS);
			for (int i = 0; i < end; ++i)
			{
				Vector3 perpendicular = (new Vector3(linePoints[i+1].y, linePoints[i].x, nearClip) -
				                         new Vector3(linePoints[i].y, linePoints[i+1].x, nearClip)).normalized * thisWidth;
				Vector3 v1 = new Vector3(linePoints[i].x, linePoints[i].y, nearClip);
				Vector3 v2 = new Vector3(linePoints[i+1].x, linePoints[i+1].y, nearClip);
				GL.Vertex(cam.ViewportToWorldPoint(v1 - perpendicular));
				GL.Vertex(cam.ViewportToWorldPoint(v1 + perpendicular));
				GL.Vertex(cam.ViewportToWorldPoint(v2 + perpendicular));
				GL.Vertex(cam.ViewportToWorldPoint(v2 - perpendicular));
			}

		}

		GL.End();
*/		
	}

	void Line(Vector2 p1, Vector2 p2, Color lineColor)
	{
		Vector2[] TwoPoints = new Vector2[2];
		TwoPoints [0] = p1;
		TwoPoints [1] = p2;
		
		float nearClip = cam.nearClipPlane + 0.00001f;
		int end = TwoPoints.Length - 1;
		float thisWidth = 1f/Screen.width * lineWidth * 0.5f;
		lineMaterial.SetPass(0);
		
		GL.Color(lineColor);
		
		if (lineWidth == 1)
		{
			GL.Begin(GL.LINES);
			for (int i = 0; i < end; ++i)
			{
				GL.Vertex(cam.ViewportToWorldPoint(new Vector3(TwoPoints[i].x, TwoPoints[i].y, nearClip)));
				GL.Vertex(cam.ViewportToWorldPoint(new Vector3(TwoPoints[i+1].x, TwoPoints[i+1].y, nearClip)));
			}
		}
		else
		{
			GL.Begin(GL.QUADS);
			for (int i = 0; i < end; ++i)
			{
				Vector3 perpendicular = (new Vector3(TwoPoints[i+1].y, TwoPoints[i].x, nearClip) -
				                         new Vector3(TwoPoints[i].y, TwoPoints[i+1].x, nearClip)).normalized * thisWidth;
				Vector3 v1 = new Vector3(TwoPoints[i].x, TwoPoints[i].y, nearClip);
				Vector3 v2 = new Vector3(TwoPoints[i+1].x, TwoPoints[i+1].y, nearClip);
				GL.Vertex(cam.ViewportToWorldPoint(v1 - perpendicular));
				GL.Vertex(cam.ViewportToWorldPoint(v1 + perpendicular));
				GL.Vertex(cam.ViewportToWorldPoint(v2 + perpendicular));
				GL.Vertex(cam.ViewportToWorldPoint(v2 - perpendicular));
			}
			
		}
		
		GL.End();
	}

	void OnApplicationQuit()
	{
		DestroyImmediate(lineMaterial);
	}



}
