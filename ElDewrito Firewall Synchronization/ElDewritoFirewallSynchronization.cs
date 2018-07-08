using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Timers;

namespace ElDewritoFirewallSynchronization
{
	public partial class ElDewritoFirewallSynchronization : ServiceBase
	{
		private Timer timer = null;
		private DBConnect DB;

		private List<int> existingRules;
		private List<Ban> bans;

		public ElDewritoFirewallSynchronization()
		{
			InitializeComponent();
			DB = DBConnect.Instance;
		}

		protected override void OnStart(string[] args)
		{
			// Run the synchronization every 20 seconds.
			timer = new Timer(20000);
			timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
			timer.Start();
		}

		protected override void OnStop()
		{
		}

		private void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			// Get rules from Windows Firewall.
			existingRules = (List<int>) Firewall.GetRules(true);

			try
			{
				// Get bans from database.
				bans = DB.GetBans();

				// If a ban is not found in rules, add it. (banned)
				foreach (Ban ban in bans)
				{
					if (! existingRules.Contains(ban.ID))
					{
						Logger.Log("Added ban #" + ban.ID, 1, 0);
						Firewall.Ban(ban);
					}
				}

				// If a rule is not found in database, remove it. (unbanned)
				for (int i = existingRules.Count - 1; i >= 0; i--)
				{
					if (! bans.Any(ban => ban.ID == existingRules[i]))
					{
						Logger.Log("Removed ban #" + existingRules[i], 2, 0);
						Firewall.Unban(existingRules[i]);
					}
				}
			}
			catch (MySqlException ex)
			{
				switch (ex.Number)
				{
					case 0:
						Logger.Log("Couldn't connect to MySQL database. Please check hostname, credentials and SSL mode in configuration file.\nService stopped.", 201, 4);
						return;

					case 1042:
						Logger.Log("Couldn't connect to MySQL database. Please check hostname, credentials and SSL mode in configuration file.\nTrying to reconnect...", 202, 3);
						return;

					case 1146:
						Logger.Log("Invalid table specified.\nService stopped.", 203, 4);
						return;

					default:
						Logger.Log("Unspecified MySQL error: " + ex.Number + "\n" + ex.Message + "\nService stopped.", 200, 4);
						return;
				}
			}

			// Optimizing memory usage by calling the Garbage Collector manually.
			System.GC.Collect();
		}
	}
}
