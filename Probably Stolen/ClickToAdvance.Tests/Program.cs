using ClickToAdvanceFinal;

static void Assert(bool ok, string label) { if (!ok) throw new Exception(label); Console.WriteLine("PASS " + label); }
ClickToAdvanceRunner.Advance(null);
var noDialog = new DialogUIManager(); ClickToAdvanceRunner.Advance(noDialog); Assert(noDialog.Calls == 0, "no active dialog");
var choice = new DialogUIManager { currentDialog = new object(), Choice = true }; ClickToAdvanceRunner.Advance(choice); Assert(choice.Calls == 0, "choices are not advanced");
var active = new DialogUIManager { currentDialog = new object() }; ClickToAdvanceRunner.Advance(active); Assert(active.Calls == 1, "active dialog calls native advance once");
