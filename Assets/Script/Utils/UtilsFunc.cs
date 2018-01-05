using UnityEngine;
using System;

public static class UtilsFunc{
	public static void Assert(bool cond, string err = "")
	{
		if (!cond)
		{
			Debug.LogError(err);
		}
	}

	public static string FormatH2M2S2Trim(int time, int limit = -1){
		TimeSpan t = TimeSpan.FromSeconds(time);
		string result = "";
		int len = 0;
		if(t.Days > 0){
			result += string.Format("{0:D2}", t.Days) + "D";
			if (limit > 0 && ++len >= limit){
				return result;
			}
		}
		if(t.Hours > 0){
			result += string.Format("{0:D2}", t.Hours) + "H";
			if (limit > 0 && ++len >= limit){
				return result;
			}
		}
		if(t.Minutes > 0){
			result += string.Format("{0:D2}", t.Minutes) + "M";
			if (limit > 0 && ++len >= limit){
				return result;
			}
		}
		result += string.Format("{0:D2}", t.Seconds) + "S";
		return result;
	}
}
