
using UnityEngine;
using System.Collections.Generic;


// 非闭合线
class LineInfo
{
	public List<Vector3> pointList = new List<Vector3>();
	public Color color = Color.cyan;

	public LineInfo(Vector3[] points, Color color)
	{
		this.color = color;
		for (int i = 0; i < points.Length; ++i)
		{
			pointList.Add(points[i] + new Vector3(0, 0.01f, 0));
		}
	}
}


// 圆周线
class CircleInfo
{
	public float radius = 10f;
	public List<Vector3> pointList = new List<Vector3>();
	public Color color = Color.cyan;

	public CircleInfo(float r, Color color)
	{
		this.radius = r;
		this.color = color;
	}
}


public class DebugLine : MonoBehaviour
{

	private List<LineInfo> lineList = new List<LineInfo>();

	private List<CircleInfo> circleList = new List<CircleInfo>();


	public void AddLine(Vector3[] points, Color color)
	{
		var info = new LineInfo(points, color);
		lineList.Add(info);
	}


	public void ClearLines()
	{
		lineList.Clear();
	}


	public void AddCircle(float radius, Color color)
	{
		var info = new CircleInfo(radius, color);
		CalculatePoints(info);
		circleList.Add(info);
	}


	public void ClearCircles()
	{
		circleList.Clear();
	}


	private void CalculatePoints(CircleInfo info)
	{
		float radius = info.radius;
		List<Vector3> points = info.pointList;

		int pointCount = 10 + (int)(radius * 3);
		float angle = 360f / pointCount;

		Quaternion r = transform.rotation;
		for (int i = 0; i < pointCount; i++)
		{
			Quaternion q = Quaternion.Euler(r.eulerAngles.x, r.eulerAngles.y - (angle * i), r.eulerAngles.z);
			Vector3 v = transform.position + (q * Vector3.forward) * radius;
			v.Set(v.x, 0.1f, v.z);
			points.Add(v);
		}

	}


	void Update()
	{
		// draw lines.
		for (int c = 0; c < lineList.Count; c++)
		{
			var info = lineList[c];
			for (int i = 0; i < info.pointList.Count-1; i++)
			{
				Debug.DrawLine(info.pointList[i], info.pointList[i + 1], info.color);
			}
		}


		// draw circles.
		Vector3 addPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

		for (int c = 0; c < circleList.Count; c++)
		{
			var info = circleList[c];
			for (int i = 0; i < info.pointList.Count; i++)
			{
				if (i != info.pointList.Count - 1)
					Debug.DrawLine(addPos + info.pointList[i], addPos + info.pointList[i + 1], info.color);
				else
					Debug.DrawLine(addPos + info.pointList[i], addPos + info.pointList[0], info.color);
			}
		}
	}



}




