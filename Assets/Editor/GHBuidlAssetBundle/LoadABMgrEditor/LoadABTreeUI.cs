using UnityEngine;
using UnityEditor;

using System;
using System.Collections.Generic;

public class LoadABTreeUI
{
	internal float itemH = 16f;
	private Rect visibleRect;
	private Vector2 position;
	
	public void Draw<T>(List<T> objList, Action<int, Rect, T> itemDrawer){
		var evtType = Event.current.type;
		Rect r = GUILayoutUtility.GetRect(1f, Screen.width, 16f, Screen.height);
		
	#if UNITY_5_5_OR_NEWER
		if (evtType != EventType.Layout){
			visibleRect = r;
		}
	#else
		if (evtType != EventType.layout){
			visibleRect = r;
		}
	#endif
		
		var n			= objList.Count;
		var contentRect	= new Rect(0f, 0f, 1f, n * itemH);
		var nVisible	= Mathf.RoundToInt(visibleRect.height/itemH) + 1;
		var min			= Mathf.Max(0, Mathf.FloorToInt(position.y / itemH));
		var max 		= Mathf.Min(min + nVisible, n);
		var noScroll	= contentRect.height < visibleRect.height;
		
		if (noScroll) position = Vector2.zero;
		
		position = GUI.BeginScrollView(visibleRect, position, contentRect);
		{
			var rect	= new Rect (0, 0, r.width - (noScroll ? 4f : 16f), itemH);
			for (var i = min; i<max; i++){
				rect.y = i * itemH;
				itemDrawer(i, rect, objList[i]);
			}
		}
		
		GUI.EndScrollView();
	}
}