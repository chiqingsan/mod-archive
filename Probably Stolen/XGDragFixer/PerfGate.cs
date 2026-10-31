namespace XGDragFixer;

/// <summary>
/// Runtime on/off switch used by the A/B diagnostic mode. When false every
/// performance patch falls back to the vanilla code path; the ghost-drag fix
/// stays active because it is a correctness fix rather than a performance one.
/// Kept separate from the user config so an A/B run never rewrites settings.
/// </summary>
internal static class PerfGate
{
    internal static bool Active = true;
}
