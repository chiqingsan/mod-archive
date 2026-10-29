using System.Collections;
using UnityEngine;

namespace SmartInventoryArrange;

internal sealed class SmartArrangeRunner : MonoBehaviour
{
    internal Coroutine RunCoroutine(IEnumerator routine) => StartCoroutine(routine);
}
