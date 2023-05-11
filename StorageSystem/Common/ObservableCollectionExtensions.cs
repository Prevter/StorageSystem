using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSystem.Common
{
	public static class ObservableCollectionExtensions
	{
		public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
		{
			foreach (var item in items)
				collection.Add(item);
		}

		public static void RemoveAll<T>(this ObservableCollection<T> collection, Func<T, bool> predicate)
		{
			foreach (var item in collection.Where(predicate).ToList())
				collection.Remove(item);
		}

		public static void ForEach<T>(this ObservableCollection<T> collection, Action<T> action)
		{
			foreach (var item in collection)
				action(item);
		}
	}
}
