//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// Editor component used to display a list of sprites.
/// </summary>

public class SpriteSelector : ScriptableWizard
{
	static public SpriteSelector instance;

	void OnEnable () { instance = this; }
	void OnDisable () { instance = null; }

	public delegate void Callback (string sprite);

	SerializedObject mObject;
	SerializedProperty mProperty;

	UISprite mSprite;
	Vector2 mPos = Vector2.zero;
	Callback mCallback;
	float mClickTime = 0f;

	/// <summary>
	/// Draw the custom wizard.
	/// </summary>

	void OnGUI ()
	{
		NGUIEditorTools.SetLabelWidth(80f);

		if (NGUISettings.atlas == null)
		{
			GUILayout.Label("No Atlas selected.", "LODLevelNotifyText");
		}
		else
		{
			UIAtlas atlas = NGUISettings.atlas;
			bool close = false;
			GUILayout.Label(atlas.name + " Sprites", "LODLevelNotifyText");
			NGUIEditorTools.DrawSeparator();

			GUILayout.BeginHorizontal();
			GUILayout.Space(84f);

			string before = NGUISettings.partialSprite;
			string after = EditorGUILayout.TextField("", before, "SearchTextField");
			if (before != after) NGUISettings.partialSprite = after;

			if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18f)))
			{
				NGUISettings.partialSprite = "";
				GUIUtility.keyboardControl = 0;
			}
			GUILayout.Space(84f);
			GUILayout.EndHorizontal();

			Texture2D tex = atlas.texture as Texture2D;

			if (tex == null)
			{
				GUILayout.Label("The atlas doesn't have a texture to work with");
				return;
			}

			BetterList<string> sprites = atlas.GetListOfSprites(NGUISettings.partialSprite);
			
			float size = 80f;
			float padded = size + 10f;
			int columns = Mathf.FloorToInt(Screen.width / padded);
			if (columns < 1) columns = 1;

			int offset = 0;
			Rect rect = new Rect(10f, 0, size, size);

			GUILayout.Space(10f);
			mPos = GUILayout.BeginScrollView(mPos);
			int rows = 1;

			while (offset < sprites.size)
			{
				GUILayout.BeginHorizontal();
				{
					int col = 0;
					rect.x = 10f;

					for (; offset < sprites.size; ++offset)
					{
						UISpriteData sprite = atlas.GetSprite(sprites[offset]);
						if (sprite == null) continue;

						// Button comes first
						if (GUI.Button(rect, ""))
						{
							if (Event.current.button == 0)
							{
								float delta = Time.realtimeSinceStartup - mClickTime;
								mClickTime = Time.realtimeSinceStartup;

								if (NGUISettings.selectedSprite != sprite.name)
								{
									if (mSprite != null)
									{
										NGUIEditorTools.RegisterUndo("Atlas Selection", mSprite);
										mSprite.MakePixelPerfect();
										EditorUtility.SetDirty(mSprite.gameObject);
									}

									NGUISettings.selectedSprite = sprite.name;
									NGUIEditorTools.RepaintSprites();
									if (mCallback != null) mCallback(sprite.name);
								}
								else if (delta < 0.5f) close = true;
							}
							else
							{
								NGUIContextMenu.AddItem("Edit", false, EditSprite, sprite);
								NGUIContextMenu.AddItem("Delete", false, DeleteSprite, sprite);
								NGUIContextMenu.AddItem("查找引用", false, OnReferenceFind, sprite);
								NGUIContextMenu.Show();
							}
						}

						if (Event.current.type == EventType.Repaint)
						{
							// On top of the button we have a checkboard grid
							NGUIEditorTools.DrawTiledTexture(rect, NGUIEditorTools.backdropTexture);
							Rect uv = new Rect(sprite.x, sprite.y, sprite.width, sprite.height);
							uv = NGUIMath.ConvertToTexCoords(uv, tex.width, tex.height);
	
							// Calculate the texture's scale that's needed to display the sprite in the clipped area
							float scaleX = rect.width / uv.width;
							float scaleY = rect.height / uv.height;
	
							// Stretch the sprite so that it will appear proper
							float aspect = (scaleY / scaleX) / ((float)tex.height / tex.width);
							Rect clipRect = rect;
	
							if (aspect != 1f)
							{
								if (aspect < 1f)
								{
									// The sprite is taller than it is wider
									float padding = size * (1f - aspect) * 0.5f;
									clipRect.xMin += padding;
									clipRect.xMax -= padding;
								}
								else
								{
									// The sprite is wider than it is taller
									float padding = size * (1f - 1f / aspect) * 0.5f;
									clipRect.yMin += padding;
									clipRect.yMax -= padding;
								}
							}
	
							GUI.DrawTextureWithTexCoords(clipRect, tex, uv);
	
							// Draw the selection
							if (NGUISettings.selectedSprite == sprite.name)
							{
								NGUIEditorTools.DrawOutline(rect, new Color(0.4f, 1f, 0f, 1f));
							}
						}

						GUI.backgroundColor = new Color(1f, 1f, 1f, 0.5f);
						GUI.contentColor = new Color(1f, 1f, 1f, 0.7f);
						GUI.Label(new Rect(rect.x, rect.y + rect.height, rect.width, 32f), sprite.name, "ProgressBarBack");
						GUI.contentColor = Color.white;
						GUI.backgroundColor = Color.white;

						if (++col >= columns)
						{
							++offset;
							break;
						}
						rect.x += padded;
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(padded);
				rect.y += padded + 26;
				++rows;
			}
			GUILayout.Space(rows * 26);
			GUILayout.EndScrollView();

			if (close) Close();
		}
	}

	/// <summary>
	/// Edit the sprite (context menu selection)
	/// </summary>

	void EditSprite (object obj)
	{
		if (this == null) return;
		UISpriteData sd = obj as UISpriteData;
		NGUIEditorTools.SelectSprite(sd.name);
		Close();
	}

	/// <summary>
	/// Delete the sprite (context menu selection)
	/// </summary>

	void DeleteSprite (object obj)
	{
		if (this == null) return;

//		// 指向原始贴图
//		NGUIAtlasSplitAlpha.BeforeUIAtlasMakerOperate();
//		//  指向原始图end

		UISpriteData sd = obj as UISpriteData;

		List<UIAtlasMaker.SpriteEntry> sprites = new List<UIAtlasMaker.SpriteEntry>();
		UIAtlasMaker.ExtractSprites(NGUISettings.atlas, sprites);

		for (int i = sprites.Count; i > 0; )
		{
			UIAtlasMaker.SpriteEntry ent = sprites[--i];
			if (ent.name == sd.name)
				sprites.RemoveAt(i);
		}
		UIAtlasMaker.UpdateAtlas(NGUISettings.atlas, sprites);
		NGUIEditorTools.RepaintSprites();

//		// 生成结束，重新split alpha
//		NGUIAtlasSplitAlpha.AfterUIAtlasMakerOperate();	

	}

	void OnReferenceFind(object obj){
		Debug.Log ("Begin... 查找的是Resource文件夹下的所有的prefab，什么时候不满足需求了再更改。");

		if (this == null) return;
		UISpriteData sd = obj as UISpriteData;

		// 得到Resource文件夹下的所有的prefab
		string aim_folder = "Assets/Resources";
		List<string> resultList = new List<string>();
		string[] dirArr = System.IO.Directory.GetFiles(aim_folder, "*", SearchOption.AllDirectories);
		for (int i = 0; i < dirArr.Length; i++)	{
			string _path = dirArr[i];
			// Atlas路径下全是大图，直接跳过
			if (_path.IndexOf("Atlas") > 0 )
				continue;

			//只检查prefab文件
			if (Path.GetExtension(_path) == ".prefab") {
				_path = _path.Replace ('\\', '/');
				resultList.Add(_path);
			}
		}
		resultList.ToArray();
		// 开始检测
		string aimGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(NGUISettings.atlas.GetInstanceID()));
		string aimSprite = sd.name;

		for (int i = 0; i < resultList.Count; i++) {
			string strFilePath = resultList[i];
			// 读取prefab
			FileStream fs = new FileStream(strFilePath, FileMode.Open, FileAccess.Read);
			byte[] buff = new byte[fs.Length];
			fs.Read(buff, 0, (int)fs.Length);
			string strText = Encoding.Default.GetString(buff);
			// 检测
			// 将prefab用关键词分开，每一段查找 图集的guid和spriteName，很大程度保持查找正确性
			string[] separatingChars = { "MonoBehaviour:" };
			string[] words = strText.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries ); 
			int nCount = 0;
			for (int j = 0; j < words.Length; j++) {
				string word = words [j];
				if (word.IndexOf (aimGuid, 0) != -1) {// 包含图集guid，包含spriteName就认为包含了该sprite
					if (word.IndexOf (aimSprite, 0) != -1) {
						nCount++;
					}
				}
			}
			if (0 != nCount) {
				UnityEngine.Debug.Log(strFilePath + "引用了" + nCount + "次");
			}

			fs.Close();
		}

		Debug.Log ("End...记得在代码全局搜索一下这个UISprite的名字，万一代码写了呢。");
	}

	/// <summary>
	/// Property-based selection result.
	/// </summary>

	void OnSpriteSelection (string sp)
	{
		if (mObject != null && mProperty != null)
		{
			mObject.Update();
			mProperty.stringValue = sp;
			mObject.ApplyModifiedProperties();
		}
	}

	/// <summary>
	/// Show the sprite selection wizard.
	/// </summary>

	static public void ShowSelected ()
	{
		if (NGUISettings.atlas != null)
		{
			Show(delegate(string sel) { NGUIEditorTools.SelectSprite(sel); });
		}
	}

	/// <summary>
	/// Show the sprite selection wizard.
	/// </summary>

	static public void Show (SerializedObject ob, SerializedProperty pro, UIAtlas atlas)
	{
		if (instance != null)
		{
			instance.Close();
			instance = null;
		}

		if (ob != null && pro != null && atlas != null)
		{
			SpriteSelector comp = ScriptableWizard.DisplayWizard<SpriteSelector>("Select a Sprite");
			NGUISettings.atlas = atlas;
			NGUISettings.selectedSprite = pro.hasMultipleDifferentValues ? null : pro.stringValue;
			comp.mSprite = null;
			comp.mObject = ob;
			comp.mProperty = pro;
			comp.mCallback = comp.OnSpriteSelection;
		}
	}

	/// <summary>
	/// Show the selection wizard.
	/// </summary>

	static public void Show (Callback callback)
	{
		if (instance != null)
		{
			instance.Close();
			instance = null;
		}

		SpriteSelector comp = ScriptableWizard.DisplayWizard<SpriteSelector>("Select a Sprite");
		comp.mSprite = null;
		comp.mCallback = callback;
	}
}
