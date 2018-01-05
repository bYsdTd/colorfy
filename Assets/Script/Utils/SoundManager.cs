using UnityEngine;
using System.Collections.Generic;

public class SoundManager
{
	static SoundManager _instance;
	public static SoundManager Instance()
	{
		if (_instance == null)
		{
			_instance = new SoundManager ();
		}
		return _instance;
	}

	public void Init()
	{
	}

	#region 战斗音效
	// 战斗音量
	float battle_volume = 0.5f;

	// 战斗
	public class Sound{
		public string name;
		public AudioClip clip;
		public AudioSource audioSource;
	}

	// 保存音效资源
	Dictionary<string, AudioClip> audio_assets = new Dictionary<string, AudioClip>();
	AudioClip GetAudioClip(string name)
	{
		if (audio_assets.ContainsKey(name))
		{
			return audio_assets [name];
		}
		AudioClip clip = Resources.Load<AudioClip> (AssetsPathConfig.assets_path_config [name]);
		audio_assets.Add (name, clip);
		return clip;
	}

	// 场景中播放的声音
	List<Sound> battle_sounds = new List<Sound>();

	public void InitBattleAudioSource()
	{
		UICamera.mainCamera.gameObject.GetComponent<AudioListener> ().enabled = false;
		Camera.main.gameObject.GetOrAddComponent<AudioListener> ().enabled = true;
		audio_assets.Clear ();
		battle_sounds.Clear ();
	}

	public void DestroyBattleAudioSource()
	{
		Camera camera = UICamera.mainCamera;
		if (camera == null)
			return; 
				
		camera.gameObject.GetComponent<AudioListener> ().enabled = true;
		for (int i = 0; i < battle_sounds.Count; ++i)
		{
			Sound sound = battle_sounds[i];
			if (sound.audioSource != null) {
				sound.audioSource.Stop ();
			}
			GameObject.Destroy (sound.audioSource);
		}

		foreach(var kv in audio_assets)
		{
			Resources.UnloadAsset (kv.Value);
		}
		audio_assets.Clear ();
		battle_sounds.Clear ();
	}

	public void PlayBattleEffectSound(string soundName, AudioSource audioSouce, bool loop)
	{
		AudioClip clip = GetAudioClip(soundName);

		if (clip == null){
			Debug.LogError ("加载音效失败 " + soundName);
			return;
		}

		audioSouce.clip = clip;
		audioSouce.loop = loop;
		audioSouce.volume = battle_volume;
		audioSouce.playOnAwake = false;
		audioSouce.spatialBlend = 1f;
		audioSouce.Play();

		Sound s = new Sound();
		s.audioSource = audioSouce;
		s.name = soundName;

		battle_sounds.Add(s);
	}
	#endregion 
}