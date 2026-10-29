using System;

namespace SmartInventoryArrange;

internal static class ReferenceIdentity
{
    internal static IntPtr Pointer(object value) => (IntPtr)System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(value);
}
