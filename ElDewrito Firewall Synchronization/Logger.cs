using System.Collections.Generic;
using System.Diagnostics;

namespace ElDewritoFirewallSynchronization
{
	class Logger
	{
		public static List<string> invalidBans = new List<string>();
		private static bool shouldLog = true;

		public static void Log(string message, int eventID, int lvl = 0)
		{
			EventLogEntryType type;

			switch (lvl)
			{
				case 1:
					type = EventLogEntryType.Information;
					break;
				case 2:
					type = EventLogEntryType.Warning;
					break;
				case 3:
				case 4:
					type = EventLogEntryType.Error;
					break;
				default:
#if DEBUG
					type = EventLogEntryType.Information;
					break;
#else
					return;
#endif
			}

			// Using Windows Events to log service errors and warnings.
			EventLog eventLog = new EventLog();

			eventLog.Source = "ElDewrito Firewall Synchronization";

			if (! EventLog.SourceExists(eventLog.Source))
				EventLog.CreateEventSource(eventLog.Source, "Application");

			if ((shouldLog && lvl != 1) || (! shouldLog && lvl == 1))
				eventLog.WriteEntry(message, type, eventID);

			eventLog.Close();

			switch (lvl)
			{
				case 1: // Re-enable logging when the previous issue is fixed.
					shouldLog = true;
					break;
				case 3: // Prevent logs spamming by displaying the message only once.
					shouldLog = false;
					break;
				case 4: // Fatar error, stop the service.
					System.Environment.Exit(eventID);
					break;
			}
		}
	}
}
