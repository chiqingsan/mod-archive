using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Il2Cpp;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace WaterTower;

public static class ModSprite
{
	public const string EmbeddedResourceName = "WaterTower.assets.WaterTower.png";

	private static bool _attempted;

	private static bool _injected;

	private static string _pngPath;

	private static Sprite _sprite;

	private static Texture2D _texture;

	private static byte[] _pngBytes;

	private static float _ppu;

	public static bool Injected => _injected;

	public static string AtlasPath => _injected ? "Items/watertower_mod" : "Items/items_tool2";

	public static string SpriteName => _injected ? "water_tower" : "machine_bay0";

	public static void Ensure()
	{
		if ((_injected && IsAlive(_sprite)) || (_attempted && !_injected))
		{
			return;
		}
		_attempted = true;
		try
		{
			if (_pngBytes == null)
			{
				string text = ResolvePngPath();
				if (text != null)
				{
					_pngPath = text + "（外部文件覆盖）";
					_pngBytes = File.ReadAllBytes(text);
				}
				else
				{
					_pngBytes = ReadEmbeddedPng();
					if (_pngBytes != null)
					{
						_pngPath = "dll 内置资源 WaterTower.assets.WaterTower.png";
					}
				}
				if (_pngBytes == null)
				{
					Log.Info("自定义贴图：dll 里没有内置图，也没找到 " + string.Join(" / ", WaterTowerConfig.SpriteFileNames) + "（可放在游戏 Mods 目录做外部覆盖）→ 使用原版占位图 Items/items_tool2/machine_bay0");
					return;
				}
			}
			if (!Build())
			{
				Log.Warn("自定义贴图：生成失败（" + _pngPath + "），使用原版占位图");
				return;
			}
			if (!InjectIntoAtlasCache(_sprite))
			{
				Log.Warn("自定义贴图：写入图集缓存失败（ResolveSpriteByName 通道仍可用）");
			}
			else
			{
				VerifyThroughGameApi();
			}
			_injected = true;
			float num = (float)((Texture)_texture).width / _ppu / 0.16f;
			float num2 = (float)((Texture)_texture).height / _ppu / 0.16f;
			Log.Success("自定义贴图已就绪：" + _pngPath + "（" + ((Texture)_texture).width + "x" + ((Texture)_texture).height + "px，key=water_tower，ppu=" + _ppu.ToString("0.#") + "）");
			Log.Info("   贴合校验：图片按当前 ppu 折算 = " + num.ToString("0.##") + " 格宽 x " + num2.ToString("0.##") + " 格高（占格 " + 3 + "x" + 4 + "；两者一致即正好铺满）");
		}
		catch (Exception ex)
		{
			Log.Exception(ex, "自定义贴图处理失败（回退到原版占位图）");
		}
	}

	private static bool Build()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Texture2D val = new Texture2D(2, 2, (TextureFormat)4, false);
			if (!LoadPng(val, _pngBytes))
			{
				return false;
			}
			_texture = ResizeToCellGrid(val);
			((Texture)_texture).filterMode = (FilterMode)0;
			((Texture)_texture).wrapMode = (TextureWrapMode)1;
			((Texture)_texture).anisoLevel = 0;
			_texture.Apply(false, false);
			try
			{
				Object.DontDestroyOnLoad((Object)(object)_texture);
			}
			catch (Exception ex)
			{
				Log.Debug("贴图 DontDestroyOnLoad 失败（继续）：" + ex.GetType().Name);
			}
			float num = 0.48f;
			_ppu = ((num > 0f) ? ((float)((Texture)_texture).width / num) : 100f);
			_sprite = Sprite.Create(_texture, new Rect(0f, 0f, (float)((Texture)_texture).width, (float)((Texture)_texture).height), new Vector2(0.5f, 0f), _ppu, 0u, (SpriteMeshType)0);
			if ((Object)(object)_texture != (Object)(object)val && (Object)(object)val != (Object)null)
			{
				try
				{
					Object.Destroy((Object)(object)val);
				}
				catch
				{
				}
			}
			return (Object)(object)_sprite != (Object)null;
		}
		catch (Exception ex2)
		{
			Log.Exception(ex2, "生成 Sprite 失败");
			return false;
		}
	}

	private static Texture2D ResizeToCellGrid(Texture2D src)
	{
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		int num = 48;
		int num2 = 64;
		if (((Texture)src).width == num && ((Texture)src).height == num2)
		{
			return src;
		}
		Log.Info("自定义贴图：原图 " + ((Texture)src).width + "x" + ((Texture)src).height + " → 缩放到占格像素 " + num + "x" + num2 + "（每格 " + 16 + "px；命中矩形与反推占格必须用它对齐）");
		RenderTexture val = null;
		RenderTexture active = RenderTexture.active;
		try
		{
			val = RenderTexture.GetTemporary(num, num2, 0, (RenderTextureFormat)0);
			Graphics.Blit((Texture)(object)src, val);
			RenderTexture.active = val;
			Texture2D val2 = new Texture2D(num, num2, (TextureFormat)4, false);
			val2.ReadPixels(new Rect(0f, 0f, (float)num, (float)num2), 0, 0);
			val2.Apply(false, false);
			return val2;
		}
		catch (Exception ex)
		{
			Log.Warn("自定义贴图：缩放到 " + num + "x" + num2 + " 失败（" + ex.GetType().Name + "）→ 保留原图（命中矩形会偏大，交互可能异常）");
			return src;
		}
		finally
		{
			RenderTexture.active = active;
			if ((Object)(object)val != (Object)null)
			{
				RenderTexture.ReleaseTemporary(val);
			}
		}
	}

	private static bool IsAlive(Sprite sp)
	{
		try
		{
			if ((Object)(object)sp == (Object)null || (Object)(object)sp.texture == (Object)null)
			{
				return false;
			}
			return ((Texture)sp.texture).width > 0;
		}
		catch
		{
			return false;
		}
	}

	public static bool TryResolve(string name, ref Sprite result)
	{
		if (string.IsNullOrEmpty(name) || name != "water_tower")
		{
			return false;
		}
		if (!_injected || !IsAlive(_sprite))
		{
			Log.Info("自定义贴图：游戏按名字要图（" + name + "）→ 按需注入" + (_injected ? "（sprite 已失效，重新生成）" : "（懒生成，不必先按 F9）"));
			_attempted = false;
			_injected = false;
			Ensure();
			if (!_injected)
			{
				Log.Warn("自定义贴图：懒生成失败（" + name + "）→ 交回原版查找");
				return false;
			}
		}
		if ((Object)(object)_sprite == (Object)null)
		{
			return false;
		}
		result = _sprite;
		return true;
	}

	private static bool LoadPng(Texture2D tex, byte[] bytes)
	{
		try
		{
			Type type = WaterTowerItemFactory.ResolveUnityType("ImageConversion") ?? FindTypeByName("UnityEngine.ImageConversion");
			if (type == null)
			{
				Log.Warn("自定义贴图：找不到 UnityEngine.ImageConversion 类型");
				return false;
			}
			object obj = WaterTowerItemFactory.CallStatic(type, "LoadImage", new object[2] { tex, bytes }, "ImageConversion.LoadImage(Texture2D, byte[])");
			return obj is bool && (bool)obj;
		}
		catch (Exception ex)
		{
			Log.Exception(ex, "ImageConversion.LoadImage 调用失败");
			return false;
		}
	}

	private static Type FindTypeByName(string fullName)
	{
		try
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				Type type = assembly.GetType(fullName);
				if (type != null)
				{
					return type;
				}
			}
		}
		catch
		{
		}
		return null;
	}

	private static bool InjectIntoAtlasCache(Sprite sprite)
	{
		try
		{
			string text = "Items/watertower_mod";
			string text2 = "water_tower";
			Dictionary<string, Dictionary<string, Sprite>> val = RenderHandler.atlasCache;
			if (val == null)
			{
				val = (RenderHandler.atlasCache = new Dictionary<string, Dictionary<string, Sprite>>());
				Log.Warn("自定义贴图：atlasCache 为空 → 已新建并写回");
			}
			Log.Info("   图集缓存：已取得 atlasCache（现有 " + val.Count + " 个图集条目）");
			Dictionary<string, Sprite> val3 = null;
			if (val.ContainsKey(text))
			{
				try
				{
					val3 = val[text];
				}
				catch (Exception ex)
				{
					Log.Warn("   图集缓存：读取已有图集条目失败（" + ex.GetType().Name + "），改为重建该条目");
					WaterTowerItemFactory.TryRemoveKey(val, text);
					val3 = null;
				}
			}
			if (val3 == null)
			{
				val3 = new Dictionary<string, Sprite>();
				val.Add(text, val3);
			}
			val3[text2] = sprite;
			Sprite val4 = null;
			try
			{
				val4 = val3[text2];
			}
			catch (Exception ex2)
			{
				Log.Warn("   图集缓存：回读失败（" + ex2.GetType().Name + "），改用 ContainsKey 判定");
			}
			bool flag = ((!((Object)(object)val4 != (Object)null)) ? val3.ContainsKey(text2) : ((Object)(object)val4.texture != (Object)null && ((Texture)val4.texture).width == ((Texture)sprite.texture).width));
			if (flag && (Object)(object)val4 != (Object)null && (Object)(object)val4.texture != (Object)null)
			{
				Log.Success("   图集缓存：已写入 RenderHandler.atlasCache[\"" + text + "\"][\"" + text2 + "\"]，回读 " + ((Texture)val4.texture).width + "x" + ((Texture)val4.texture).height + "px ");
			}
			else if (flag)
			{
				Log.Success("   图集缓存：已写入 RenderHandler.atlasCache[\"" + text + "\"][\"" + text2 + "\"]（条目存在；回读走 ContainsKey）");
			}
			else
			{
				Log.Warn("   图集缓存：写入后回读异常（" + (((Object)(object)val4 == (Object)null) ? "<null>" : "宽度不符") + "）");
			}
			return flag;
		}
		catch (Exception ex3)
		{
			Log.Exception(ex3, "写入 RenderHandler.atlasCache 失败");
			return false;
		}
	}

	private static void VerifyThroughGameApi()
	{
		try
		{
			Sprite val = RenderHandler.LoadFromAtlas("Items/watertower_mod", "water_tower");
			if ((Object)(object)val == (Object)null)
			{
				Log.Warn("   贴图通道自证：RenderHandler.LoadFromAtlas 返回 null（dummy 也没拿到）");
				return;
			}
			int num = (((Object)(object)val.texture != (Object)null) ? ((Texture)val.texture).width : (-1));
			int num2 = (((Object)(object)val.texture != (Object)null) ? ((Texture)val.texture).height : (-1));
			if ((Object)(object)_texture != (Object)null && num == ((Texture)_texture).width && num2 == ((Texture)_texture).height)
			{
				Log.Success("   贴图通道自证：游戏 API 取到我们的图（" + num + "x" + num2 + "px） —— GameItemElement 构造与 SetSpriteAndShape 都会命中它");
			}
			else
			{
				Log.Warn("   贴图通道自证：游戏 API 取到的不是我们的图（" + num + "x" + num2 + "px ≠ " + (((Object)(object)_texture != (Object)null) ? (((Texture)_texture).width + "x" + ((Texture)_texture).height) : "?") + "px） —— 仍是 dummy，贴图不会显示");
			}
		}
		catch (Exception ex)
		{
			Log.Exception(ex, "贴图通道自证失败");
		}
	}

	private static byte[] ReadEmbeddedPng()
	{
		try
		{
			Assembly assembly = typeof(ModSprite).Assembly;
			using Stream stream = assembly.GetManifestResourceStream("WaterTower.assets.WaterTower.png");
			if (stream == null)
			{
				Log.Debug("自定义贴图：dll 里没有资源 WaterTower.assets.WaterTower.png（现有资源：" + string.Join(", ", assembly.GetManifestResourceNames()) + "）");
				return null;
			}
			byte[] array = new byte[stream.Length];
			int num;
			for (int i = 0; i < array.Length; i += num)
			{
				num = stream.Read(array, i, array.Length - i);
				if (num <= 0)
				{
					break;
				}
			}
			return array;
		}
		catch (Exception ex)
		{
			Log.Exception(ex, "读内嵌贴图失败");
			return null;
		}
	}

	private static string ResolvePngPath()
	{
		try
		{
			List<string> list = new List<string>();
			string directoryName = Path.GetDirectoryName(typeof(ModSprite).Assembly.Location);
			if (!string.IsNullOrEmpty(directoryName))
			{
				list.Add(directoryName);
			}
			string dataPath = Application.dataPath;
			if (!string.IsNullOrEmpty(dataPath))
			{
				string directoryName2 = Path.GetDirectoryName(dataPath);
				if (!string.IsNullOrEmpty(directoryName2))
				{
					list.Add(Path.Combine(directoryName2, "Mods"));
				}
			}
			foreach (string item in list)
			{
				string[] spriteFileNames = WaterTowerConfig.SpriteFileNames;
				foreach (string path in spriteFileNames)
				{
					string text = Path.Combine(item, path);
					if (File.Exists(text))
					{
						return text;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Log.Debug("自定义贴图：定位 PNG 失败（" + ex.GetType().Name + "）");
		}
		return null;
	}

	public static bool ApplyToItem(Type itemType, object item, string label)
	{
		string atlasPath = AtlasPath;
		string spriteName = SpriteName;
		object obj = WaterTowerItemFactory.CallInstance(itemType, item, "SetSprite", new object[2] { atlasPath, spriteName }, label + " SetSprite(" + atlasPath + "/" + spriteName + ")");
		return obj != null;
	}

	public static void ApplyQuickToExisting(object item)
	{
		if (item != null && _injected)
		{
			WaterTowerItemFactory.CallInstance(item.GetType(), item, "SetSpriteQuick", new object[2] { "Items/watertower_mod", "water_tower" }, "SetSpriteQuick(水塔, 自定义贴图)");
		}
	}

	public static int RefreshExistingTowers()
	{
		if (!_injected)
		{
			return 0;
		}
		int num = 0;
		foreach (TowerInstance item in TowerRegistry.All)
		{
			if (item != null && item.Item != null)
			{
				try
				{
					ApplyQuickToExisting(item.Item);
					num++;
				}
				catch (Exception ex)
				{
					Log.Debug("热换贴图失败（" + item.Label + "）：" + ex.GetType().Name);
				}
			}
		}
		if (num > 0)
		{
			Log.Success("贴图热换：已对 " + num + " 台已登记的水塔应用自定义贴图（旧塔占格不变）");
		}
		return num;
	}
}
