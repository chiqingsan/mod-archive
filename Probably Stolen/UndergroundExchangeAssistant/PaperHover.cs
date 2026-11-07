using UnityEngine;
using UnityEngine.EventSystems;

namespace UndergroundExchangeAssistant;

// Toggle only the precomputed exterior silhouette; do not tint, scale or move
// the paper. Native TabletTooltipTrigger owns the mouse-following name tooltip.
internal sealed class PaperHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject? highlight;

    internal void Init(GameObject highlightVisual)
    {
        highlight = highlightVisual;
        if (highlight != null) highlight.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData) => Set(true);

    public void OnPointerExit(PointerEventData eventData) => Set(false);

    private void OnDisable() => Set(false);

    private void Set(bool value)
    {
        if (highlight == null || highlight.activeSelf == value) return;
        highlight.SetActive(value);
    }
}
