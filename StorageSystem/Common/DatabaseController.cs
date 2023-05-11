using Microsoft.Data.SqlClient;
using StorageSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSystem.Common
{
	public static class DatabaseController
	{
		public static SqlConnection Connection;
		public static string Username = "", Password = "";
		public static bool ReadonlyAccess;
		
		public static void TryConnect(string user, string password)
		{
			Username = user;
			Password = password;

			string connectionString = $"Data Source={AppHelpers.Settings.DataSource};Initial Catalog={AppHelpers.Settings.InitialCatalog};TrustServerCertificate=true;Persist Security Info=False";
			
			if (!string.IsNullOrEmpty(user))
				connectionString += $";User ID={user}";
			if (!string.IsNullOrEmpty(password))
				connectionString += $";Password={password}";

			Connection = new SqlConnection(connectionString);
			Connection.Open();

			// TODO: Check if user has readonly access
			ReadonlyAccess = false;
		}

		#region Manufacturer

		public static IEnumerable<Manufacturer> GetManufacturers()
		{
			var command = new SqlCommand("SELECT * FROM Manufacturer", Connection);
			var reader = command.ExecuteReader();

			while (reader.Read())
			{
				yield return new Manufacturer
				{
					Id = reader.GetString(0),
					Name = reader.GetString(1),
					Contacts = reader.GetString(2)
				};
			}

			reader.Close();
		}

		public static void InsertManufacturer(Manufacturer manufacturer)
		{
			var command = new SqlCommand("INSERT INTO Manufacturer (manufacturer_id, name, contacts) VALUES (@id, @name, @contacts)", Connection);
			command.Parameters.AddWithValue("@id", manufacturer.Id);
			command.Parameters.AddWithValue("@name", manufacturer.Name);
			command.Parameters.AddWithValue("@contacts", manufacturer.Contacts);
			command.ExecuteNonQuery();
		}

		public static void UpdateManufacturer(string id, Manufacturer manufacturer)
		{
			var command = new SqlCommand("UPDATE Manufacturer SET manufacturer_id = @id, name = @name, contacts = @contacts WHERE manufacturer_id = @oldId", Connection);
			command.Parameters.AddWithValue("@id", manufacturer.Id);
			command.Parameters.AddWithValue("@name", manufacturer.Name);
			command.Parameters.AddWithValue("@contacts", manufacturer.Contacts);
			command.Parameters.AddWithValue("@oldId", id);
			command.ExecuteNonQuery();
		}

		public static void DeleteManufacturer(string id)
		{
			var command = new SqlCommand("DELETE FROM Manufacturer WHERE manufacturer_id = @id", Connection);
			command.Parameters.AddWithValue("@id", id);
			command.ExecuteNonQuery();
		}

		#endregion

		#region Product

		public static IEnumerable<Product> GetProducts()
		{
			var command = new SqlCommand("SELECT * FROM Product", Connection);
			var reader = command.ExecuteReader();

			while (reader.Read())
			{
				yield return new Product
				{
					Id = reader.GetString(0),
					Name = reader.GetString(1),
					ManufacturerId = reader.GetString(2)
				};
			}

			reader.Close();
		}

		public static void InsertProduct(Product product)
		{
			var command = new SqlCommand("INSERT INTO Product (product_id, name, manufacturer_id) VALUES (@id, @name, @manufacturer)", Connection);
			command.Parameters.AddWithValue("@id", product.Id);
			command.Parameters.AddWithValue("@name", product.Name);
			command.Parameters.AddWithValue("@manufacturer", product.ManufacturerId);
			command.ExecuteNonQuery();
		}

		public static void UpdateProduct(string id, Product product)
		{
			var command = new SqlCommand("UPDATE Product SET product_id = @id, name = @name, manufacturer_id = @manufacturer WHERE product_id = @oldId", Connection);
			command.Parameters.AddWithValue("@id", product.Id);
			command.Parameters.AddWithValue("@name", product.Name);
			command.Parameters.AddWithValue("@manufacturer", product.ManufacturerId);
			command.Parameters.AddWithValue("@oldId", id);
			command.ExecuteNonQuery();
		}

		public static void DeleteProduct(string id)
		{
			var command = new SqlCommand("DELETE FROM Product WHERE product_id = @id", Connection);
			command.Parameters.AddWithValue("@id", id);
			command.ExecuteNonQuery();
		}

		#endregion

	}
}
