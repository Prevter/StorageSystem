﻿using Microsoft.Data.SqlClient;
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

		public static bool IsConnected()
		{
			return Connection != null;
		}

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

		public static Manufacturer? GetManufacturer(string id)
		{
			var command = new SqlCommand("SELECT * FROM Manufacturer WHERE manufacturer_id = @id", Connection);
			command.Parameters.AddWithValue("@id", id);
			var reader = command.ExecuteReader();

			Manufacturer? manufacturer = null;

			if (reader.Read())
			{
				manufacturer = new Manufacturer
				{
					Id = reader.GetString(0),
					Name = reader.GetString(1),
					Contacts = reader.GetString(2)
				};
			}

			reader.Close();
			return manufacturer;
		}

		public static void InsertManufacturer(Manufacturer manufacturer)
		{
			var command = new SqlCommand("INSERT INTO Manufacturer (manufacturer_id, name, contacts) VALUES (@id, @name, @contacts)", Connection);
			command.Parameters.AddWithValue("@id", manufacturer.Id);
			command.Parameters.AddWithValue("@name", manufacturer.Name);
			command.Parameters.AddWithValue("@contacts", manufacturer.Contacts);
			command.ExecuteNonQuery();
		}

		public static void UpdateManufacturer(Manufacturer manufacturer)
		{
			var command = new SqlCommand("UPDATE Manufacturer SET name = @name, contacts = @contacts WHERE manufacturer_id = @id", Connection);
			command.Parameters.AddWithValue("@id", manufacturer.Id);
			command.Parameters.AddWithValue("@name", manufacturer.Name);
			command.Parameters.AddWithValue("@contacts", manufacturer.Contacts);
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

		public static Product? GetProduct(string id)
		{
			var command = new SqlCommand("SELECT * FROM Product WHERE product_id = @id", Connection);
			command.Parameters.AddWithValue("@id", id);
			var reader = command.ExecuteReader();

			Product? result = null;

			if (reader.Read())
			{
				result = new Product
				{
					Id = reader.GetString(0),
					Name = reader.GetString(1),
					ManufacturerId = reader.GetString(2)
				};
			}

			reader.Close();
			return result;
		}

		public static void InsertProduct(Product product)
		{
			var command = new SqlCommand("INSERT INTO Product (product_id, name, manufacturer_id) VALUES (@id, @name, @manufacturer)", Connection);
			command.Parameters.AddWithValue("@id", product.Id);
			command.Parameters.AddWithValue("@name", product.Name);
			command.Parameters.AddWithValue("@manufacturer", product.ManufacturerId);
			command.ExecuteNonQuery();
		}

		public static void UpdateProduct(Product product)
		{
			var command = new SqlCommand("UPDATE Product SET name = @name, manufacturer_id = @manufacturer WHERE product_id = @id", Connection);
			command.Parameters.AddWithValue("@id", product.Id);
			command.Parameters.AddWithValue("@name", product.Name);
			command.Parameters.AddWithValue("@manufacturer", product.ManufacturerId);
			command.ExecuteNonQuery();
		}

		public static void DeleteProduct(string id)
		{
			var command = new SqlCommand("DELETE FROM Product WHERE product_id = @id", Connection);
			command.Parameters.AddWithValue("@id", id);
			command.ExecuteNonQuery();
		}

		#endregion

		#region Shop

		public static IEnumerable<Shop> GetShops()
		{
			var command = new SqlCommand("SELECT * FROM Shop", Connection);
			var reader = command.ExecuteReader();

			while (reader.Read())
			{
				yield return new Shop
				{
					Id = reader.GetString(0),
					Name = reader.GetString(1),
					Floor = reader.GetInt32(2),
					Location = reader.GetString(3)
				};
			}

			reader.Close();
		}

		public static void InsertShop(Shop shop)
		{
			var command = new SqlCommand("INSERT INTO Shop (shop_id, name, floor, location) VALUES (@id, @name, @floor, @location)", Connection);
			command.Parameters.AddWithValue("@id", shop.Id);
			command.Parameters.AddWithValue("@name", shop.Name);
			command.Parameters.AddWithValue("@floor", shop.Floor);
			command.Parameters.AddWithValue("@location", shop.Location);
			command.ExecuteNonQuery();
		}

		public static void UpdateShop(string id, Shop shop)
		{
			var command = new SqlCommand("UPDATE Shop SET name = @name, floor = @floor, location = @location WHERE shop_id = @id", Connection);
			command.Parameters.AddWithValue("@id", shop.Id);
			command.Parameters.AddWithValue("@name", shop.Name);
			command.Parameters.AddWithValue("@floor", shop.Floor);
			command.Parameters.AddWithValue("@location", shop.Location);
			command.ExecuteNonQuery();
		}

		public static void DeleteShop(string id)
		{
			var command = new SqlCommand("DELETE FROM Shop WHERE shop_id = @id", Connection);
			command.Parameters.AddWithValue("@id", id);
			command.ExecuteNonQuery();
		}

		#endregion

		#region ShopProduct

		public static IEnumerable<ShopProduct> GetShopProducts()
		{
			var command = new SqlCommand("SELECT * FROM ShopProduct", Connection);
			var reader = command.ExecuteReader();

			while (reader.Read())
			{
				yield return new ShopProduct
				{
					ProductId = reader.GetString(0),
					ShopId = reader.GetString(1),
					Price = reader.GetDecimal(2)
				};
			}

			reader.Close();
		}

		public static void InsertShopProduct(ShopProduct shopProduct)
		{
			var command = new SqlCommand("INSERT INTO ShopProduct (shop_id, product_id, price) VALUES (@shopId, @productId, @price)", Connection);
			command.Parameters.AddWithValue("@shopId", shopProduct.ShopId);
			command.Parameters.AddWithValue("@productId", shopProduct.ProductId);
			command.Parameters.AddWithValue("@price", shopProduct.Price);
			command.ExecuteNonQuery();
		}

		public static void UpdateShopProduct(ShopProduct shopProduct)
		{
			var command = new SqlCommand("UPDATE ShopProduct SET price = @price WHERE shop_id = @shopId AND product_id = @productId", Connection);
			command.Parameters.AddWithValue("@shopId", shopProduct.ShopId);
			command.Parameters.AddWithValue("@productId", shopProduct.ProductId);
			command.Parameters.AddWithValue("@price", shopProduct.Price);
			command.ExecuteNonQuery();
		}

		public static void DeleteShopProduct(string shopId, string productId)
		{
			var command = new SqlCommand("DELETE FROM ShopProduct WHERE shop_id = @shopId AND product_id = @productId", Connection);
			command.Parameters.AddWithValue("@shopId", shopId);
			command.Parameters.AddWithValue("@productId", productId);
			command.ExecuteNonQuery();
		}

		#endregion
	}
}
