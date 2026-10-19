using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Il2Cpp;
using Il2CppInterop.Runtime;
using MelonLoader;

namespace WaterTower;

public static class ItemRegistry
{
	private static bool _registered;

	public static bool TankRegisteredInto { get; private set; }

	public static void RegisterInto(object directory)
	{
		if (_registered || directory == null)
		{
			return;
		}
		_registered = true;
		try
		{
			object obj = BuildFactoryDelegate(directory, "CreateWaterTowerItem");
			if (obj == null || !CallAdd(directory, "watertower", obj))
			{
				MelonLogger.Error("[WaterTower] 水塔工厂注册失败");
				_registered = false;
				return;
			}
			object obj2 = BuildFactoryDelegate(directory, "CreateWaterTankItem");
			if (obj2 == null || !CallAdd(directory, "watertower_tank", obj2))
			{
				MelonLogger.Error("[WaterTower] 蓄水箱工厂注册失败（水塔本体不受影响）");
				TankRegisteredInto = false;
			}
			else
			{
				TankRegisteredInto = true;
			}
		}
		catch (Exception ex)
		{
			_registered = false;
			MelonLogger.Error("[WaterTower] 物品注册异常：" + ex);
		}
	}

	public static object TryGetStore()
	{
		try
		{
			return PlayerStore.Instance;
		}
		catch
		{
			return null;
		}
	}

	private static object BuildFactoryDelegate(object directory, string factoryMethodName)
	{
		Type type = null;
		MethodInfo[] methods = directory.GetType().GetMethods();
		foreach (MethodInfo methodInfo in methods)
		{
			if (!(methodInfo.Name != "Add"))
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
				if (parameters.Length == 2 && parameters[0].ParameterType == typeof(string))
				{
					type = parameters[1].ParameterType;
					break;
				}
			}
		}
		if (type == null)
		{
			MelonLogger.Error("[WaterTower] 目录上找不到 Add(string, Func<GameItem>)");
			return null;
		}
		MethodInfo method = typeof(ItemRegistry).GetMethod(factoryMethodName, BindingFlags.Static | BindingFlags.Public);
		if (method == null)
		{
			MelonLogger.Error("[WaterTower] 找不到工厂方法 " + factoryMethodName);
			return null;
		}
		Delegate @delegate = Delegate.CreateDelegate(typeof(Func<GameItem>), method);
		try
		{
			MethodInfo[] methods2 = typeof(DelegateSupport).GetMethods(BindingFlags.Static | BindingFlags.Public);
			foreach (MethodInfo methodInfo2 in methods2)
			{
				if (methodInfo2.Name != "ConvertDelegate" || !methodInfo2.IsGenericMethodDefinition)
				{
					continue;
				}
				ParameterInfo[] parameters2 = methodInfo2.GetParameters();
				if (parameters2.Length == 1 && typeof(Delegate).IsAssignableFrom(parameters2[0].ParameterType))
				{
					object obj = methodInfo2.MakeGenericMethod(type).Invoke(null, new object[1] { @delegate });
					if (obj != null)
					{
						return obj;
					}
				}
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Error("[WaterTower] ConvertDelegate 失败：" + ex.Message);
		}
		try
		{
			ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (ConstructorInfo constructorInfo in constructors)
			{
				ParameterInfo[] parameters3 = constructorInfo.GetParameters();
				if (parameters3.Length == 1 && typeof(Delegate).IsAssignableFrom(parameters3[0].ParameterType))
				{
					return constructorInfo.Invoke(new object[1] { @delegate });
				}
			}
			return Activator.CreateInstance(type, @delegate);
		}
		catch (Exception ex2)
		{
			MelonLogger.Error("[WaterTower] 委托兜底构造失败：" + ex2.Message);
			return null;
		}
	}

	private static bool CallAdd(object directory, string id, object del)
	{
		MethodInfo[] methods = directory.GetType().GetMethods();
		foreach (MethodInfo methodInfo in methods)
		{
			if (methodInfo.Name != "Add")
			{
				continue;
			}
			ParameterInfo[] parameters = methodInfo.GetParameters();
			if (parameters.Length == 2 && parameters[0].ParameterType == typeof(string))
			{
				try
				{
					object obj = methodInfo.Invoke(directory, new object[2] { id, del });
					return !(obj is bool) || (bool)obj;
				}
				catch (Exception ex)
				{
					MelonLogger.Error("[WaterTower] Add(\"" + id + "\") 异常：" + ex.Message);
					return false;
				}
			}
		}
		return false;
	}

	public static GameItem CreateWaterTowerItem()
	{
		object obj = WaterTowerItemFactory.CreateWaterTowerItem();
		GameItem val = (GameItem)((obj is GameItem) ? obj : null);
		if (val == null)
		{
			MelonLogger.Error("[WaterTower] 构造结果不是 GameItem");
		}
		return val;
	}

	public static GameItem CreateWaterTankItem()
	{
		object obj = WaterTowerItemFactory.BuildWaterTankItem();
		GameItem val = (GameItem)((obj is GameItem) ? obj : null);
		if (val == null)
		{
			MelonLogger.Error("[WaterTower] 蓄水箱构造结果不是 GameItem");
		}
		return val;
	}

	public static List<object> EnumerateIl2Cpp(object collection)
	{
		List<object> list = new List<object>();
		if (collection == null)
		{
			return list;
		}
		try
		{
			Type type = collection.GetType();
			MethodInfo methodInfo = null;
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
			foreach (MethodInfo methodInfo2 in methods)
			{
				if (methodInfo2.Name == "GetEnumerator" && methodInfo2.GetParameters().Length == 0 && !methodInfo2.IsGenericMethodDefinition)
				{
					methodInfo = methodInfo2;
					break;
				}
			}
			if (methodInfo != null)
			{
				object obj = methodInfo.Invoke(collection, null);
				if (obj != null)
				{
					Type type2 = obj.GetType();
					MethodInfo methodInfo3 = null;
					MethodInfo[] methods2 = type2.GetMethods(BindingFlags.Instance | BindingFlags.Public);
					foreach (MethodInfo methodInfo4 in methods2)
					{
						if (methodInfo4.Name == "MoveNext" && methodInfo4.GetParameters().Length == 0 && methodInfo4.ReturnType == typeof(bool))
						{
							methodInfo3 = methodInfo4;
							break;
						}
					}
					PropertyInfo property = type2.GetProperty("Current");
					if (methodInfo3 != null && property != null)
					{
						int num = 0;
						while ((bool)methodInfo3.Invoke(obj, null) && ++num <= 10000)
						{
							list.Add(property.GetValue(obj));
						}
						return list;
					}
				}
			}
			if (collection is IEnumerable enumerable)
			{
				foreach (object item in enumerable)
				{
					list.Add(item);
				}
			}
		}
		catch
		{
		}
		return list;
	}
}
