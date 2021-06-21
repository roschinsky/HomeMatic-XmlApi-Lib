using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TRoschinsky.Lib.HomeMaticXmlApi;

namespace TST_CoreHMXmlApi {
	class Program {
		static async Task Main(string[] args) {
			Console.WriteLine("Please enter your HM URL (http://192.168.1.199 for an example)");
			var strHMURL=Console.ReadLine();
			await TryConnectHM(strHMURL);
			Console.Write("Hit enter to finish");
			Console.ReadLine();
		}

		private static async Task TryConnectHM(string hmURL) {
			var sw = new Stopwatch();

			try {
				sw.Start();
				var hmWrapper = new HMApiWrapper(new Uri(hmURL));
				await hmWrapper.InitializeAsync(false, false);
				sw.Stop();
				if(hmWrapper.Devices.Count > 0) {
					Console.WriteLine($"{hmWrapper.Devices.Count} found");
					var nCnt = 0;
					foreach(var hD in hmWrapper.Devices) {
						Console.WriteLine($"{nCnt++}:\t{hD.Name} - {hD.Channels.Count} channels");
					}
				}
				Console.WriteLine($"Read {hmWrapper.Devices.Count} devices in {sw.Elapsed.TotalSeconds} seconds");
				sw.Restart();
				await hmWrapper.UpdateVariablesAsync();
				sw.Stop();
				Console.WriteLine($"Update variables in {sw.Elapsed.TotalSeconds} seconds");
				sw.Restart();
				await hmWrapper.UpdateMessagesAsync();
				sw.Stop();
				Console.WriteLine($"Update messages in {sw.Elapsed.TotalSeconds} seconds");
			}
			catch(Exception ex) {
				Console.WriteLine($"Connect to {hmURL} failed!");
				Console.WriteLine($"{ex.Message}");
			}
		}
	}
}
