using UnityEngine;

public static class CoroutineTool
{
	public static WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();

	public static WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();

	public static WaitForSeconds WaitForSeconds1 = new WaitForSeconds(1f);

	public static WaitForSeconds WaitForSeconds2 = new WaitForSeconds(2f);

	public static WaitForSeconds WaitForSeconds3 = new WaitForSeconds(3f);
}
