using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EmojiSprite : UIWidget {

	public sealed partial class chat_expression
	{
		public string expression_name;
		public int expression_type;
		public string expression_icon;
    }

	public class EmojiVO {
		public chat_expression gdsInfo;
		public BMSymbol emojiInfo;

		public EmojiVO(chat_expression gdsInfo) {
			this.gdsInfo = gdsInfo;
			emojiInfo = new BMSymbol()
			{
				sequence = "[#" + gdsInfo.expression_name + "]",
				spriteName = gdsInfo.expression_icon
			};
		}
	}

    public delegate bool EmojiFilter (string emojiName, int emojiType, string text);

    string mLastMatchText = null;
    Dictionary<int, EmojiVO> mEmojiCache = new Dictionary<int, EmojiVO>();

    public UILabel mLabel;
	public UILabel label {
		get {
            if (mLabel == null) {
				if (transform.parent != null) {
                    var parentLbl = transform.parent.GetComponent<UILabel>();
					if (parentLbl != null) {
                        mLabel = parentLbl;
                    }
                }
            }

            return mLabel;
        }
		set {
            if (mLabel != value)
            {
                mLabel = value;
                MarkAsChanged();
            }
        }
	}

    List<BMSymbol> mSymbols;

    public EmojiFilter emojiFilter;

    [HideInInspector][SerializeField] UIAtlas mAtlas;
	public UIAtlas atlas {
        get { return mAtlas; }
		set {
			if (mAtlas != value) {
				RemoveFromPanel();

				mAtlas = value;
				MarkAsChanged();
			}
		}
    }
    public override Material material { get { return (mAtlas != null) ? mAtlas.spriteMaterial : null; } }

    public BMSymbol MatchSymbol (string text, int offset, int textLength) {

        if (!string.Equals(mLastMatchText, text)) {
            //mEmojiCache = MatchAllEmojis(text);
            mLastMatchText = text;
        }

        if (!mEmojiCache.ContainsKey(offset))
            return null;

        var emoji = mEmojiCache[offset];
        if (emojiFilter != null && !emojiFilter (emoji.emojiInfo.sequence, emoji.gdsInfo.expression_type, text)) {
            return null;
        }

        if (emoji.emojiInfo.Validate(mAtlas)) {
            if (emoji.gdsInfo.expression_type == 2)
            {
                // Adjust big emojis' size
                int newWidth = 330;
                int newHeight = 270;
                emoji.emojiInfo.advance += newWidth - emoji.emojiInfo.width;
                emoji.emojiInfo.width = newWidth;
                emoji.emojiInfo.height = newHeight;
            }
            return emoji.emojiInfo;
        }

        return null;
	}

    public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols) {
		Texture tex = mainTexture;
		if (tex == null) return;

        label.PrintEmojis(verts, uvs, cols);
		// Change the tint to white
		for (int i = 0; i < cols.size; i++)
		{
            cols[i] = Color.white;
        }
    }
}