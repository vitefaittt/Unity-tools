var popupStyle = GUI.skin.FindStyle("IconButton");
var popupIcon = EditorGUIUtility.IconContent("_Popup");
var buttonRect = EditorGUILayout.GetControlRect(false, 20f, GUILayout.MaxWidth(20f));
if (GUI.Button(buttonRect, popupIcon, popupStyle))
{
    //Stuff that happens when you click the button
}