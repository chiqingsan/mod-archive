using System.Collections.Generic;
using UnityEngine;

public class RoundCornerSquareExtrudeShape : ShapeProvider
{
	public float size = 1f;

	public float bevelSize = 0.1f;

	public override PipeRenderer.OrientedPoint[] GenerateShape()
	{
		float num = size;
		float num2 = bevelSize;
		Vector2 vector = new Vector2((0f - num) / 2f + num2, num / 2f - num2);
		Vector2 vector2 = vector + new Vector2(-1f, 1f).normalized * num2;
		Vector2 vector3 = vector + new Vector2(0f, 1f) * num2;
		PipeRenderer.OrientedPoint orientedPoint = default(PipeRenderer.OrientedPoint);
		orientedPoint.position = vector2;
		orientedPoint.normal = new Vector2(-1f, 1f);
		orientedPoint.uv = Vector2.zero;
		PipeRenderer.OrientedPoint orientedPoint2 = orientedPoint;
		orientedPoint = default(PipeRenderer.OrientedPoint);
		orientedPoint.position = vector3;
		orientedPoint.normal = new Vector3(0f, 1f);
		orientedPoint.uv = num2 * Vector2.one;
		PipeRenderer.OrientedPoint orientedPoint3 = orientedPoint;
		PipeRenderer.OrientedPoint orientedPoint4 = orientedPoint3;
		orientedPoint4.position.x = 0f - orientedPoint4.position.x;
		orientedPoint4.normal.x = 0f - orientedPoint4.normal.x;
		orientedPoint4.uv = Vector2.one - orientedPoint4.uv;
		PipeRenderer.OrientedPoint orientedPoint5 = orientedPoint2;
		orientedPoint5.position.x = 0f - orientedPoint5.position.x;
		orientedPoint5.normal.x = 0f - orientedPoint5.normal.x;
		orientedPoint5.uv = Vector2.one;
		List<PipeRenderer.OrientedPoint> list = new List<PipeRenderer.OrientedPoint>();
		list.Add(orientedPoint2);
		list.Add(orientedPoint3);
		list.Add(orientedPoint4);
		list.Add(orientedPoint5);
		for (int i = 1; i <= 3; i++)
		{
			PipeRenderer.OrientedPoint orientedPoint6 = orientedPoint2;
			PipeRenderer.OrientedPoint orientedPoint7 = orientedPoint3;
			PipeRenderer.OrientedPoint orientedPoint8 = orientedPoint4;
			PipeRenderer.OrientedPoint orientedPoint9 = orientedPoint5;
			for (int j = 0; j < i; j++)
			{
				orientedPoint6 = Rotate90(orientedPoint6);
				orientedPoint7 = Rotate90(orientedPoint7);
				orientedPoint8 = Rotate90(orientedPoint8);
				orientedPoint9 = Rotate90(orientedPoint9);
			}
			orientedPoint6.uv += i * Vector2.one;
			orientedPoint7.uv += i * Vector2.one;
			orientedPoint8.uv += i * Vector2.one;
			orientedPoint9.uv += i * Vector2.one;
			list.Add(orientedPoint6);
			list.Add(orientedPoint7);
			list.Add(orientedPoint8);
			list.Add(orientedPoint9);
		}
		list.Reverse();
		return list.ToArray();
		static PipeRenderer.OrientedPoint Rotate90(PipeRenderer.OrientedPoint original)
		{
			Quaternion quaternion = Quaternion.AngleAxis(-90f, Vector3.forward);
			PipeRenderer.OrientedPoint result = original;
			result.position = quaternion * original.position;
			result.normal = quaternion * original.normal;
			return result;
		}
	}
}
