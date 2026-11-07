namespace XGDragFixer.Tests;

/// <summary>Offline checks for the retained (reference-ported) patches.</summary>
internal static class Program
{
    private static int Main()
    {
        try
        {
            RuntimeRegressionTests.Run();
            Console.WriteLine("ALL TESTS PASSED");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("TESTS FAILED: " + ex);
            return 1;
        }
    }
}
