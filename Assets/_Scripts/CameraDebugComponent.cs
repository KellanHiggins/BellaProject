using UnityEngine;
using System.Collections;

public class CameraDebugComponent : MonoBehaviour 
{

	private int maxLength = 160;
	//private float nextUpdate;
	private const float delay = 0.4f;//in seconds delay.
	private float xspace = 0.005f;
	private string platform;

	public int numberOfPoints = 2;
	public Color lineColor = Color.yellow;
	public int lineWidth = 1;
	public bool drawLines = true;
	
	private Material lineMaterial;
	public Vector2[] LinePoints;
	private Camera cam;
	private float c0x = 0.1f, c0y = 0.2f;

	void Awake()
	{
		LinePoints = new Vector2[maxLength];
		float x = c0x;
		for (int i = 0; i < maxLength; i++) 
		{
			LinePoints[i] = new Vector2(x, c0y);
			x = x + xspace;
		}

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

	void OnPostRender()
	{
		if (!drawLines)
			return;

		Line (LinePoints[0], LinePoints[1], Color.yellow);
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
		for (int i = 0; i < LinePoints.Length - 1; i++)
		{
			Line (LinePoints[i], LinePoints[i+1], Color.green);
		}
	
	}

	private void Line(Vector2 p1, Vector2 p2, Color lineColor)
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

	private void OnApplicationQuit()
	{
		DestroyImmediate(lineMaterial);
	}

}
