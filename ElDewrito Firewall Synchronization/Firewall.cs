using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NetFwTypeLib;

namespace ElDewritoFirewallSynchronization
{
	class Firewall
	{
		private static Regex ruleNameRegex = new Regex(@"^Ban #([0-9]+): [A-Za-z]+ ban.*");

		public static object GetRules(bool onlyID = false)
		{
			// Using Windows Firewall API to get a firewall instance.
			INetFwPolicy2 firewallPolicy = (INetFwPolicy2) Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

			List<INetFwRule> rules = new List<INetFwRule>();

			// Filtering firewall rules to get only ElDewrito-related ones.
			foreach (INetFwRule rule in firewallPolicy.Rules)
			{
				Match match = ruleNameRegex.Match(rule.Name);

				if (rule.Direction == NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN &&
					rule.Grouping == "ElDewrito" &&
					match.Success)
				{
					rules.Add(rule);
				}
			}

			// It's easier to compare integers instead of firewall rules.
			if (onlyID)
			{
				List<int> rulesID = new List<int>();

				foreach (INetFwRule rule in rules)
				{
					Match match = ruleNameRegex.Match(rule.Name);

					int id = Convert.ToInt32(match.Groups[1].Value);
					rulesID.Add(id);
				}

				return rulesID;
			}

			return rules;
		}

		private static void AddRule(INetFwRule r)
		{
			INetFwPolicy2 firewallPolicy = (INetFwPolicy2) Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
			firewallPolicy.Rules.Add(r);
		}

		private static void RemoveRule(string name)
		{
			INetFwPolicy2 firewallPolicy2 = (INetFwPolicy2) Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
			firewallPolicy2.Rules.Remove(name);
		}

		public static void Ban(Ban b)
		{
			// Using Windows Firewall API to create a new rule.
			INetFwRule r = (INetFwRule) Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));

			r.Enabled = true; // Of course
			r.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN; // Inbound rule
			r.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK; // Block connection
			r.InterfaceTypes = "All";
			r.Grouping = "ElDewrito";
			r.RemoteAddresses = b.IP;
			
			if (b.IsSubnet)
				r.Name = $"Ban #{b.ID}: Subnet ban ({b.IP})";
			else if (! String.IsNullOrEmpty(b.Username))
				r.Name = $"Ban #{b.ID}: IP ban ({b.IP} - {b.Username})";
			else
				r.Name = $"Ban #{b.ID}: IP ban ({b.IP})";

			if (! String.IsNullOrEmpty(b.Description))
				r.Description = $"Date: {b.Date}\r\nDescription: {b.Description}";
			else
				r.Description = $"Date: {b.Date}";

			AddRule(r);
		}

		public static void Unban(int id)
		{
			List<INetFwRule> rules = (List<INetFwRule>) GetRules();

			foreach (INetFwRule rule in rules)
			{
				if (rule.Name.StartsWith("Ban #" + id))
				{
					RemoveRule(rule.Name);
				}
			}
		}
	}
}
