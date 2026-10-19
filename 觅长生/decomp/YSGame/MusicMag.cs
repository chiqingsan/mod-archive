using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace YSGame;

public class MusicMag : MonoBehaviour
{
	public List<MusicInfo> BackGroudMusic = new List<MusicInfo>();

	public List<MusicInfo> EffectMusic = new List<MusicInfo>();

	public List<NewMusicInfo> NewBackMusic = new List<NewMusicInfo>();

	public List<NewMusicInfo> NewEffectMusic = new List<NewMusicInfo>();

	public AudioSource audioSource;

	public List<AudioSource> effectAudioSouces = new List<AudioSource>();

	private int effectPlayIndex;

	public static MusicMag instance;

	private bool flagPlay;

	private int nowBackgroundIndex = -999;

	public int EffectPlayIndex
	{
		get
		{
			return effectPlayIndex;
		}
		set
		{
			effectPlayIndex = value;
			if (effectPlayIndex >= effectAudioSouces.Count)
			{
				effectPlayIndex = 0;
			}
		}
	}

	private void Start()
	{
		instance = this;
		initAudio();
		Init();
	}

	private void Init()
	{
		BackGroudMusic = new List<MusicInfo>();
		foreach (NewMusicInfo item in NewBackMusic)
		{
			BackGroudMusic.Add(new MusicInfo
			{
				name = item.GameName,
				audioClip = ResManager.inst.LoadAudioClip(item.FileName)
			});
		}
		EffectMusic = new List<MusicInfo>();
		foreach (NewMusicInfo item2 in NewEffectMusic)
		{
			EffectMusic.Add(new MusicInfo
			{
				name = item2.GameName,
				audioClip = ResManager.inst.LoadAudioClip(item2.FileName)
			});
		}
	}

	public void Create()
	{
		NewBackMusic = new List<NewMusicInfo>();
		NewEffectMusic = new List<NewMusicInfo>();
		foreach (MusicInfo item in BackGroudMusic)
		{
			NewBackMusic.Add(new NewMusicInfo
			{
				GameName = item.name,
				FileName = ((Object)item.audioClip).name,
				DirectoryName = GetDirectoryName(((Object)item.audioClip).name)
			});
		}
		foreach (MusicInfo item2 in EffectMusic)
		{
			NewEffectMusic.Add(new NewMusicInfo
			{
				GameName = item2.name,
				FileName = ((Object)item2.audioClip).name,
				DirectoryName = GetDirectoryName(((Object)item2.audioClip).name)
			});
		}
	}

	public void Check()
	{
		foreach (NewMusicInfo item in NewBackMusic)
		{
			if (item.DirectoryName == "")
			{
				Debug.LogError((object)(item.FileName + "的DirectoryName为空"));
			}
		}
		foreach (NewMusicInfo item2 in NewEffectMusic)
		{
			if (item2.DirectoryName == "")
			{
				Debug.LogError((object)(item2.FileName + "的DirectoryName为空"));
			}
		}
		Debug.LogError((object)"检查完毕");
	}

	private string GetDirectoryName(string fileName)
	{
		string text = "BG";
		if ((Object)(object)Resources.Load<AudioClip>("Music/" + text + "/" + fileName) != (Object)null)
		{
			return text;
		}
		text = "Skill";
		if ((Object)(object)Resources.Load<AudioClip>("Music/" + text + "/" + fileName) != (Object)null)
		{
			return text;
		}
		text = "UI";
		if ((Object)(object)Resources.Load<AudioClip>("Music/" + text + "/" + fileName) != (Object)null)
		{
			return text;
		}
		return "";
	}

	public AudioClip GetEffect(string name)
	{
		foreach (MusicInfo item in EffectMusic)
		{
			if (item.name == name)
			{
				return item.audioClip;
			}
		}
		foreach (MusicInfo item2 in EffectMusic)
		{
			if (((Object)item2.audioClip).name == name)
			{
				return item2.audioClip;
			}
		}
		return null;
	}

	public void PlayEffectMusic(int index, float pitch = 1f)
	{
		AudioSource val = effectAudioSouces[EffectPlayIndex++];
		val.clip = EffectMusic[index - 1].audioClip;
		PlayEffectMusic(val, pitch);
	}

	public void PlayEffectMusic(AudioClip audioClip, float pitch = 1f)
	{
		AudioSource val = effectAudioSouces[EffectPlayIndex++];
		val.clip = audioClip;
		PlayEffectMusic(val, pitch);
	}

	public void PlayEffectMusic(string name, float pitch = 1f)
	{
		try
		{
			AudioClip effect = GetEffect(name);
			if ((Object)(object)effect != (Object)null)
			{
				PlayEffectMusic(effect, pitch);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError((object)(name + "音效出错"));
			Debug.LogError((object)ex);
		}
	}

	private void PlayEffectMusic(AudioSource audioSource, float pitch = 1f)
	{
		audioSource.pitch = pitch;
		audioSource.Play();
	}

	public void playMusic(int index)
	{
		if (nowBackgroundIndex != index || !(audioSource.volume > 0f))
		{
			nowBackgroundIndex = index;
			((MonoBehaviour)this).StopAllCoroutines();
			if (audioSource.isPlaying)
			{
				((MonoBehaviour)this).StartCoroutine(JianBianStopMusic());
			}
			((MonoBehaviour)this).StartCoroutine(RealizePlayeMusic(index));
		}
	}

	public void playMusic(string name)
	{
		int musicIndex = getMusicIndex(name);
		playMusic(musicIndex);
	}

	public void PlayMusic(string name)
	{
		((MonoBehaviour)this).StopAllCoroutines();
		flagPlay = true;
		int index = (nowBackgroundIndex = getMusicIndex(name));
		float time = audioSource.time;
		audioSource.clip = BackGroudMusic[index].audioClip;
		audioSource.time = time;
		audioSource.volume = getBackgroundVoice();
		audioSource.Play();
	}

	public void PlayMusicImmediately(string name)
	{
		((MonoBehaviour)this).StopAllCoroutines();
		int num = (nowBackgroundIndex = getMusicIndex(name));
		flagPlay = true;
		try
		{
			audioSource.clip = BackGroudMusic[num].audioClip;
			audioSource.volume = getBackgroundVoice();
			audioSource.Play();
		}
		catch (Exception arg)
		{
			Debug.LogError((object)$"播放音乐时出错，PlayMusicImmediately,index{num} {arg}");
		}
	}

	public void PlayMusicList(List<string> musicNameList, bool firstImmediately, bool listImmediately)
	{
		((MonoBehaviour)this).StopAllCoroutines();
		((MonoBehaviour)this).StartCoroutine(PlayMusicListCoroutine(musicNameList, firstImmediately, listImmediately));
	}

	private IEnumerator PlayMusicListCoroutine(List<string> musicNameList, bool firstImmediately, bool listImmediately)
	{
		if (musicNameList.Count < 1)
		{
			yield break;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append($"开始播放音乐列表,第一首立即切换:{firstImmediately},列表音乐立即切换:{listImmediately},音乐列表:\n");
		for (int j = 0; j < musicNameList.Count; j++)
		{
			stringBuilder.AppendLine($"{j}:{musicNameList[j]}");
		}
		Debug.Log((object)stringBuilder.ToString());
		if (!firstImmediately && audioSource.isPlaying)
		{
			Debug.Log((object)"有其他音乐在播放，渐变停止前奏");
			yield return ((MonoBehaviour)this).StartCoroutine(JianBianStopMusic());
		}
		for (int i = 0; i < musicNameList.Count; i++)
		{
			Debug.Log((object)$"开始处理索引第{i}首音乐 《{musicNameList[i]}》");
			int nowMusicIndex = (nowBackgroundIndex = getMusicIndex(musicNameList[i]));
			if (i != 0 && !listImmediately && audioSource.isPlaying)
			{
				Debug.Log((object)"渐变停止前奏");
				yield return ((MonoBehaviour)this).StartCoroutine(JianBianStopMusic());
			}
			audioSource.clip = BackGroudMusic[nowMusicIndex].audioClip;
			audioSource.volume = getBackgroundVoice();
			Debug.Log((object)$"开始播放索引第{i}首音乐 《{musicNameList[i]}》, 此音乐时长{audioSource.clip.length}");
			audioSource.Play();
			if (audioSource.clip.length > 3f)
			{
				yield return (object)new WaitForSeconds(audioSource.clip.length - 3f);
				Debug.Log((object)$"索引第{i}首音乐 《{musicNameList[i]}》 即将播放完毕，倒计时3");
				yield return CoroutineTool.WaitForSeconds1;
				Debug.Log((object)$"索引第{i}首音乐 《{musicNameList[i]}》 即将播放完毕，倒计时2");
				yield return CoroutineTool.WaitForSeconds1;
				Debug.Log((object)$"索引第{i}首音乐 《{musicNameList[i]}》 即将播放完毕，倒计时1");
				yield return CoroutineTool.WaitForSeconds1;
			}
			else
			{
				yield return (object)new WaitForSeconds(audioSource.clip.length);
			}
		}
	}

	private IEnumerator RealizePlayeMusic(int index)
	{
		float num = (audioSource.isPlaying ? 1.5f : 0.5f);
		yield return (object)new WaitForSeconds(num);
		flagPlay = true;
		try
		{
			audioSource.clip = BackGroudMusic[index].audioClip;
			audioSource.volume = getBackgroundVoice();
			audioSource.Play();
		}
		catch (Exception arg)
		{
			Debug.LogError((object)$"RealizePlayeMusic,index{index} {arg}");
		}
	}

	private IEnumerator JianBianStopMusic()
	{
		float volumeAdd = 0f - getBackgroundVoice();
		if (volumeAdd == 0f)
		{
			yield return (object)new WaitForSeconds(0.1f);
			yield break;
		}
		float initVolume2 = audioSource.volume;
		flagPlay = false;
		while (true)
		{
			if (flagPlay)
			{
				flagPlay = false;
				yield break;
			}
			initVolume2 += volumeAdd / 10f;
			if (initVolume2 > getBackgroundVoice() || initVolume2 < 0f)
			{
				break;
			}
			audioSource.volume = initVolume2;
			yield return (object)new WaitForSeconds(0.1f);
		}
		initVolume2 = Mathf.Clamp01(initVolume2);
		audioSource.volume = initVolume2;
		if (initVolume2 == 0f)
		{
			audioSource.Stop();
		}
	}

	public int getMusicIndex(string name)
	{
		int num = 0;
		using (List<MusicInfo>.Enumerator enumerator = BackGroudMusic.GetEnumerator())
		{
			while (enumerator.MoveNext() && !(enumerator.Current.name == name))
			{
				num++;
			}
		}
		return num;
	}

	public void stopMusic()
	{
		((MonoBehaviour)this).StartCoroutine(JianBianStopMusic());
	}

	public float getBackgroundVoice()
	{
		return SystemConfig.Inst.GetBackGroundVolume();
	}

	public void initAudio()
	{
		setBackGroundVolume(getBackgroundVoice());
		setEffectVolum(SystemConfig.Inst.GetEffectVolume());
	}

	public void setFunguseMusice()
	{
		float num = ((PlayerPrefs.GetInt("SavePlayerSet0", 0) == 0) ? 1 : 0);
		_ = (float)PlayerPrefs.GetInt("SavePlayerSet2", 10) / 10f;
		GameObject val = GameObject.Find("FungusManager");
		if ((Object)(object)val != (Object)null)
		{
			AudioSource[] components = val.GetComponents<AudioSource>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].volume = SystemConfig.Inst.GetBackGroundVolume();
			}
		}
	}

	public void setBackGroundVolume(float volume)
	{
		audioSource.volume = volume;
	}

	public void setEffectVolum(float volume)
	{
		foreach (AudioSource effectAudioSouce in effectAudioSouces)
		{
			effectAudioSouce.volume = volume;
		}
	}

	public void StopMusic()
	{
		audioSource.Stop();
	}
}
