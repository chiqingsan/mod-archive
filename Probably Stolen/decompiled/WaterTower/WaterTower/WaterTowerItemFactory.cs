using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;

namespace WaterTower;

public static class WaterTowerItemFactory
{
	private static readonly Dictionary<string, Type> _typeCache = new Dictionary<string, Type>();

	public static Type ResolveType(string simpleName)
	{
		return Resolve(simpleName, "Il2Cpp", gameAssembliesOnly: true);
	}

	public static Type ResolveUnityType(string simpleName)
	{
		return Resolve(simpleName, "UnityEngine", gameAssembliesOnly: false);
	}

	private static Type Resolve(string simpleName, string preferNamespace, bool gameAssembliesOnly)
	{
		string key = preferNamespace + ":" + simpleName;
		if (_typeCache.TryGetValue(key, out var value))
		{
			return value;
		}
		Type type = null;
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly assembly in assemblies)
		{
			string name = assembly.GetName().Name;
			if (name == "WaterTower")
			{
				continue;
			}
			if (gameAssembliesOnly)
			{
				if (!name.StartsWith("Assembly-CSharp", StringComparison.Ordinal))
				{
					continue;
				}
			}
			else if (!name.StartsWith("UnityEngine", StringComparison.Ordinal))
			{
				continue;
			}
			Type[] array;
			try
			{
				array = assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				List<Type> list = new List<Type>();
				Type[] types = ex.Types;
				foreach (Type type2 in types)
				{
					if (type2 != null)
					{
						list.Add(type2);
					}
				}
				array = list.ToArray();
			}
			catch
			{
				continue;
			}
			Type[] array2 = array;
			foreach (Type type3 in array2)
			{
				if (!(type3 == null) && string.Equals(type3.Name, simpleName, StringComparison.Ordinal))
				{
					if (type3.Namespace == preferNamespace)
					{
						type = type3;
						break;
					}
					if (type == null)
					{
						type = type3;
					}
				}
			}
			if (!(type != null) || !(type.Namespace == preferNamespace))
			{
				continue;
			}
			break;
		}
		if (type == null)
		{
			Log.Error("找不到类型: " + simpleName + "（命名空间 " + preferNamespace + "） —— 依赖它的功能本次会全部静默失败（游戏版本变化或被裁剪？）");
		}
		else
		{
			Log.Debug("解析类型 " + simpleName + " -> " + type.FullName + " [" + type.Assembly.GetName().Name + "]");
		}
		_typeCache[key] = type;
		return type;
	}

	public static object CreateWaterTowerItem()
	{
		Log.Section("构造水塔物品  开始");
		if (LazyPatches.InsideSaveEncode)
		{
			Log.Warn("检测到存档编码期现造水塔（游戏对显示元素已销毁物品的替换路径）—— 这只是编码替身，不是玩家新建的");
			Diag.Once("save-mint", "存档编码期现造水塔：这只塔是编码替身。若登记表出现多台\"水塔\"且只有一台有水，即此路径所致（docs/07 第十七节）");
		}
		Type type = ResolveType("ItemDirectory");
		if (type == null)
		{
			Log.Error("构造中止：拿不到 ItemDirectory 类型");
			return null;
		}
		Log.Info("步骤 1/6  调用 ItemDirectory.CreateEmptyItem(\"watertower\")");
		object obj = CallStatic(type, "CreateEmptyItem", new object[1] { "watertower" }, "CreateEmptyItem", new Type[1] { typeof(string) });
		if (obj == null)
		{
			Log.Error("构造中止：CreateEmptyItem 返回 null。可能 ItemDirectory 需要先初始化，或方法名/签名不同。");
			return null;
		}
		Type type2 = obj.GetType();
		Log.Info("步骤 2/2  交给 WaterTowerWindow 按 machine_bay 配方完成构造");
		try
		{
			WaterTowerWindow.BuildAndAttach(obj);
		}
		catch (Exception ex)
		{
			Log.Exception(ex, "内容窗口构建失败（物品本体仍已创建）");
		}
		Log.Success("水塔物品构造流程结束");
		return obj;
	}

	public static object SpawnWaterTower()
	{
		Type type = ResolveType("DirectoryMaster");
		if (type == null)
		{
			Log.Error("SpawnWaterTower：找不到 DirectoryMaster（注册尚未完成？）");
			return null;
		}
		return CallStatic(type, "Item", new object[2] { "watertower", true }, "DirectoryMaster.Item(\"watertower\", true)（注册表路径）");
	}

	public static object SpawnWaterTank()
	{
		if (ItemRegistry.TankRegisteredInto)
		{
			Type type = ResolveType("DirectoryMaster");
			if (type != null)
			{
				object obj = CallStatic(type, "Item", new object[2] { "watertower_tank", true }, "DirectoryMaster.Item(\"watertower_tank\", true)（注册表路径）");
				if (obj != null)
				{
					return obj;
				}
				Log.Warn("watertower_tank 经注册表取物返回 null，退回直造");
			}
		}
		object obj2 = BuildWaterTankItem();
		if (obj2 != null)
		{
			EnsureOwned(obj2, "蓄水箱（直造兜底）");
		}
		return obj2;
	}

	public static object BuildWaterTankItem()
	{
		Type type = ResolveType("ItemDirectory");
		Type type2 = ResolveType("WaterHelper");
		if (type == null || type2 == null)
		{
			Log.Error("蓄水箱构造：类型解析失败");
			return null;
		}
		object obj = CallStatic(type, "CreateEmptyItem", new object[1] { "watertower_tank" }, "CreateEmptyItem(watertower_tank)", new Type[1] { typeof(string) });
		if (obj == null)
		{
			Log.Error("蓄水箱物品创建失败");
			return null;
		}
		Type type3 = obj.GetType();
		CallStatic(type2, "InitLiquidContainerItem", new object[8] { obj, 600000, 0, false, false, false, false, 3 }, "InitLiquidContainerItem(水箱, " + 600000 + "ml)");
		CallInstance(type3, obj, "SetSpriteAndShape", new object[2] { "Items/water_container4", "xl" }, "SetSpriteAndShape(水箱)");
		SetMemberValue(type3, obj, "name", "水塔蓄水箱（600L）");
		object obj2 = CallInstance(type3, obj, "IsTag", new object[1] { "LIQUID_CONTAINER_TAG" }, "IsTag(LIQUID_CONTAINER_TAG)");
		if (!(obj2 is bool) || !(bool)obj2)
		{
			Log.Warn("水箱未带 LIQUID_CONTAINER_TAG —— 手动补打");
			CallInstance(type3, obj, "EnableTag", new object[2] { "LIQUID_CONTAINER_TAG", false }, "EnableTag(LIQUID_CONTAINER_TAG)");
		}
		return obj;
	}

	public static void EnsureOwned(object item, string label)
	{
		if (item == null)
		{
			return;
		}
		if (IsItemDestroyed(item))
		{
			Log.Debug("所有权补齐：跳过已销毁的物品（" + label + "）");
			return;
		}
		try
		{
			Type type = item.GetType();
			object obj = CallInstance(type, item, "IsTag", new object[1] { "IS_OWNED_TAG" }, "IsTag(IS_OWNED_TAG)");
			bool flag = default(bool);
			int num;
			if (obj is bool)
			{
				flag = (bool)obj;
				num = 1;
			}
			else
			{
				num = 0;
			}
			bool flag2 = (byte)((uint)num & (flag ? 1u : 0u)) != 0;
			int num3 = ((GetMemberValue(type, item, "uniqueId") is int num2) ? num2 : 0);
			if (flag2 && num3 != 0)
			{
				return;
			}
			if (!flag2)
			{
				Type type2 = ResolveType("GeneralHelper");
				if (type2 != null)
				{
					CallStatic(type2, "SetItemOwned", new object[2] { item, true }, "SetItemOwned(" + label + ", true)");
				}
			}
			if (num3 == 0)
			{
				object obj2 = ItemRegistry.TryGetStore();
				if (obj2 != null && CallInstance(obj2.GetType(), obj2, "GetUniqueID", null, "PlayerStore.GetUniqueID()") is int num4 && num4 != 0)
				{
					SetMemberValue(type, item, "uniqueId", num4);
				}
			}
			Type type3 = ResolveType("PermitHelper");
			if (type3 != null)
			{
				CallStatic(type3, "OnNewItemCreated", new object[1] { item }, "OnNewItemCreated(" + label + ")");
			}
			Log.Info("所有权补齐：" + label + "（IS_OWNED_TAG=" + flag2 + " → 已确保，uniqueId " + num3 + " → 已确保）");
		}
		catch (Exception ex)
		{
			Log.Exception(ex, "所有权补齐失败（" + label + "）");
		}
	}

	public static bool IsItemDestroyed(object item)
	{
		if (item == null)
		{
			return true;
		}
		try
		{
			Il2CppObjectBase val = (Il2CppObjectBase)((item is Il2CppObjectBase) ? item : null);
			if (val == null)
			{
				return false;
			}
			GameItemElement val2 = val.TryCast<GameItemElement>();
			if (val2 == null)
			{
				return false;
			}
			return val2.IsDestroyed();
		}
		catch
		{
			return false;
		}
	}

	private static object GetMember(Type type, object instance, string name)
	{
		try
		{
			PropertyInfo property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (property != null && property.GetIndexParameters().Length == 0)
			{
				return property.GetValue(instance);
			}
		}
		catch
		{
		}
		try
		{
			FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field != null)
			{
				return field.GetValue(instance);
			}
		}
		catch
		{
		}
		return null;
	}

	public static bool SameRef(object a, object b)
	{
		if (a == null || b == null)
		{
			return a == b;
		}
		try
		{
			Il2CppObjectBase val = (Il2CppObjectBase)((a is Il2CppObjectBase) ? a : null);
			Il2CppObjectBase val2 = (Il2CppObjectBase)((b is Il2CppObjectBase) ? b : null);
			if (val != null && val2 != null)
			{
				return val.Pointer == val2.Pointer;
			}
		}
		catch
		{
		}
		return a == b;
	}

	public static int ToInt(object v)
	{
		if (v is int result)
		{
			return result;
		}
		if (v is long num)
		{
			return (int)num;
		}
		if (v is float num2)
		{
			return (int)num2;
		}
		if (v is double num3)
		{
			return (int)num3;
		}
		return 0;
	}

	public static string Fmt(object v)
	{
		return (v == null) ? "<null>" : v.ToString();
	}

	public static object NewInstance(Type type, object[] args, string label)
	{
		if (type == null)
		{
			Log.Warn("[构造] " + label + " 跳过：类型为 null");
			return null;
		}
		try
		{
			ConstructorInfo constructorInfo = null;
			ConstructorInfo constructorInfo2 = null;
			ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (ConstructorInfo constructorInfo3 in constructors)
			{
				int num = constructorInfo3.GetParameters().Length;
				int num2 = ((args != null) ? args.Length : 0);
				if (num == num2)
				{
					constructorInfo = constructorInfo3;
					break;
				}
				if (num > num2 && (constructorInfo2 == null || num < constructorInfo2.GetParameters().Length))
				{
					constructorInfo2 = constructorInfo3;
				}
			}
			ConstructorInfo constructorInfo4 = constructorInfo ?? constructorInfo2;
			if (constructorInfo4 == null)
			{
				Log.Warn("[构造] " + label + " 失败：没有可用构造函数");
				return null;
			}
			ParameterInfo[] parameters = constructorInfo4.GetParameters();
			object[] array = new object[parameters.Length];
			for (int j = 0; j < parameters.Length; j++)
			{
				object arg = ((args != null && j < args.Length) ? args[j] : DefaultOf(parameters[j].ParameterType));
				array[j] = ConvertForInterop(arg, parameters[j].ParameterType);
			}
			object obj = constructorInfo4.Invoke(array);
			Log.Debug("[构造] " + label + " -> " + ((obj == null) ? "<null>" : obj.GetType().Name));
			return obj;
		}
		catch (Exception ex)
		{
			Log.Exception(ex, "[构造] " + label + " 抛异常");
			return null;
		}
	}

	private static object DefaultOf(Type t)
	{
		if (t == null)
		{
			return null;
		}
		if (t.IsValueType)
		{
			try
			{
				return Activator.CreateInstance(t);
			}
			catch
			{
				return null;
			}
		}
		return null;
	}

	public static object GetMemberValue(Type type, object instance, string name)
	{
		return GetMember(type, instance, name);
	}

	public static object GetMemberRobust(Type type, object instance, string name, out string via)
	{
		via = null;
		if (type == null || instance == null)
		{
			return null;
		}
		string[] array = new string[3]
		{
			name,
			"_" + name + "_k__BackingField",
			"<" + name + ">k__BackingField"
		};
		string[] array2 = array;
		foreach (string text in array2)
		{
			try
			{
				PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (PropertyInfo propertyInfo in properties)
				{
					if (propertyInfo.Name != text || propertyInfo.GetIndexParameters().Length != 0)
					{
						continue;
					}
					object value = propertyInfo.GetValue(instance);
					if (value != null)
					{
						via = "属性 " + text;
						return value;
					}
					if (via == null)
					{
						via = "属性 " + text + "（值为 null）";
					}
					break;
				}
			}
			catch (Exception ex)
			{
				Log.Debug("   读属性 " + text + " 抛异常：" + ex.GetType().Name);
				if (via == null)
				{
					via = "属性 " + text + " 抛 " + ex.GetType().Name;
				}
			}
			try
			{
				FieldInfo field = type.GetField(text, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (field != null)
				{
					object value2 = field.GetValue(instance);
					if (value2 != null)
					{
						via = "字段 " + text;
						return value2;
					}
					if (via == null)
					{
						via = "字段 " + text + "（值为 null）";
					}
				}
			}
			catch (Exception ex2)
			{
				Log.Debug("   读字段 " + text + " 抛异常：" + ex2.GetType().Name);
			}
		}
		return null;
	}

	public static bool SetMemberValue(Type type, object instance, string name, object value)
	{
		try
		{
			PropertyInfo property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (property != null && property.CanWrite)
			{
				property.SetValue(instance, value, null);
				return true;
			}
			FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field != null)
			{
				field.SetValue(instance, value);
				return true;
			}
		}
		catch (Exception ex)
		{
			Log.Exception(ex, "写入成员 " + name + " 失败");
		}
		return false;
	}

	public static bool TryRemoveKey(object dictionary, object key)
	{
		if (dictionary == null)
		{
			return false;
		}
		try
		{
			MethodInfo methodInfo = PickMethod(dictionary.GetType(), "Remove", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, new object[1] { key });
			if (methodInfo == null)
			{
				return false;
			}
			object obj = methodInfo.Invoke(dictionary, AdaptArgs(methodInfo, new object[1] { key }));
			return obj is bool && (bool)obj;
		}
		catch (Exception ex)
		{
			Log.Debug("   删除字典键失败（" + ex.GetType().Name + "）");
			return false;
		}
	}

	public static bool SetIndexedValue(Type type, object instance, object key, object value)
	{
		try
		{
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (PropertyInfo propertyInfo in properties)
			{
				ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
				if (indexParameters.Length == 1 && propertyInfo.CanWrite)
				{
					propertyInfo.SetValue(instance, new object[1] { value }, new object[1] { key });
					return true;
				}
			}
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (MethodInfo methodInfo in methods)
			{
				if (!(methodInfo.Name != "set_Item") || !(methodInfo.Name != "Set_Item"))
				{
					ParameterInfo[] parameters = methodInfo.GetParameters();
					if (parameters.Length == 2)
					{
						methodInfo.Invoke(instance, new object[2] { key, value });
						return true;
					}
				}
			}
			Log.Warn("SetIndexedValue：类型 " + type.Name + " 上没有可写的索引器");
		}
		catch (Exception ex)
		{
			Log.Exception(ex, "SetIndexedValue 失败（" + type.Name + "）");
		}
		return false;
	}

	public static object CallStatic(Type type, string method, object[] args, string label, Type[] wantParamTypes = null)
	{
		if (type == null)
		{
			Log.Warn("[调用] " + label + " 跳过：类型为 null");
			return null;
		}
		try
		{
			MethodInfo methodInfo = PickMethod(type, method, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, args, wantParamTypes);
			if (methodInfo == null)
			{
				Log.Debug("[调用] " + label + " 失败：找不到匹配的静态方法 " + method);
				ReportCandidates(type, method);
				Diag.Once("call-miss-" + type.Name + "." + method, "[调用缺方法] " + type.Name + "." + method + " 找不到匹配的静态重载（label=" + label + "）—— 可能是游戏更新改了签名");
				return null;
			}
			object obj = methodInfo.Invoke(null, AdaptArgs(methodInfo, args));
			Log.Debug("[调用] " + label + " -> " + ((obj == null) ? "<null>" : obj.ToString()));
			return obj;
		}
		catch (Exception ex)
		{
			Log.Debug("[调用] " + label + " 抛异常：" + ex.GetType().Name);
			Diag.Once("call-ex-" + type.Name + "." + method, "[调用异常] " + type.Name + "." + method + "（label=" + label + "）<err>" + ex.GetType().Name + "：" + ex.Message);
			return null;
		}
	}

	public static bool TryCallInstance(Type type, object instance, string method, object[] args, out object result, Type[] wantParamTypes = null)
	{
		result = null;
		if (type == null || instance == null)
		{
			return false;
		}
		try
		{
			MethodInfo methodInfo = PickMethod(type, method, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, args, wantParamTypes);
			if (methodInfo == null)
			{
				Log.Debug("[探测] 找不到实例方法 " + method);
				return false;
			}
			result = methodInfo.Invoke(instance, AdaptArgs(methodInfo, args));
			return true;
		}
		catch (Exception ex)
		{
			Log.Debug("[探测] " + method + " 调用失败：" + ex.GetType().Name);
			return false;
		}
	}

	public static object CallInstance(Type type, object instance, string method, object[] args, string label, Type[] wantParamTypes = null)
	{
		if (type == null || instance == null)
		{
			Log.Warn("[调用] " + label + " 跳过：type 或 instance 为 null");
			return null;
		}
		try
		{
			MethodInfo methodInfo = PickMethod(type, method, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, args, wantParamTypes);
			if (methodInfo == null)
			{
				Log.Debug("[调用] " + label + " 失败：找不到匹配的实例方法 " + method);
				ReportCandidates(type, method);
				Diag.Once("call-miss-" + type.Name + "." + method, "[调用缺方法] " + type.Name + "." + method + " 找不到匹配的实例重载（label=" + label + "）—— 可能是游戏更新改了签名");
				return null;
			}
			object obj = methodInfo.Invoke(instance, AdaptArgs(methodInfo, args));
			Log.Debug("[调用] " + label + " -> " + ((obj == null) ? "<null>" : obj.ToString()));
			return obj;
		}
		catch (Exception ex)
		{
			Log.Debug("[调用] " + label + " 抛异常：" + ex.GetType().Name);
			Diag.Once("call-ex-" + type.Name + "." + method, "[调用异常] " + type.Name + "." + method + "（label=" + label + "）<err>" + ex.GetType().Name + "：" + ex.Message);
			return null;
		}
	}

	private static MethodInfo PickMethod(Type type, string name, BindingFlags flags, object[] args, Type[] wantParamTypes = null)
	{
		int num = ((args != null) ? args.Length : 0);
		MethodInfo methodInfo = null;
		MethodInfo methodInfo2 = null;
		MethodInfo methodInfo3 = null;
		int num2 = int.MaxValue;
		MethodInfo[] methods = type.GetMethods(flags);
		foreach (MethodInfo methodInfo4 in methods)
		{
			if (methodInfo4.Name != name || methodInfo4.IsGenericMethodDefinition)
			{
				continue;
			}
			ParameterInfo[] parameters = methodInfo4.GetParameters();
			int num3 = parameters.Length;
			if (num3 == num)
			{
				if (methodInfo2 == null && ParamTypesMatch(parameters, wantParamTypes))
				{
					methodInfo2 = methodInfo4;
				}
				if (methodInfo == null)
				{
					methodInfo = methodInfo4;
				}
			}
			else if (num3 < num && num3 < num2)
			{
				methodInfo3 = methodInfo4;
				num2 = num3;
			}
		}
		return methodInfo2 ?? methodInfo ?? methodInfo3;
	}

	private static bool ParamTypesMatch(ParameterInfo[] ps, Type[] want)
	{
		if (want == null || want.Length == 0)
		{
			return true;
		}
		int num = ((ps.Length < want.Length) ? ps.Length : want.Length);
		for (int i = 0; i < num; i++)
		{
			if (!(want[i] == null) && ps[i].ParameterType != want[i])
			{
				return false;
			}
		}
		return true;
	}

	private static object[] AdaptArgs(MethodInfo m, object[] args)
	{
		ParameterInfo[] parameters = m.GetParameters();
		int num = ((args != null) ? args.Length : 0);
		int num2 = ((parameters.Length < num) ? parameters.Length : num);
		object[] array = new object[parameters.Length];
		for (int i = 0; i < parameters.Length; i++)
		{
			object arg = ((args != null && i < args.Length) ? args[i] : null);
			array[i] = ConvertForInterop(arg, parameters[i].ParameterType);
		}
		return array;
	}

	private static object ConvertForInterop(object arg, Type paramType)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		if (arg == null || paramType == null)
		{
			return arg;
		}
		string name = paramType.Name;
		if (arg is string[] && name == "Il2CppStringArray")
		{
			try
			{
				return (object)new Il2CppStringArray((string[])arg);
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "转换 string[] → Il2CppStringArray 失败");
				return arg;
			}
		}
		if (arg is byte[] && name.StartsWith("Il2CppStructArray"))
		{
			try
			{
				return new Il2CppStructArray<byte>((byte[])arg);
			}
			catch (Exception ex2)
			{
				Log.Exception(ex2, "转换 byte[] → Il2CppStructArray<byte> 失败");
				return arg;
			}
		}
		if (arg is List<string> && name == "List`1")
		{
			try
			{
				List<string> val = new List<string>();
				foreach (string item in (List<string>)arg)
				{
					val.Add(item);
				}
				return val;
			}
			catch (Exception ex3)
			{
				Log.Exception(ex3, "转换 List<string> → Il2Cpp List 失败");
				return arg;
			}
		}
		if (arg is Il2CppObjectBase && typeof(Il2CppObjectBase).IsAssignableFrom(paramType) && !paramType.IsInstanceOfType(arg))
		{
			try
			{
				MethodInfo methodInfo = null;
				MethodInfo[] methods = arg.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
				foreach (MethodInfo methodInfo2 in methods)
				{
					if (methodInfo2.Name == "TryCast" && methodInfo2.IsGenericMethodDefinition && methodInfo2.GetParameters().Length == 0)
					{
						methodInfo = methodInfo2;
						break;
					}
				}
				if (methodInfo == null)
				{
					Log.Warn("   找不到可用的 TryCast 泛型方法，跳过接口转换 " + name);
					return arg;
				}
				object obj = methodInfo.MakeGenericMethod(paramType).Invoke(arg, null);
				if (obj != null)
				{
					return obj;
				}
				Log.Warn("   TryCast 到 " + name + " 返回 null");
			}
			catch (Exception ex4)
			{
				Log.Exception(ex4, "TryCast 到 " + name + " 失败");
			}
		}
		return arg;
	}

	private static void ReportCandidates(Type type, string name)
	{
		int num = 0;
		try
		{
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (MethodInfo methodInfo in methods)
			{
				if (methodInfo.Name != name)
				{
					continue;
				}
				StringBuilder stringBuilder = new StringBuilder();
				ParameterInfo[] parameters = methodInfo.GetParameters();
				foreach (ParameterInfo parameterInfo in parameters)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(parameterInfo.ParameterType.Name).Append(' ').Append(parameterInfo.Name);
				}
				Log.Info("      可用重载: " + methodInfo.ReturnType.Name + " " + name + "(" + stringBuilder?.ToString() + ")");
				num++;
			}
		}
		catch (Exception ex)
		{
			Log.Exception(ex, "列出重载失败");
		}
		if (num == 0)
		{
			Log.Info("      该类型上没有名为 " + name + " 的任何方法");
		}
	}
}
