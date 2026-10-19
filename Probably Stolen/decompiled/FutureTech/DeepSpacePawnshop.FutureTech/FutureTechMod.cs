using MelonLoader;

namespace DeepSpacePawnshop.FutureTech;

public sealed class FutureTechMod : MelonMod
{
	public override void OnInitializeMelon()
	{
		FutureIcons.Initialize(delegate(string message)
		{
			((MelonBase)this).LoggerInstance.Msg(message);
		});
		FutureItems.Initialize(delegate(string message)
		{
			((MelonBase)this).LoggerInstance.Msg(message);
		});
		FutureAcquisition.Initialize(delegate(string message)
		{
			((MelonBase)this).LoggerInstance.Msg(message);
		});
		FutureNight.Initialize(delegate(string message)
		{
			((MelonBase)this).LoggerInstance.Msg(message);
		});
		((MelonBase)this).LoggerInstance.Msg("Future Tech Public edition initialized.");
	}
}
