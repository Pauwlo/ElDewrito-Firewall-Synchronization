using System;
using System.Text.RegularExpressions;

namespace ElDewritoFirewallSynchronization
{
	class Ban
	{
		public int    ID { get; private set; }
		public string IP { get; private set; }
		public string Description { get; private set; }
		public string Date { get; private set; }
		public string Username { get; private set; }
		public string UID { get; private set; }
		public bool   IsSubnet { get; private set; }
		
		// IPv4 and CIDR regular expressions.
		private static Regex ipRegex     = new Regex(@"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
		private static Regex subnetRegex = new Regex(@"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\/([0-9]|[1-2][0-9]|3[0-2]))$");

		public Ban(string id, string ip, string date,
				   string description, string username, string uid)
		{
			ID = Convert.ToInt32(id);
			IP = ip;
			Description = description;
			Date = date;
			Username = username;
			UID = uid;

			if (id == "" || ip == "" || date == "")
				throw new Exception("Invalid database entry: ID, IP and Date are required.\n" + this);

			if (subnetRegex.Match(ip).Success)
				IsSubnet = true;
			else if (! ipRegex.Match(ip).Success)
				throw new Exception("Invalid database entry: IP format must be IPv4 or CIDR (subnet ban).\n" + this);
		}

		public override string ToString()
		{
			return $"ID: {ID}\nIP: {IP}\nDescription: {Description}\nDate: {Date}\n" +
				   $"Username: {Username}\nUID: {UID}";
		}
	}
}
