using System;
using System.Reflection;
using UnityEngine;

namespace Utilities;

public static class ReflectionHelper
{
	private const string TAG = "ReflectionHelper";

	public static T GetInstanceFieldValue<T>(object obj, string fieldName, T defaultValue = default(T), BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
	{
		try
		{
			if (obj == null)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 获取实例字段 '" + fieldName + "' 失败：目标对象为null"));
				return defaultValue;
			}
			Type type = obj.GetType();
			FieldInfo field = type.GetField(fieldName, bindingFlags);
			if (field == null)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 在类型 '" + type.Name + "' 中未找到实例字段 '" + fieldName + "'"));
				return defaultValue;
			}
			if (field.IsStatic)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 字段 '" + type.Name + "." + fieldName + "' 是静态字段，请使用 GetStaticFieldValue 方法"));
				return defaultValue;
			}
			object value = field.GetValue(obj);
			if (value is T val)
			{
				Debug.Log((object)string.Format("[{0}] 成功获取实例字段 '{1}.{2}' = {3}", "ReflectionHelper", type.Name, fieldName, val));
				return val;
			}
			Debug.LogWarning((object)("[ReflectionHelper] 实例字段 '" + type.Name + "." + fieldName + "' 类型不匹配。期望: " + typeof(T).Name + ", 实际: " + (value?.GetType().Name ?? "null")));
			return defaultValue;
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("[ReflectionHelper] 获取实例字段 '" + fieldName + "' 时发生异常: " + ex.Message + "\n" + ex.StackTrace));
			return defaultValue;
		}
	}

	public static bool SetInstanceFieldValue(object obj, string fieldName, object value, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
	{
		try
		{
			if (obj == null)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 设置实例字段 '" + fieldName + "' 失败：目标对象为null"));
				return false;
			}
			Type type = obj.GetType();
			FieldInfo field = type.GetField(fieldName, bindingFlags);
			if (field == null)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 在类型 '" + type.Name + "' 中未找到实例字段 '" + fieldName + "'"));
				return false;
			}
			if (field.IsStatic)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 字段 '" + type.Name + "." + fieldName + "' 是静态字段，请使用 SetStaticFieldValue 方法"));
				return false;
			}
			if (field.IsInitOnly || field.IsLiteral)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 实例字段 '" + type.Name + "." + fieldName + "' 是只读字段"));
				return false;
			}
			object obj2 = ConvertValue(value, field.FieldType);
			field.SetValue(obj, obj2);
			Debug.Log((object)string.Format("[{0}] 成功设置实例字段 '{1}.{2}' = {3}", "ReflectionHelper", type.Name, fieldName, obj2));
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("[ReflectionHelper] 设置实例字段 '" + fieldName + "' 时发生异常: " + ex.Message + "\n" + ex.StackTrace));
			return false;
		}
	}

	public static T GetInstancePropertyValue<T>(object obj, string propertyName, T defaultValue = default(T), BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
	{
		try
		{
			if (obj == null)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 获取实例属性 '" + propertyName + "' 失败：目标对象为null"));
				return defaultValue;
			}
			Type type = obj.GetType();
			PropertyInfo property = type.GetProperty(propertyName, bindingFlags);
			if (property == null)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 在类型 '" + type.Name + "' 中未找到实例属性 '" + propertyName + "'"));
				return defaultValue;
			}
			if (!property.CanRead)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 实例属性 '" + type.Name + "." + propertyName + "' 不可读"));
				return defaultValue;
			}
			MethodInfo getMethod = property.GetGetMethod(nonPublic: true);
			if ((object)getMethod != null && getMethod.IsStatic)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 属性 '" + type.Name + "." + propertyName + "' 是静态属性，请使用 GetStaticPropertyValue 方法"));
				return defaultValue;
			}
			object value = property.GetValue(obj);
			if (value is T val)
			{
				Debug.Log((object)string.Format("[{0}] 成功获取实例属性 '{1}.{2}' = {3}", "ReflectionHelper", type.Name, propertyName, val));
				return val;
			}
			Debug.LogWarning((object)("[ReflectionHelper] 实例属性 '" + type.Name + "." + propertyName + "' 类型不匹配。期望: " + typeof(T).Name + ", 实际: " + (value?.GetType().Name ?? "null")));
			return defaultValue;
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("[ReflectionHelper] 获取实例属性 '" + propertyName + "' 时发生异常: " + ex.Message + "\n" + ex.StackTrace));
			return defaultValue;
		}
	}

	public static object InvokeInstanceMethod(object obj, string methodName, object[] parameters = null, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
	{
		try
		{
			if (obj == null)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 调用实例方法 '" + methodName + "' 失败：目标对象为null"));
				return null;
			}
			Type type = obj.GetType();
			MethodInfo method = type.GetMethod(methodName, bindingFlags);
			if (method == null)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 在类型 '" + type.Name + "' 中未找到实例方法 '" + methodName + "'"));
				return null;
			}
			if (method.IsStatic)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 方法 '" + type.Name + "." + methodName + "' 是静态方法，请使用 InvokeStaticMethod 方法"));
				return null;
			}
			object result = method.Invoke(obj, parameters);
			Debug.Log((object)("[ReflectionHelper] 成功调用实例方法 '" + type.Name + "." + methodName + "'"));
			return result;
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("[ReflectionHelper] 调用实例方法 '" + methodName + "' 时发生异常: " + ex.Message + "\n" + ex.StackTrace));
			return null;
		}
	}

	public static T GetStaticFieldValue<T>(Type type, string fieldName, T defaultValue = default(T), BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
	{
		try
		{
			if (type == null)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 获取静态字段 '" + fieldName + "' 失败：目标类型为null"));
				return defaultValue;
			}
			FieldInfo field = type.GetField(fieldName, bindingFlags);
			if (field == null)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 在类型 '" + type.Name + "' 中未找到静态字段 '" + fieldName + "'"));
				return defaultValue;
			}
			if (!field.IsStatic)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 字段 '" + type.Name + "." + fieldName + "' 不是静态字段"));
				return defaultValue;
			}
			object value = field.GetValue(null);
			if (value is T val)
			{
				Debug.Log((object)string.Format("[{0}] 成功获取静态字段 '{1}.{2}' = {3}", "ReflectionHelper", type.Name, fieldName, val));
				return val;
			}
			Debug.LogWarning((object)("[ReflectionHelper] 静态字段 '" + type.Name + "." + fieldName + "' 类型不匹配。期望: " + typeof(T).Name + ", 实际: " + (value?.GetType().Name ?? "null")));
			return defaultValue;
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("[ReflectionHelper] 获取静态字段 '" + fieldName + "' 时发生异常: " + ex.Message + "\n" + ex.StackTrace));
			return defaultValue;
		}
	}

	public static bool SetStaticFieldValue(Type type, string fieldName, object value, BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
	{
		try
		{
			if (type == null)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 设置静态字段 '" + fieldName + "' 失败：目标类型为null"));
				return false;
			}
			FieldInfo field = type.GetField(fieldName, bindingFlags);
			if (field == null)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 在类型 '" + type.Name + "' 中未找到静态字段 '" + fieldName + "'"));
				return false;
			}
			if (!field.IsStatic)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 字段 '" + type.Name + "." + fieldName + "' 不是静态字段"));
				return false;
			}
			if (field.IsInitOnly || field.IsLiteral)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 静态字段 '" + type.Name + "." + fieldName + "' 是只读字段"));
				return false;
			}
			object obj = ConvertValue(value, field.FieldType);
			field.SetValue(null, obj);
			Debug.Log((object)string.Format("[{0}] 成功设置静态字段 '{1}.{2}' = {3}", "ReflectionHelper", type.Name, fieldName, obj));
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("[ReflectionHelper] 设置静态字段 '" + fieldName + "' 时发生异常: " + ex.Message + "\n" + ex.StackTrace));
			return false;
		}
	}

	public static T GetStaticPropertyValue<T>(Type type, string propertyName, T defaultValue = default(T), BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
	{
		try
		{
			if (type == null)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 获取静态属性 '" + propertyName + "' 失败：目标类型为null"));
				return defaultValue;
			}
			PropertyInfo property = type.GetProperty(propertyName, bindingFlags);
			if (property == null)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 在类型 '" + type.Name + "' 中未找到静态属性 '" + propertyName + "'"));
				return defaultValue;
			}
			if (!property.CanRead)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 静态属性 '" + type.Name + "." + propertyName + "' 不可读"));
				return defaultValue;
			}
			MethodInfo getMethod = property.GetGetMethod(nonPublic: true);
			if ((object)getMethod == null || !getMethod.IsStatic)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 属性 '" + type.Name + "." + propertyName + "' 不是静态属性"));
				return defaultValue;
			}
			object value = property.GetValue(null);
			if (value is T val)
			{
				Debug.Log((object)string.Format("[{0}] 成功获取静态属性 '{1}.{2}' = {3}", "ReflectionHelper", type.Name, propertyName, val));
				return val;
			}
			Debug.LogWarning((object)("[ReflectionHelper] 静态属性 '" + type.Name + "." + propertyName + "' 类型不匹配。期望: " + typeof(T).Name + ", 实际: " + (value?.GetType().Name ?? "null")));
			return defaultValue;
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("[ReflectionHelper] 获取静态属性 '" + propertyName + "' 时发生异常: " + ex.Message + "\n" + ex.StackTrace));
			return defaultValue;
		}
	}

	public static object InvokeStaticMethod(Type type, string methodName, object[] parameters = null, BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
	{
		try
		{
			if (type == null)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 调用静态方法 '" + methodName + "' 失败：目标类型为null"));
				return null;
			}
			MethodInfo method = type.GetMethod(methodName, bindingFlags);
			if (method == null)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 在类型 '" + type.Name + "' 中未找到静态方法 '" + methodName + "'"));
				return null;
			}
			if (!method.IsStatic)
			{
				Debug.LogWarning((object)("[ReflectionHelper] 方法 '" + type.Name + "." + methodName + "' 不是静态方法"));
				return null;
			}
			object result = method.Invoke(null, parameters);
			Debug.Log((object)("[ReflectionHelper] 成功调用静态方法 '" + type.Name + "." + methodName + "'"));
			return result;
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("[ReflectionHelper] 调用静态方法 '" + methodName + "' 时发生异常: " + ex.Message + "\n" + ex.StackTrace));
			return null;
		}
	}

	private static object ConvertValue(object value, Type targetType)
	{
		if (value == null)
		{
			return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
		}
		if (targetType.IsInstanceOfType(value))
		{
			return value;
		}
		try
		{
			if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				Type underlyingType = Nullable.GetUnderlyingType(targetType);
				return Convert.ChangeType(value, underlyingType);
			}
			return Convert.ChangeType(value, targetType);
		}
		catch (Exception ex)
		{
			Debug.LogError((object)string.Format("[{0}] 类型转换失败: 从 {1} 到 {2}, 值: {3}. 异常: {4}", "ReflectionHelper", value.GetType().Name, targetType.Name, value, ex.Message));
			throw;
		}
	}

	public static bool HasField(Type type, string fieldName, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
	{
		return type.GetField(fieldName, bindingFlags) != null;
	}

	public static bool HasProperty(Type type, string propertyName, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
	{
		return type.GetProperty(propertyName, bindingFlags) != null;
	}

	public static bool HasMethod(Type type, string methodName, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
	{
		return type.GetMethod(methodName, bindingFlags) != null;
	}
}
