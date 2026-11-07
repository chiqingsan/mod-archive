using UnityEngine;
using UnityEngine.EventSystems;

namespace UndergroundExchangeAssistant;

// Button handles pointer down/up/click; own drag gestures so they cannot
// bubble to the trashcan's inventory drag handler.
public sealed class PaperPointerBlocker : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public void OnBeginDrag(PointerEventData data) { data.Use(); }
    public void OnDrag(PointerEventData data) { data.Use(); }
    public void OnEndDrag(PointerEventData data) { data.Use(); }
}
