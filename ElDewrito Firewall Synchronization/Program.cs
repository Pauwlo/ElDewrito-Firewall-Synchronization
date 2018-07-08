using System;
using System.Configuration;
using System.ServiceProcess;

namespace ElDewritoFirewallSynchronization
{
	static class Program
	{
		static void Main()
		{
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[]
			{
				new ElDewritoFirewallSynchronization()
			};
			ServiceBase.Run(ServicesToRun);
		}
	}
}
