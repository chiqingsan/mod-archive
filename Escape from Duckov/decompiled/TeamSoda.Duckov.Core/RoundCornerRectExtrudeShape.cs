using System.Collections.Generic;
using UnityEngine;

public class RoundCornerRectExtrudeShape : ShapeProvider
{
	public Vector2 size = Vector2.one;

	public float bevelSize = 0.1f;

	public bool resample;

	[Range(0.1f, 1f)]
	public float stepLength = 0.25f;

	public override PipeRenderer.OrientedPoint[] GenerateShape()
	{
		float num = bevelSize;
		Vector2 vector = new Vector2((0f - size.x) / 2f + num, size.y / 2f - num);
		Vector2 vector2 = vector + new Vector2(-1f, 1f).normalized * num;
		Vector2 vector3 = vector + new Vector2(0f, 1f) * num;
		PipeRenderer.OrientedPoint orientedPoint = default(PipeRenderer.OrientedPoint);
		orientedPoint.position = vector2;
		orientedPoint.normal = new Vector2(-1f, 1f);
		orientedPoint.uv = Vector2.zero;
		PipeRenderer.OrientedPoint orientedPoint2 = orientedPoint;
		orientedPoint = default(PipeRenderer.OrientedPoint);
		orientedPoint.position = vector3;
		orientedPoint.normal = new Vector3(0f, 1f);
		orientedPoint.uv = num * Vector2.one;
		PipeRenderer.OrientedPoint orientedPoint3 = orientedPoint;
		PipeRenderer.OrientedPoint orientedPoint4 = orientedPoint3;
		orientedPoint4.position.x = 0f - orientedPoint4.position.x;
		orientedPoint4.normal.x = 0f - orientedPoint4.normal.x;
		orientedPoint4.uv = Vector2.one - orientedPoint4.uv;
		PipeRenderer.OrientedPoint orientedPoint5 = orientedPoint2;
		orientedPoint5.position.x = 0f - orientedPoint5.position.x;
		orientedPoint5.normal.x = 0f - orientedPoint5.normal.x;
		orientedPoint5.uv = Vector2.one;
		PipeRenderer.OrientedPoint item = orientedPoint5;
		item.uv = Vector2.zero;
		PipeRenderer.OrientedPoint orientedPoint6 = default(PipeRenderer.OrientedPoint);
		orientedPoint6.position = vector;
		orientedPoint6.position.x = 0f - orientedPoint6.position.x;
		orientedPoint6.position.x += num;
		orientedPoint6.normal = new Vector3(1f, 0f);
		orientedPoint6.uv = orientedPoint3.uv;
		PipeRenderer.OrientedPoint orientedPoint7 = orientedPoint6;
		orientedPoint7.position.y = 0f - orientedPoint7.position.y;
		orientedPoint7.uv = orientedPoint4.uv;
		PipeRenderer.OrientedPoint orientedPoint8 = orientedPoint5;
		orientedPoint8.position.y = 0f - orientedPoint8.position.y;
		orientedPoint8.normal = new Vector2(1f, -1f);
		orientedPoint8.uv = Vector2.one;
		PipeRenderer.OrientedPoint item2 = orientedPoint8;
		item2.uv = Vector2.zero;
		PipeRenderer.OrientedPoint item3 = orientedPoint4;
		item3.position.y = 0f - item3.position.y;
		item3.normal = Vector2.down;
		item3.uv = orientedPoint6.uv;
		PipeRenderer.OrientedPoint item4 = orientedPoint3;
		item4.position.y = 0f - item4.position.y;
		item4.normal = Vector2.down;
		item4.uv = orientedPoint4.uv;
		PipeRenderer.OrientedPoint orientedPoint9 = orientedPoint2;
		orientedPoint9.position.y = 0f - orientedPoint9.position.y;
		orientedPoint9.normal = new Vector2(-1f, -1f);
		orientedPoint9.uv = Vector2.one;
		PipeRenderer.OrientedPoint item5 = orientedPoint9;
		item5.uv = Vector2.zero;
		PipeRenderer.OrientedPoint item6 = orientedPoint7;
		item6.position.x = 0f - item6.position.x;
		item6.normal = Vector2.left;
		item6.uv = orientedPoint3.uv;
		PipeRenderer.OrientedPoint orientedPoint10 = orientedPoint6;
		orientedPoint10.position.x = 0f - orientedPoint10.position.x;
		orientedPoint10.normal = Vector2.left;
		orientedPoint10.uv = orientedPoint4.uv;
		PipeRenderer.OrientedPoint item7 = orientedPoint10;
		item7.uv = Vector2.zero;
		List<PipeRenderer.OrientedPoint> list = new List<PipeRenderer.OrientedPoint>();
		list.Add(orientedPoint2);
		list.Add(orientedPoint3);
		list.Add(orientedPoint4);
		list.Add(orientedPoint5);
		list.Add(item);
		list.Add(orientedPoint6);
		list.Add(orientedPoint7);
		list.Add(orientedPoint8);
		list.Add(item2);
		list.Add(item3);
		list.Add(item4);
		list.Add(orientedPoint9);
		list.Add(item5);
		list.Add(item6);
		list.Add(orientedPoint10);
		list.Add(item7);
		list.Reverse();
		if (resample)
		{
			list = Resample(list, stepLength);
		}
		return list.ToArray();
	}

	private List<PipeRenderer.OrientedPoint> Resample(List<PipeRenderer.OrientedPoint> original, float stepLength)
	{
		if (stepLength < 0.01f)
		{
			return new List<PipeRenderer.OrientedPoint>();
		}
		List<PipeRenderer.OrientedPoint> list = new List<PipeRenderer.OrientedPoint>();
		int i = 0;
		float num = 0f;
		for (; i < original.Count; i++)
		{
			PipeRenderer.OrientedPoint orientedPoint = original[i];
			PipeRenderer.OrientedPoint orientedPoint2 = ((i + 1 >= original.Count) ? original[0] : original[i + 1]);
			Vector3 vector = orientedPoint2.position - orientedPoint.position;
			Vector3 normalized = vector.normalized;
			float magnitude = vector.magnitude;
			for (float num2 = 0f; num2 < magnitude; num2 += stepLength)
			{
				Vector3 position = orientedPoint.position + normalized * num2;
				float t = num2 / magnitude;
				num += num2;
				PipeRenderer.OrientedPoint orientedPoint3 = default(PipeRenderer.OrientedPoint);
				orientedPoint3.position = position;
				orientedPoint3.normal = Vector3.Lerp(orientedPoint.normal, orientedPoint2.normal, t);
				orientedPoint3.uv = num * Vector2.one;
				PipeRenderer.OrientedPoint item = orientedPoint3;
				list.Add(item);
			}
		}
		return list;
	}
}
