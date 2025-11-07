using System;

using Skyline.AppInstaller;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Net.AppPackages;
using Skyline.DataMiner.Net.Messages;

/// <summary>
/// DataMiner Script Class.
/// </summary>
internal class Script
{
	/// <summary>
	/// The script entry point.
	/// </summary>
	/// <param name="engine">Provides access to the Automation engine.</param>
	/// <param name="context">Provides access to the installation context.</param>
	[AutomationEntryPoint(AutomationEntryPointType.Types.InstallAppPackage)]
	public void Install(IEngine engine, AppInstallContext context)
	{
		try
		{
			engine.Timeout = new TimeSpan(0, 10, 0);
			engine.GenerateInformation("Starting installation");
			var installer = new AppInstaller(Engine.SLNetRaw, context);
			installer.InstallDefaultContent();
			UpdateThemeScriptReference(engine);
		}
		catch (Exception e)
		{
			engine.ExitFail($"Exception encountered during installation: {e}");
		}
	}

	private static void UpdateThemeScriptReference(IEngine engine)
	{
		var infoMessage = new GetScriptInfoMessage
		{
			Name = "Merge LCA themes",
		};

		var response = engine.SendSLNetMessage(infoMessage)[0] as GetScriptInfoResponseMessage;

		var dllRefs = response.Exes[0].CSharpDllRefs;
		dllRefs = dllRefs.Replace("C:\\Skyline DataMiner\\ProtocolScripts\\WebApiLib.dll", "C:\\Skyline DataMiner\\Webpages\\API\\bin\\WebApiLib.dll");

		response.Exes[0].CSharpDllRefs = dllRefs;

		var saveScriptMessage = new SaveAutomationScriptMessage
		{
			Definition = response,
			IsUpdate = true,
		};

		engine.SendSLNetMessage(saveScriptMessage);
	}
}