using UnityEngine;
using System.Collections;
public class PEPTestDataInput : MonoBehaviour
{

	// Used to make it simpler to know the status
	public enum BreathStatus 
	{
		Ready = 0,
		Inhale = 1,
		Exhale = 2,
		Rest = 3,
		Finished = 4
	}

	public GUIText bellabroadcast_parameter_text;
	public GUIText bellabroadcast_output_text;

	public float BreathStrength { get { return fltPEPData; } }
	public string Platform { get {return platform; } }
	public int Breathing { get { return breathing; } }
	public int Repeat { get { return repeat; } }
	public BreathStatus Status { get { return (BreathStatus)status; } }
	public int PressureMinimum { get { return pressureMinimum; } }
	public int PressureMaximum { get { return pressureMaximum; } }
	public int InhaleMinimum { get { return inhaleMinimum; } }
	public int InhaleMaximum { get { return inhaleMaximum; } }
	public int ExhaleMinimum { get { return exhaleMinimum; } }
	public int ExhaleMaximum { get { return exhaleMaximum; } }
	public int RestMinimum { get { return restMinimum; } }
	public int RestMaximum { get { return restMaximum; } }
		
	//**** define variable
	private AndroidJavaClass _ActivityClass;
	private AndroidJavaObject _ActivityObject;
	private AndroidJavaClass _MyActivityClass; 
	private AndroidJavaObject _MyActivityObject;
	private PEPDataGenerator pepTestData;

	private float fltPEPData;
	private int status;
	private int breathCount;
	private int repeatcount;
	
	private int breathing;
	private int repeat;
	private int pressureMinimum;
	private int pressureMaximum;
	private int inhaleMinimum;
	private int inhaleMaximum;
	private int exhaleMinimum;
	private int exhaleMaximum;
	private int restMinimum;
	private int restMaximum;

	private int maxLength = 160;
	private float nextUpdate;
	private const float delay = 0.4f;//in seconds delay.
	private float xspace = 0.005f;
	private string platform;

	private Vector2[] linePoints;

	void Awake()
	{
		GameObject mainCameraObj = GameObject.Find("Main Camera");

		linePoints = mainCameraObj.GetComponent<CameraDebugComponent>().LinePoints;
	}

	void Start()
	{
		#if UNITY_EDITOR
		initUnity3DEditorPEPDataGenerator();
		#elif UNITY_ANDROID
		initAndroidPlugin();
		#endif


		// common init
		showParameter ();

//		linePoints = new Vector2[maxLength];
//		float x = c0x;
//		for (int i = 0; i < maxLength; i++) 
//		{
//			linePoints[i] = new Vector2(x, c0y);
//			x = x + xspace;
//		}

		nextUpdate = Time.time + delay;
	}

	private void initUnity3DEditorPEPDataGenerator()
	{
		pepTestData = new PEPDataGenerator ();
		platform = "Unity3D Editor";
		//set parameters
		status = pepTestData.Status;
		breathCount = pepTestData.BreathCount;
		repeatcount = pepTestData.RepeatCount;

		breathing = pepTestData.Breathing;
		repeat = pepTestData.Repeat;
		pressureMinimum = pepTestData.MinPressure;
		pressureMaximum = pepTestData.MaxPressure;
		inhaleMinimum = pepTestData.MinInhale;
		inhaleMaximum = pepTestData.MaxInhale;
		exhaleMinimum = pepTestData.MinExhale;
		exhaleMaximum = pepTestData.MaxExhale;
		restMinimum = pepTestData.MinRest;
		restMaximum = pepTestData.MaxRest;
	}

	private void initAndroidPlugin()
	{
		platform = "Android Device";
		_ActivityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		_ActivityObject = _ActivityClass.GetStatic<AndroidJavaObject>("currentActivity");
		_MyActivityObject = new AndroidJavaObject("com.spotsolutions.bella.plugin.BellaBroadcastReceiver",
		                                          _ActivityObject);
		_MyActivityObject.Call ("registerService");

		breathing = int.Parse(_MyActivityObject.Get<string> ("Breathing"));
		repeat = int.Parse(_MyActivityObject.Get<string> ("Repeat"));
		pressureMinimum = int.Parse(_MyActivityObject.Get<string> ("MinPressure"));
		pressureMaximum = int.Parse(_MyActivityObject.Get<string> ("MaxPressure"));
		inhaleMinimum = int.Parse(_MyActivityObject.Get<string> ("MinInhale"));
		inhaleMaximum = int.Parse(_MyActivityObject.Get<string> ("MaxInhale"));
		exhaleMinimum = int.Parse(_MyActivityObject.Get<string> ("MinExhale"));
		exhaleMaximum = int.Parse(_MyActivityObject.Get<string> ("MaxExhale"));
		restMinimum = int.Parse(_MyActivityObject.Get<string> ("MinRest"));
		restMaximum = int.Parse(_MyActivityObject.Get<string> ("MaxRest"));

	}

	private void showParameter()
	{
		bellabroadcast_parameter_text.text = platform
				+ ":   " + breathing.ToString()
				+ " : " + repeat.ToString()
				+ " : " + pressureMinimum.ToString()
				+ " : " + pressureMaximum.ToString()
				+ " : " + inhaleMinimum.ToString()
				+ " : " + inhaleMaximum.ToString()
				+ " : " + exhaleMinimum.ToString()
				+ " : " + exhaleMaximum.ToString()
				+ " : " + restMinimum.ToString()
				+ " : " + restMaximum.ToString();
	}

	private void getPEPData()
	{
		#if UNITY_EDITOR
		fltPEPData = pepTestData.GetNextValue();
		status = pepTestData.Status;
		breathCount = pepTestData.BreathCount;
		repeatcount = pepTestData.RepeatCount;
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
		if (status == 0)
			strStatus = "ready";
		else if (status == 1)
			strStatus = "inhale";
		else if (status == 2)
			strStatus = "exhale";
		else if (status == 3)
			strStatus = "rest";
		else if (status == 4)
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
					+ " : breathing: " + breathCount.ToString()
					+ " : repeat: " + repeatcount.ToString();


			float pepvalue = fltPEPData / 100;
			if( pepvalue > 0.1f)
				pepvalue = pepvalue	+ 0.4f;
			else
				pepvalue = pepvalue	+ 0.2f;

			if (linePoints.Length < maxLength - 1) 
			{
				linePoints[linePoints.Length].y = pepvalue;
			} 
			else 
			{
				for (int i=0; i < maxLength - 1; i++) 
				{
					linePoints[i].y = linePoints [i + 1].y;
				}
				linePoints [maxLength - 1].y = pepvalue;
			}
		}
//		linePoints[0] = new Vector2(0.1f, 0.1f);
//		linePoints[1] = new Vector2(Input.mousePosition.x/Screen.width, Input.mousePosition.y/Screen.height);
//
//		linePoints[1] = new Vector2(0.9f, 0.9f);

	}

	//*********************************************************************************
//	public int numberOfPoints = 2;
//	public Color lineColor = Color.yellow;
//	public int lineWidth = 1;
//	public bool drawLines = true;
//	
//	private Material lineMaterial;
//	private Vector2[] linePoints;
//	public Camera cam;
//	private float c0x = 0.1f, c0y = 0.2f;


	

}