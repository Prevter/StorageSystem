using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StorageSystem.Common
{
	public sealed class Settings
	{
		public string DataSource { get; set; } = "HOME-PC\\SQLEXPRESS";
		public string InitialCatalog { get; set; } = "StorageSystem";

		public static Settings Load()
		{
			try
			{
				if (File.Exists("config.json"))
				{
					string json = File.ReadAllText("config.json");
					Settings? settings = JsonSerializer.Deserialize<Settings>(json);
					if (settings != null)
						return settings;
				}
			}
			catch (Exception) { }

			return new();
		}

		public void Save()
		{
			File.WriteAllText("config.json", JsonSerializer.Serialize(this));
		}
	}
}
