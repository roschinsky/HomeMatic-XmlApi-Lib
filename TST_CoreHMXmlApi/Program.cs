using System;
using System.Diagnostics;
using TRoschinsky.Lib.HomeMaticXmlApi;

namespace TST_CoreHMXmlApi {
	class Program {
		static void Main(string[] args) {
			Console.WriteLine("Please enter your HM URL (http://192.168.1.199 for an example)");
			string strHMURL=Console.ReadLine();
			TryConnectHM(strHMURL);
			Console.Write("Hit enter to finish");
			Console.ReadLine();
		}

		private static void TryConnectHM(string hmURL) {
			Stopwatch sw = new Stopwatch();

			try {
				sw.Start();
				HMApiWrapper hmWrapper = new HMApiWrapper(new Uri(hmURL), false, false);
				sw.Stop();
				if(hmWrapper.Devices.Count > 0) {					
					Console.WriteLine($"{hmWrapper.Devices.Count} found");
					int nCnt = 0;
					foreach(HMDevice hD in hmWrapper.Devices) {
						Console.WriteLine($"{nCnt++}:\t{hD.Name} - {hD.Channels.Count} channels");
					}
				}
				Console.WriteLine($"Read {hmWrapper.Devices.Count} devices in {sw.Elapsed.TotalSeconds} seconds");
				sw.Restart();
				hmWrapper.UpdateVariables();
				sw.Stop();
				Console.WriteLine($"Update variables in {sw.Elapsed.TotalSeconds} seconds");
				sw.Restart();
				hmWrapper.UpdateMessages();
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