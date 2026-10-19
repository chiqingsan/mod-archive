using UnityEngine;

namespace YSGame;

public class YSPlayBackgroundMusic : MonoBehaviour
{
	public int MusicIndex;

	private void Start()
	{
		Play();
	}

	public void Play()
	{
		MusicMag.instance.playMusic(MusicIndex);
	}
}
