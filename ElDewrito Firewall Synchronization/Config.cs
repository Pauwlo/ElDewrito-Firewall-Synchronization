using System;
using System.Configuration;
using System.IO;

namespace ElDewritoFirewallSynchronization
{
	class Config
	{
		private bool ssl;

		public string Hostname { get; private set; }
		public string Database { get; private set; }
		public string Table    { get; private set; }
		public string Username { get; private set; }
		public string Password { get; private set; }

		public bool SSL
		{
			get { return ssl; }
			private set { ssl = value; }
		}

		private static Config _instance = null;
		static readonly object instanceLock = new Object();

		// Singleton pattern.
		public static Config Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (instanceLock)
					{
						if (_instance == null)
							_instance = new Config();
					}
				}
				return _instance;
			}
		}

		private Config()
		{
			AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", "Settings.xml");

			Hostname = ConfigurationManager.AppSettings["hostname"];
			Database = ConfigurationManager.AppSettings["database"];
			Table = ConfigurationManager.AppSettings["table"];
			Username = ConfigurationManager.AppSettings["username"];
			Password = ConfigurationManager.AppSettings["password"];
			Boolean.TryParse(ConfigurationManager.AppSettings["ssl"], out ssl);

			if (String.IsNullOrEmpty(Hostname))
				Logger.Log("Configuration file is corrupted or missing. Please fix it.", 101, 4);
		}
	}
}
