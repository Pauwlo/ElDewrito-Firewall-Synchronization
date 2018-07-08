using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace ElDewritoFirewallSynchronization
{
	class DBConnect
	{
		private static DBConnect _instance = null;
		static readonly object instanceLock = new Object();

		private Config config;
		private MySqlConnection connection;

		// Singleton pattern.
		public static DBConnect Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (instanceLock)
					{
						if (_instance == null)
							_instance = new DBConnect();
					}
				}
				return _instance;
			}
		}

		private DBConnect()
		{
			config = Config.Instance;

			string dsn = $"server={config.Hostname};database={config.Database};" +
						 $"uid={config.Username};password={config.Password}" +
						 (config.SSL ? "" : ";sslmode=none");

			connection = new MySqlConnection(dsn);
		}

		private bool OpenConnection()
		{
			connection.Open();
			return true;
		}

		private bool CloseConnection()
		{
			connection.Close();
			return true;
		}

		public List<Ban> GetBans()
		{
			string query = "SELECT * FROM " + config.Table;

			List<Ban> rules = new List<Ban>();

			if (OpenConnection())
			{
				// This will be logged only if the connection was lost earlier.
				Logger.Log("Regained connection to database.", 10, 1);

				MySqlCommand cmd = new MySqlCommand(query, connection);
				MySqlDataReader dataReader = cmd.ExecuteReader();

				while (dataReader.Read())
				{
					Ban b;

					try
					{
						b = new Ban(dataReader["id"] + "",
									dataReader["ip"] + "",
									dataReader["date"] + "",
									dataReader["description"] + "",
									dataReader["username"] + "",
									dataReader["uid"] + "");

						rules.Add(b);
					}
					catch (Exception ex)
					{
						// Prevent logs spamming if an invalid ban has already been reported.
						if (! Logger.invalidBans.Contains(dataReader["id"] + ""))
						{
							Logger.Log(ex.Message, 11, 2);
							Logger.invalidBans.Add(dataReader["id"] + "");
						}

						continue;
					}
				}

				dataReader.Close();
				CloseConnection();

				return rules;
			}

			return null;
		}
	}
}
