using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SymmetrySprite : UISprite {
	
	public enum SymmetryType{
		SimpleLR,
		SimpleBoth,
		SliceLR,
		SliceBoth,
	}
	
	public SymmetryType symmetryType = SymmetryType.SimpleLR;
	
	public override void OnFill (BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols){
		Texture tex = mainTexture;
		if (tex == null) return;
		
		if (mSprite == null) mSprite = atlas.GetSprite(spriteName);
		if (mSprite == null) return;
		
		Rect outer = new Rect(mSprite.x, mSprite.y, mSprite.width, mSprite.height);
		Rect inner = new Rect(mSprite.x + mSprite.borderLeft, mSprite.y + mSprite.borderTop,
		                      mSprite.width - mSprite.borderLeft - mSprite.borderRight,
		                      mSprite.height - mSprite.borderBottom - mSprite.borderTop);
		
		outer = NGUIMath.ConvertToTexCoords(outer, tex.width, tex.height);
		inner = NGUIMath.ConvertToTexCoords(inner, tex.width, tex.height);
		
		int offset = verts.size;
		
		mOuterUV = outer;
		mInnerUV = inner;
		
		switch(symmetryType){
		case SymmetryType.SimpleLR:
			SimpleFillLR(verts, uvs, cols);
			break;
		
		case SymmetryType.SimpleBoth:
			SimpleFillBoth(verts, uvs, cols);
			break;
			
		case SymmetryType.SliceLR:
			SlicedFillLR(verts, uvs, cols);
			break;
			
		case SymmetryType.SliceBoth:
			SlicedFillBoth(verts, uvs, cols);
			break;
		}
		
		
		if (onPostFill != null)
			onPostFill(this, offset, verts, uvs, cols);
	}
	
	void SimpleFillLR (BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Vector4 v = drawingDimensions;
		Color32 c = drawingColor;
		
		float[] xPos = new float[3]{
			v.x,
			(v.x + v.z)/2f,
			v.z
		};
		
		float[] yPos = new float[2]{
			v.y,
			v.w,
		};
		
		float[] xUV = new float[3]{
			mOuterUV.xMin,
			mInnerUV.xMax,
			mOuterUV.xMin,
		};
		
		float[] yUV = new float[2]{
			mOuterUV.yMin,
			mOuterUV.yMax,
		};
		
		Fill(xPos, yPos, xUV, yUV, c, verts, uvs, cols);
	}
	
	void SlicedFillLR (BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Vector4 br = border * pixelSize;
		
		if (br.x == 0f && br.z == 0f )
		{
			SimpleFillLR(verts, uvs, cols);
			return;
		}
		
		Color32 c = drawingColor;
		Vector4 v = drawingDimensions;
		
		float[] xPos = new float[5]{
			v.x,
			v.x + br.x,
			(v.x + v.z)/2f,
			v.z - br.x,
			v.z
		};
		
		float[] yPos = new float[2]{
			v.y,
			v.w,
		};
		
		float[] xUV = new float[5]{
			mOuterUV.xMin,
			mInnerUV.xMin,
			mInnerUV.xMax,
			mInnerUV.xMin,
			mOuterUV.xMin,
		};
		
		float[] yUV = new float[2]{
			mOuterUV.yMin,
			mOuterUV.yMax,
		};
		
		Fill(xPos, yPos, xUV, yUV, c, verts, uvs, cols);
	}
	
	void SimpleFillBoth (BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Vector4 v = drawingDimensions;
		Color32 c = drawingColor;
		
		float[] xPos = new float[3]{
			v.x,
			(v.x + v.z)/2f,
			v.z
		};
		
		float[] yPos = new float[3]{
			v.y,
			(v.y + v.w)/2f,
			v.w,
		};
		
		float[] xUV = new float[3]{
			mOuterUV.xMin,
			mInnerUV.xMax,
			mOuterUV.xMin,
		};
		
		float[] yUV = new float[3]{
			mOuterUV.yMax,
			mInnerUV.yMin,
			mOuterUV.yMax,
		};
		
		Fill(xPos, yPos, xUV, yUV, c, verts, uvs, cols);
	}
	
	void SlicedFillBoth (BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Vector4 br = border * pixelSize;
		
		if (br.x == 0f && br.y == 0f && br.z == 0f && br.w == 0f)
		{
			SimpleFillBoth(verts, uvs, cols);
			return;
		}
		
		Color32 c = drawingColor;
		Vector4 v = drawingDimensions;
		
		float[] xPos = new float[5]{
			v.x,
			v.x + br.x,
			(v.x + v.z)/2f,
			v.z - br.x,
			v.z
		};
		
		float[] yPos = new float[5]{
			v.y,
			v.y + br.w,
			(v.y + v.w)/2f,
			v.w - br.w,
			v.w,
		};
		
		float[] xUV = new float[5]{
			mOuterUV.xMin,
			mInnerUV.xMin,
			mInnerUV.xMax,
			mInnerUV.xMin,
			mOuterUV.xMin,
		};
		
		float[] yUV = new float[5]{
			mOuterUV.yMax,
			mInnerUV.yMax,
			mInnerUV.yMin,
			mInnerUV.yMax,
			mOuterUV.yMax,
		};
		
		Fill(xPos, yPos, xUV, yUV, c, verts, uvs, cols);
	}
	
	void Fill(float[] xPos, float[] yPos, float[] xUV, float[] yUV, Color c,
	          BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols){
	          
		for (int x=0; x<xPos.Length-1; ++x){
			for (int y=0; y<yPos.Length-1; ++y){
				verts.Add(new Vector3(xPos[x], yPos[y]));
				verts.Add(new Vector3(xPos[x+1], yPos[y]));
				verts.Add(new Vector3(xPos[x+1], yPos[y+1]));
				verts.Add(new Vector3(xPos[x], yPos[y+1]));
				
				uvs.Add(new Vector2(xUV[x], yUV[y]));
				uvs.Add(new Vector2(xUV[x+1], yUV[y]));
				uvs.Add(new Vector2(xUV[x+1], yUV[y+1]));
				uvs.Add(new Vector2(xUV[x], yUV[y+1]));
				
				cols.Add(c);
				cols.Add(c);
				cols.Add(c);
				cols.Add(c);
			}
		}
	}
}
