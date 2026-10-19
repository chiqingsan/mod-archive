using System.Collections.Generic;
using UnityEngine;

namespace Duckov.Splines;

[RequireComponent(typeof(PipeRenderer))]
public class BeveledLineShape : ShapeProvider
{
	public PipeRenderer pipeRenderer;

	public Points pointsComponent;

	[Header("Shape")]
	public float bevelSize = 0.5f;

	[Header("Subdivide")]
	public int subdivision = 2;

	public bool subdivideByLength;

	public float subdivisionLength = 0.1f;

	[Header("UV")]
	public float uvMultiplier = 1f;

	public float uvOffset;

	[Header("Extra")]
	public bool useProtectionOffset;

	public float protectionOffset = 0.2f;

	public bool edit;

	[HideInInspector]
	public List<Vector3> points
	{
		get
		{
			if ((bool)pointsComponent)
			{
				return pointsComponent.points;
			}
			return null;
		}
	}

	public override PipeRenderer.OrientedPoint[] GenerateShape()
	{
		List<PipeRenderer.OrientedPoint> list = new List<PipeRenderer.OrientedPoint>();
		if (!pointsComponent || points.Count <= 1)
		{
			return list.ToArray();
		}
		if (pointsComponent.worldSpace)
		{
			pointsComponent.worldSpace = false;
		}
		int count = points.Count;
		for (int i = 0; i < count; i++)
		{
			Vector3 vector = points[i];
			if (i == 0 || i == count - 1)
			{
				PipeRenderer.OrientedPoint orientedPoint = default(PipeRenderer.OrientedPoint);
				orientedPoint.position = vector;
				orientedPoint.tangent = ((i == 0) ? (points[i + 1] - vector).normalized : (vector - points[i - 1]).normalized);
				orientedPoint.normal = Vector3.up;
				orientedPoint.rotationalAxisVector = Vector3.forward;
				PipeRenderer.OrientedPoint item = orientedPoint;
				list.Add(item);
				continue;
			}
			Vector3 vector2 = points[i - 1];
			Vector3 vector3 = points[i + 1];
			Vector3 vector4 = vector - vector2;
			Vector3 vector5 = vector3 - vector2;
			Vector3 vector6 = Vector3.Cross(vector3 - vector, vector2 - vector);
			if (vector4.magnitude == 0f || vector5.magnitude == 0f || vector4.normalized == vector5.normalized || vector4.normalized == -vector5.normalized)
			{
				Vector3 normalized = (vector3 - vector).normalized;
				PipeRenderer.OrientedPoint orientedPoint = default(PipeRenderer.OrientedPoint);
				orientedPoint.position = vector;
				orientedPoint.tangent = normalized;
				orientedPoint.normal = vector6;
				orientedPoint.rotationalAxisVector = Vector3.forward;
				orientedPoint.rotation = Quaternion.LookRotation(normalized, vector6);
				orientedPoint.uv = Vector2.zero;
				PipeRenderer.OrientedPoint item2 = orientedPoint;
				list.Add(item2);
				continue;
			}
			float a = ((i >= 2) ? ((vector - vector2).magnitude / 2f) : (vector - vector2).magnitude);
			float b = ((i < count - 2) ? ((vector - vector3).magnitude / 2f) : (vector - vector3).magnitude);
			float clipDistance = Mathf.Min(a, b);
			Vector3 o;
			Vector3 axis;
			Vector3[] array = Bevel.Evaluate(vector, vector2, vector3, subdivision, bevelSize, out o, out axis, protectionOffset, useProtectionOffset, clipDistance);
			for (int j = 0; j < array.Length; j++)
			{
				Vector3 vector7 = array[j];
				Vector3 vector8 = ((j < array.Length - 1) ? array[j + 1] : vector3) - vector7;
				Vector3 vector9 = o - vector7;
				Vector3 vector10 = ((useProtectionOffset && (j == 0 || j == array.Length - 1)) ? vector8.normalized : (Quaternion.AngleAxis(-90f, axis) * vector9));
				float num = 0.001f;
				if (!(vector8.magnitude < num))
				{
					Quaternion rotation = Quaternion.LookRotation(vector10, vector6);
					Vector3 forward = Vector3.forward;
					PipeRenderer.OrientedPoint orientedPoint = default(PipeRenderer.OrientedPoint);
					orientedPoint.position = vector7;
					orientedPoint.tangent = vector10;
					orientedPoint.normal = vector6;
					orientedPoint.rotationalAxisVector = forward;
					orientedPoint.rotation = rotation;
					orientedPoint.uv = Vector2.zero;
					PipeRenderer.OrientedPoint item3 = orientedPoint;
					list.Add(item3);
				}
			}
		}
		if (subdivideByLength && subdivisionLength > 0f)
		{
			for (int k = 0; k < list.Count - 1; k++)
			{
				PipeRenderer.OrientedPoint orientedPoint2 = list[k];
				PipeRenderer.OrientedPoint orientedPoint3 = list[k + 1];
				Vector3 vector11 = orientedPoint3.position - orientedPoint2.position;
				Vector3 normalized2 = vector11.normalized;
				float magnitude = vector11.magnitude;
				if (!(magnitude > subdivisionLength))
				{
					continue;
				}
				int num2 = Mathf.FloorToInt(magnitude / subdivisionLength);
				for (int l = 1; l <= num2; l++)
				{
					Vector3 vector12 = orientedPoint2.position + l * normalized2 * subdivisionLength;
					if ((vector12 - orientedPoint3.position).magnitude < subdivisionLength)
					{
						break;
					}
					PipeRenderer.OrientedPoint orientedPoint = default(PipeRenderer.OrientedPoint);
					orientedPoint.position = vector12;
					orientedPoint.normal = orientedPoint2.normal;
					orientedPoint.rotation = orientedPoint2.rotation;
					orientedPoint.rotationalAxisVector = orientedPoint2.rotationalAxisVector;
					orientedPoint.tangent = orientedPoint2.tangent;
					orientedPoint.uv = orientedPoint2.uv;
					PipeRenderer.OrientedPoint item4 = orientedPoint;
					list.Insert(k + l, item4);
				}
			}
		}
		PipeRenderer.OrientedPoint[] array2 = list.ToArray();
		array2 = PipeHelperFunctions.RemoveDuplicates(array2);
		PipeHelperFunctions.RecalculateNormals(ref array2);
		PipeHelperFunctions.RecalculateUvs(ref array2, uvMultiplier, uvOffset);
		return array2;
	}
}
