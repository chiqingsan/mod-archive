using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace AutoStorage;

internal static class World
{
	private static readonly List<IntPtr> _protected = new List<IntPtr>();

	private static readonly string[] BlockWords = new string[19]
	{
		"display_case", "displaycase", "showcase", "counter", "sold", "trashcan", "trash", "drain", "faucet", "vending",
		"cassette", "swapbuffer", "swap_buffer", "response", "afterhour", "docinv", "bazar", "hidden", "dossier"
	};

	private static readonly string[] StorageHints = new string[14]
	{
		"storage", "stash", "chest", "crate", "locker", "cabinet", "smuggler", "储藏", "储物", "货柜",
		"货架", "箱", "柜", "暗格"
	};

	public static bool IncludeAll = false;

	private static readonly string[] StorageExclusions = new string[2] { "toolbox", "工具箱" };

	private static readonly List<Container> _containers = new List<Container>();

	private static float _lastScan = -1000f;

	private static void Protect(GameInventory inv)
	{
		IntPtr intPtr = U.Ptr((Il2CppObjectBase)(object)inv);
		if (intPtr != IntPtr.Zero && !_protected.Contains(intPtr))
		{
			_protected.Add(intPtr);
		}
	}

	public static bool IsProtected(GameInventory inv)
	{
		IntPtr intPtr = U.Ptr((Il2CppObjectBase)(object)inv);
		if (intPtr != IntPtr.Zero)
		{
			return _protected.Contains(intPtr);
		}
		return false;
	}

	private static void RebuildProtected()
	{
		_protected.Clear();
		try
		{
			EmporiumEntry instance = EmporiumEntry.Instance;
			if ((Object)(object)instance != (Object)null)
			{
				Protect((GameInventory)(object)instance.invElement);
				Protect((GameInventory)(object)instance.frontInvinvElement);
				Protect((GameInventory)(object)instance.backInvinvElement);
				Protect((GameInventory)(object)instance.backInvinvElementCounter);
				Protect((GameInventory)(object)instance.showcaseElement);
				Protect((GameInventory)(object)instance.soldElement);
				Protect((GameInventory)(object)instance.hiddenElement);
				Protect((GameInventory)(object)instance.bazarLeftinvElement);
				Protect((GameInventory)(object)instance.trashcanInvElement);
				Protect((GameInventory)(object)instance.afterhourInventory);
				Protect((GameInventory)(object)instance.hirelingInv);
				Protect((GameInventory)(object)instance.responseInventory);
			}
		}
		catch
		{
		}
		try
		{
			PlayerStore instance2 = PlayerStore.instance;
			if (instance2 != null)
			{
				Protect((GameInventory)(object)instance2.gridInv);
			}
		}
		catch
		{
		}
	}

	public static bool BlockedId(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return false;
		}
		string text = id.ToLowerInvariant();
		for (int i = 0; i < BlockWords.Length; i++)
		{
			if (text.Contains(BlockWords[i]))
			{
				return true;
			}
		}
		return false;
	}

	public static bool LooksLikeStorage(string id, string name)
	{
		string text = (id + " " + name).ToLowerInvariant();
		if (text.Length == 0)
		{
			return false;
		}
		for (int i = 0; i < StorageExclusions.Length; i++)
		{
			if (text.Contains(StorageExclusions[i]))
			{
				return false;
			}
		}
		for (int j = 0; j < StorageHints.Length; j++)
		{
			if (text.Contains(StorageHints[j]))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsInventoryItem(GameItem it, out GameInventory inv)
	{
		inv = null;
		if (it == null)
		{
			return false;
		}
		if (BlockedId(U.SafeId(it)))
		{
			return false;
		}
		inv = ResolveInventory(it);
		if (inv == null)
		{
			return false;
		}
		if (IsProtected(inv))
		{
			return false;
		}
		GameItem val = null;
		try
		{
			val = inv.GetParentItem();
		}
		catch
		{
		}
		if (val != null && !U.Same((Il2CppObjectBase)(object)val, (Il2CppObjectBase)(object)it))
		{
			return false;
		}
		if (val == null && !OwnsInventory(it, inv))
		{
			return false;
		}
		try
		{
			if (inv.childItems == null)
			{
				return false;
			}
		}
		catch
		{
			return false;
		}
		return true;
	}

	private static bool OwnsInventory(GameItem it, GameInventory inv)
	{
		try
		{
			List<GraphNodeStorage> children = it.children;
			if (children == null)
			{
				return false;
			}
			int num = 0;
			try
			{
				num = children.Count;
			}
			catch
			{
			}
			if (num <= 0 || num > 256)
			{
				return false;
			}
			IntPtr intPtr = U.Ptr((Il2CppObjectBase)(object)inv);
			for (int i = 0; i < num; i++)
			{
				GraphNodeStorage val = null;
				try
				{
					val = children[i];
				}
				catch
				{
				}
				if (val != null)
				{
					GameInventory val2 = ((Il2CppObjectBase)val).TryCast<GameInventory>();
					if (val2 != null && U.Ptr((Il2CppObjectBase)(object)val2) == intPtr)
					{
						return true;
					}
				}
			}
		}
		catch
		{
		}
		return false;
	}

	public static bool IsContainerItem(GameItem it, out GameInventory inv)
	{
		if (!IsInventoryItem(it, out inv))
		{
			return false;
		}
		if (IncludeAll)
		{
			return true;
		}
		return LooksLikeStorage(U.SafeId(it), U.SafeName(it));
	}

	public static GameInventory ResolveInventory(GameItem it)
	{
		if (it == null)
		{
			return null;
		}
		try
		{
			PixelWindow contentWindow = it.contentWindow;
			if (contentWindow != null)
			{
				GameInventory val = FromWindow(contentWindow);
				if (val != null)
				{
					return val;
				}
			}
		}
		catch
		{
		}
		try
		{
			List<GraphNodeStorage> children = it.children;
			if (children != null)
			{
				int num = 0;
				try
				{
					num = children.Count;
				}
				catch
				{
				}
				if (num > 0 && num <= 256)
				{
					for (int i = 0; i < num; i++)
					{
						GraphNodeStorage val2 = null;
						try
						{
							val2 = children[i];
						}
						catch
						{
						}
						if (val2 != null)
						{
							GameInventory val3 = ((Il2CppObjectBase)val2).TryCast<GameInventory>();
							if (val3 != null && U.Ptr((Il2CppObjectBase)(object)val3) != IntPtr.Zero)
							{
								return val3;
							}
						}
					}
				}
			}
		}
		catch
		{
		}
		try
		{
			GraphNodeWindow child = it.child;
			if (child != null)
			{
				GraphNodeStorage node = null;
				try
				{
					node = child.child;
				}
				catch
				{
				}
				GameInventory val4 = DigInventory(node, 0);
				if (val4 != null)
				{
					return val4;
				}
			}
		}
		catch
		{
		}
		GameInventory val5 = ResolveInventoryByReflection(it);
		if (val5 != null)
		{
			return val5;
		}
		return null;
	}

	private static GameInventory DigInventory(GraphNodeStorage node, int depth)
	{
		if (node == null || depth > 4)
		{
			return null;
		}
		try
		{
			GameInventory val = ((Il2CppObjectBase)node).TryCast<GameInventory>();
			if (val != null && U.Ptr((Il2CppObjectBase)(object)val) != IntPtr.Zero)
			{
				return val;
			}
		}
		catch
		{
		}
		try
		{
			List<GraphNodeStorage> children = node.children;
			if (children != null)
			{
				int num = 0;
				try
				{
					num = children.Count;
				}
				catch
				{
				}
				if (num > 0 && num <= 256)
				{
					for (int i = 0; i < num; i++)
					{
						GraphNodeStorage node2 = null;
						try
						{
							node2 = children[i];
						}
						catch
						{
						}
						GameInventory val2 = DigInventory(node2, depth + 1);
						if (val2 != null)
						{
							return val2;
						}
					}
				}
			}
		}
		catch
		{
		}
		return null;
	}

	private static GameInventory ResolveInventoryByReflection(GameItem it)
	{
		try
		{
			Type type = ((object)it).GetType();
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (PropertyInfo propertyInfo in properties)
			{
				Type propertyType;
				try
				{
					propertyType = propertyInfo.PropertyType;
				}
				catch
				{
					continue;
				}
				if (propertyType != typeof(GameInventory) && !propertyType.IsSubclassOf(typeof(GameInventory)))
				{
					continue;
				}
				try
				{
					object? value = propertyInfo.GetValue(it, null);
					GameInventory val = (GameInventory)((value is GameInventory) ? value : null);
					if (val != null && U.Ptr((Il2CppObjectBase)(object)val) != IntPtr.Zero)
					{
						return val;
					}
				}
				catch
				{
				}
			}
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				Type fieldType;
				try
				{
					fieldType = fieldInfo.FieldType;
				}
				catch
				{
					continue;
				}
				if (fieldType != typeof(GameInventory) && !fieldType.IsSubclassOf(typeof(GameInventory)))
				{
					continue;
				}
				try
				{
					object? value2 = fieldInfo.GetValue(it);
					GameInventory val2 = (GameInventory)((value2 is GameInventory) ? value2 : null);
					if (val2 != null && U.Ptr((Il2CppObjectBase)(object)val2) != IntPtr.Zero)
					{
						return val2;
					}
				}
				catch
				{
				}
			}
		}
		catch
		{
		}
		return null;
	}

	private static GameInventory FromWindow(PixelWindow w)
	{
		if (w == null)
		{
			return null;
		}
		try
		{
			PixelElement childElement = w.childElement;
			if (childElement != null)
			{
				GameInventory val = ((Il2CppObjectBase)childElement).TryCast<GameInventory>();
				if (val != null)
				{
					return val;
				}
			}
		}
		catch
		{
		}
		try
		{
			GraphNodeStorage child = w.child;
			if (child != null)
			{
				GameInventory val2 = ((Il2CppObjectBase)child).TryCast<GameInventory>();
				if (val2 != null)
				{
					return val2;
				}
			}
		}
		catch
		{
		}
		try
		{
			PixelElement childElement2 = w.childElement;
			GridPixelElement val3 = ((childElement2 != null) ? ((Il2CppObjectBase)childElement2).TryCast<GridPixelElement>() : null);
			if (val3 != null)
			{
				int num = 0;
				int num2 = 0;
				try
				{
					num = val3.gridWidth;
				}
				catch
				{
				}
				try
				{
					num2 = val3.gridHeight;
				}
				catch
				{
				}
				if (num > 0 && num2 > 0 && num <= 64 && num2 <= 64)
				{
					for (int i = 0; i < num2; i++)
					{
						for (int j = 0; j < num; j++)
						{
							PixelElement element = val3.GetElement(j, i);
							if (element != null)
							{
								GameInventory val4 = ((Il2CppObjectBase)element).TryCast<GameInventory>();
								if (val4 != null)
								{
									return val4;
								}
							}
						}
					}
				}
			}
		}
		catch
		{
		}
		return null;
	}

	public static List<GameItem> ItemsIn(GameItem container)
	{
		List<GameItem> list = new List<GameItem>();
		if (container == null)
		{
			return list;
		}
		HashSet<IntPtr> seenInv = new HashSet<IntPtr>();
		HashSet<IntPtr> seenItem = new HashSet<IntPtr>();
		try
		{
			PixelWindow contentWindow = container.contentWindow;
			if (contentWindow != null)
			{
				GameInventory val = FromWindow(contentWindow);
				if (val != null)
				{
					PullInv(val, list, seenInv, seenItem, 0);
				}
			}
		}
		catch
		{
		}
		try
		{
			GraphNodeWindow child = container.child;
			if (child != null)
			{
				GraphNodeStorage child2 = child.child;
				if (child2 != null)
				{
					PullNode(child2, list, seenInv, seenItem, 0);
				}
			}
		}
		catch
		{
		}
		try
		{
			GameInventory val2 = ResolveInventory(container);
			if (val2 != null)
			{
				PullInv(val2, list, seenInv, seenItem, 0);
			}
		}
		catch
		{
		}
		return list;
	}

	private static void PullInv(GameInventory inv, List<GameItem> res, HashSet<IntPtr> seenInv, HashSet<IntPtr> seenItem, int depth)
	{
		if (inv == null || depth > 3)
		{
			return;
		}
		IntPtr intPtr = U.Ptr((Il2CppObjectBase)(object)inv);
		if (intPtr == IntPtr.Zero || !seenInv.Add(intPtr))
		{
			return;
		}
		List<GameItem> val = null;
		try
		{
			val = inv.childItems;
		}
		catch
		{
		}
		if (val == null)
		{
			return;
		}
		int num = 0;
		try
		{
			num = val.Count;
		}
		catch
		{
		}
		if (num <= 0 || num > 4000)
		{
			return;
		}
		for (int i = 0; i < num; i++)
		{
			GameItem val2 = null;
			try
			{
				val2 = val[i];
			}
			catch
			{
			}
			if (val2 == null)
			{
				continue;
			}
			IntPtr intPtr2 = U.Ptr((Il2CppObjectBase)(object)val2);
			if (intPtr2 != IntPtr.Zero && seenItem.Add(intPtr2))
			{
				res.Add(val2);
			}
			try
			{
				GameInventory val3 = ResolveInventory(val2);
				if (val3 != null)
				{
					PullInv(val3, res, seenInv, seenItem, depth + 1);
				}
			}
			catch
			{
			}
		}
	}

	private static void PullNode(GraphNodeStorage node, List<GameItem> res, HashSet<IntPtr> seenInv, HashSet<IntPtr> seenItem, int depth)
	{
		if (node == null || depth > 3)
		{
			return;
		}
		try
		{
			List<GraphNodeStorage> children = node.children;
			if (children == null)
			{
				return;
			}
			int count = children.Count;
			if (count > 0 && count <= 400)
			{
				for (int i = 0; i < count; i++)
				{
					PullNode(children[i], res, seenInv, seenItem, depth + 1);
				}
			}
		}
		catch
		{
		}
	}

	public static List<Container> Containers(bool force)
	{
		if (!force && Time.realtimeSinceStartup - _lastScan < 3f)
		{
			return _containers;
		}
		_lastScan = Time.realtimeSinceStartup;
		_containers.Clear();
		RebuildProtected();
		HashSet<IntPtr> seenItems = new HashSet<IntPtr>();
		HashSet<IntPtr> seenInvs = new HashSet<IntPtr>();
		try
		{
			EmporiumEntry instance = EmporiumEntry.Instance;
			if ((Object)(object)instance != (Object)null)
			{
				AddInventory((GameInventory)(object)instance.invElement, seenInvs, seenItems);
				AddInventory((GameInventory)(object)instance.frontInvinvElement, seenInvs, seenItems);
				AddInventory((GameInventory)(object)instance.backInvinvElement, seenInvs, seenItems);
				AddInventory((GameInventory)(object)instance.backInvinvElementCounter, seenInvs, seenItems);
				AddInventory((GameInventory)(object)instance.showcaseElement, seenInvs, seenItems);
				AddInventory((GameInventory)(object)instance.soldElement, seenInvs, seenItems);
				AddInventory((GameInventory)(object)instance.hiddenElement, seenInvs, seenItems);
				AddInventory((GameInventory)(object)instance.bazarLeftinvElement, seenInvs, seenItems);
				AddInventory((GameInventory)(object)instance.trashcanInvElement, seenInvs, seenItems);
				AddInventory((GameInventory)(object)instance.afterhourInventory, seenInvs, seenItems);
				AddInventory((GameInventory)(object)instance.hirelingInv, seenInvs, seenItems);
				AddInventory((GameInventory)(object)instance.responseInventory, seenInvs, seenItems);
				AddItem(instance.selectedBackpack, seenItems);
			}
		}
		catch (Exception ex)
		{
			Core.Log.Warning("[AutoStorage] EmporiumEntry 扫描: " + ex.Message);
		}
		try
		{
			EmporiumEntry instance2 = EmporiumEntry.Instance;
			if ((Object)(object)instance2 != (Object)null)
			{
				List<GameItem> allItems = instance2.GetAllItems();
				if (allItems != null)
				{
					for (int i = 0; i < allItems.Count; i++)
					{
						AddItem(allItems[i], seenItems);
					}
				}
			}
		}
		catch (Exception ex2)
		{
			Core.Log.Warning("[AutoStorage] GetAllItems: " + ex2.Message);
		}
		try
		{
			PlayerStore instance3 = PlayerStore.instance;
			if (instance3 != null)
			{
				List<GameItem> val = instance3.FindAllItem(true);
				if (val != null)
				{
					for (int j = 0; j < val.Count; j++)
					{
						AddItem(val[j], seenItems);
					}
				}
			}
		}
		catch (Exception ex3)
		{
			Core.Log.Warning("[AutoStorage] FindAllItem: " + ex3.Message);
		}
		try
		{
			EmporiumEntry instance4 = EmporiumEntry.Instance;
			if ((Object)(object)instance4 != (Object)null)
			{
				List<GameItem> allOwnedItems = instance4.GetAllOwnedItems(false, true);
				if (allOwnedItems != null)
				{
					for (int k = 0; k < allOwnedItems.Count; k++)
					{
						AddItem(allOwnedItems[k], seenItems);
					}
				}
			}
		}
		catch (Exception ex4)
		{
			Core.Log.Warning("[AutoStorage] GetAllOwnedItems: " + ex4.Message);
		}
		try
		{
			PlayerStore instance5 = PlayerStore.instance;
			if (instance5 != null)
			{
				AddInventory((GameInventory)(object)instance5.gridInv, seenInvs, seenItems);
			}
		}
		catch (Exception ex5)
		{
			Core.Log.Warning("[AutoStorage] gridInv: " + ex5.Message);
		}
		try
		{
			Il2CppReferenceArray<Object> val2 = Resources.FindObjectsOfTypeAll(Il2CppType.Of<TreeNodeRender>());
			if (val2 != null)
			{
				for (int l = 0; l < ((Il2CppArrayBase<Object>)(object)val2).Length; l++)
				{
					Object val3 = ((Il2CppArrayBase<Object>)(object)val2)[l];
					if (val3 == (Object)null)
					{
						continue;
					}
					TreeNodeRender val4 = ((Il2CppObjectBase)val3).TryCast<TreeNodeRender>();
					if ((Object)(object)val4 == (Object)null)
					{
						continue;
					}
					try
					{
						PixelElement element = val4.element;
						if (element != null)
						{
							GameInventory val5 = ((Il2CppObjectBase)element).TryCast<GameInventory>();
							if (val5 != null)
							{
								AddInventory(val5, seenInvs, seenItems);
							}
							else
							{
								GameItem val6 = ((Il2CppObjectBase)element).TryCast<GameItem>();
								if (val6 != null)
								{
									AddItem(val6, seenItems);
								}
							}
						}
					}
					catch
					{
					}
					try
					{
						PixelWindow window = val4.window;
						if (window != null)
						{
							AddInventory(FromWindow(window), seenInvs, seenItems);
						}
					}
					catch
					{
					}
				}
			}
		}
		catch (Exception ex6)
		{
			Core.Log.Warning("[AutoStorage] 场景扫描: " + ex6.Message);
		}
		return _containers;
	}

	private static void AddInventory(GameInventory inv, HashSet<IntPtr> seenInvs, HashSet<IntPtr> seenItems)
	{
		IntPtr intPtr = U.Ptr((Il2CppObjectBase)(object)inv);
		if (intPtr == IntPtr.Zero || !seenInvs.Add(intPtr))
		{
			return;
		}
		Register(inv);
		try
		{
			List<GameItem> childItems = inv.childItems;
			if (childItems == null)
			{
				return;
			}
			int count = childItems.Count;
			if (count <= 4000)
			{
				for (int i = 0; i < count; i++)
				{
					AddItem(childItems[i], seenItems);
				}
			}
		}
		catch
		{
		}
	}

	private static void AddItem(GameItem it, HashSet<IntPtr> seenItems)
	{
		IntPtr intPtr = U.Ptr((Il2CppObjectBase)(object)it);
		if (intPtr == IntPtr.Zero || !seenItems.Add(intPtr) || BlockedId(U.SafeId(it)))
		{
			return;
		}
		TryRegister(it);
		try
		{
			PixelWindow contentWindow = it.contentWindow;
			if (contentWindow != null)
			{
				GameInventory val = FromWindow(contentWindow);
				if (val != null)
				{
					Register(val);
				}
			}
		}
		catch
		{
		}
	}

	private static void TryRegister(GameItem it)
	{
		if (IsContainerItem(it, out var inv))
		{
			Pair(it, inv);
		}
	}

	private static void Register(GameInventory inv)
	{
		if (inv != null)
		{
			GameItem val = null;
			try
			{
				val = inv.GetParentItem();
			}
			catch
			{
			}
			if (val != null)
			{
				TryRegister(val);
			}
		}
	}

	private static void Pair(GameItem it, GameInventory inv)
	{
		for (int i = 0; i < _containers.Count; i++)
		{
			if (U.Ptr((Il2CppObjectBase)(object)_containers[i].Item) == U.Ptr((Il2CppObjectBase)(object)it))
			{
				if (U.Ptr((Il2CppObjectBase)(object)_containers[i].Inv) == IntPtr.Zero)
				{
					_containers[i].Inv = inv;
				}
				return;
			}
		}
		_containers.Add(new Container
		{
			Item = it,
			Inv = inv
		});
	}

	public static List<GameItem> SourceItems(out string report)
	{
		List<GameItem> res = new List<GameItem>();
		HashSet<IntPtr> seen = new HashSet<IntPtr>();
		StringBuilder sb = new StringBuilder();
		Action<string, List<GameItem>> action = delegate(string label, List<GameItem> list)
		{
			int num2 = 0;
			if (list != null)
			{
				for (int j = 0; j < list.Count; j++)
				{
					GameItem val2 = list[j];
					IntPtr intPtr2 = U.Ptr((Il2CppObjectBase)(object)val2);
					if (!(intPtr2 == IntPtr.Zero) && seen.Add(intPtr2))
					{
						res.Add(val2);
						num2++;
					}
				}
			}
			if (num2 > 0)
			{
				sb.Append("\n    物品栏/柜台[").Append(label).Append("] = ")
					.Append(num2);
			}
		};
		try
		{
			EmporiumEntry instance = EmporiumEntry.Instance;
			if ((Object)(object)instance != (Object)null)
			{
				action("invElement", Child((GameInventory)(object)instance.invElement));
				action("frontInv", Child((GameInventory)(object)instance.frontInvinvElement));
				action("backInv", Child((GameInventory)(object)instance.backInvinvElement));
				action("hidden", Child((GameInventory)(object)instance.hiddenElement));
			}
		}
		catch (Exception ex)
		{
			Core.Log.Warning("[AutoStorage] 物品栏: " + ex.Message);
		}
		try
		{
			EmporiumEntry instance2 = EmporiumEntry.Instance;
			if ((Object)(object)instance2 != (Object)null)
			{
				action("backInvCounter", Child((GameInventory)(object)instance2.backInvinvElementCounter));
				List<GameItem> countersItems = instance2.GetCountersItems();
				if (countersItems != null)
				{
					int num = 0;
					for (int i = 0; i < countersItems.Count; i++)
					{
						GameItem val = countersItems[i];
						IntPtr intPtr = U.Ptr((Il2CppObjectBase)(object)val);
						if (!(intPtr == IntPtr.Zero) && seen.Add(intPtr))
						{
							res.Add(val);
							num++;
						}
					}
					if (num > 0)
					{
						sb.Append("\n    谈判柜台[GetCountersItems] = ").Append(num);
					}
				}
			}
		}
		catch (Exception ex2)
		{
			Core.Log.Warning("[AutoStorage] 谈判柜台: " + ex2.Message);
		}
		report = sb.ToString();
		return res;
	}

	private static List<GameItem> Child(GameInventory inv)
	{
		if (inv == null)
		{
			return null;
		}
		try
		{
			return inv.childItems;
		}
		catch
		{
			return null;
		}
	}
}
