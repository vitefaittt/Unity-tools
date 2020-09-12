using System;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.Events;
using UnityEngine;
#endif
using UnityEngine.Events;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Video;

public static class EditorUtilities
{
    #region Persistent events ------------
    /// <summary>
    /// Add an action to the UnityEvent displayed in the editor.
    /// </summary>
    /// <param name="button">The object that will listen to the event.</param>
    /// <param name="action">The action that will be triggered after the event.</param>
    /// <returns>Whether the action was successfully added.</returns>
    public static bool AddPersistentEvent(this UnityEvent evnt, UnityEngine.Object caller, UnityAction action)
    {
#if !UNITY_EDITOR
        return false;
#else
        if (evnt.HasCall(caller, action.Method.Name))
            return false;
        UnityEventTools.AddPersistentListener(evnt, action);
        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
#endif
        return true;
    }

    /// <summary>
    /// Add an action to the UnityEvent displayed in the editor.
    /// </summary>
    /// <param name="button">The object that will listen to the event.</param>
    /// <param name="action">The action to trigger.</param>
    /// <returns>Success state.</returns>
    public static bool AddPersistentEvent<T>(this UnityEvent @event, UnityEngine.Object caller, UnityAction<T> method, T argument) where T : UnityEngine.Object
    {
#if !UNITY_EDITOR
        return false;
#else
        if (@event.HasCall(caller, method.Method.Name))
            return false;
        var targetInfo = UnityEventBase.GetValidMethodInfo(caller, method.Method.Name, new Type[] { typeof(UnityEngine.Object) });
        UnityAction<T> methodDelegate = Delegate.CreateDelegate(typeof(UnityAction<T>), caller, targetInfo) as UnityAction<T>;
        UnityEventTools.AddObjectPersistentListener(@event, methodDelegate, argument);

        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
#endif
        return true;
    }

    /// <summary>
    /// Add an action to the UnityEvent displayed in the editor.
    /// </summary>
    /// <param name="button">The object that will listen to the event.</param>
    /// <param name="action">The action to trigger.</param>
    /// <returns>Success state.</returns>
    public static bool AddPersistentEvent(this UnityEvent<string> @event, UnityEngine.Object caller, UnityAction<string> method)
    {
#if !UNITY_EDITOR
        return false;
#else
        if (@event.HasCall(caller, method.Method.Name))
            return false;
        var targetInfo = UnityEventBase.GetValidMethodInfo(caller, method.Method.Name, new Type[] { typeof(string) });
        UnityAction<string> methodDelegate = Delegate.CreateDelegate(typeof(UnityAction<string>), caller, targetInfo) as UnityAction<string>;
        UnityEventTools.AddPersistentListener(@event, methodDelegate);

        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
#endif
        return true;
    }

    /// <summary>
    /// Add an action to the UnityEvent displayed in the editor.
    /// </summary>
    /// <param name="button">The object that will listen to the event.</param>
    /// <param name="action">The action to trigger.</param>
    /// <returns>Success state.</returns>
    public static bool AddPersistentEvent(this UnityEvent @event, UnityEngine.Object caller, UnityAction<float> method, float argument)
    {
#if !UNITY_EDITOR
        return false;
#else
        if (@event.HasCall(caller, method.Method.Name))
            return false;
        var targetInfo = UnityEventBase.GetValidMethodInfo(caller, method.Method.Name, new Type[] { typeof(float) });
        UnityAction<float> methodDelegate = Delegate.CreateDelegate(typeof(UnityAction<float>), caller, targetInfo) as UnityAction<float>;
        UnityEventTools.AddFloatPersistentListener(@event, methodDelegate, argument);

        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
#endif
        return true;
    }

    /// <summary>
    /// Add an action to the UnityEvent displayed in the editor.
    /// </summary>
    /// <param name="button">The object that will listen to the event.</param>
    /// <param name="action">The action to trigger.</param>
    /// <returns>Success state.</returns>
    public static bool AddPersistentEvent(this UnityEvent @event, UnityEngine.Object caller, UnityAction<int> method, int argument)
    {
#if !UNITY_EDITOR
        return false;
#else
        if (@event.HasCall(caller, method.Method.Name))
            return false;
        var targetInfo = UnityEventBase.GetValidMethodInfo(caller, method.Method.Name, new Type[] { typeof(int) });
        UnityAction<int> methodDelegate = Delegate.CreateDelegate(typeof(UnityAction<int>), caller, targetInfo) as UnityAction<int>;
        UnityEventTools.AddIntPersistentListener(@event, methodDelegate, argument);

        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
#endif
        return true;
    }

    static bool HasCall(this UnityEvent @event, UnityEngine.Object caller, string methodName)
    {
        for (int i = 0; i < @event.GetPersistentEventCount(); i++)
            if (@event.GetPersistentTarget(i) == caller && @event.GetPersistentMethodName(i) == methodName)
                return true;
        return false;
    }

    static bool HasCall<T>(this UnityEvent<T> @event, UnityEngine.Object caller, string methodName)
    {
        for (int i = 0; i < @event.GetPersistentEventCount(); i++)
            if (@event.GetPersistentTarget(i) == caller && @event.GetPersistentMethodName(i) == methodName)
                return true;
        return false;
    }
    #endregion ------------------------

#if UNITY_EDITOR
    [MenuItem("Window/Tools/Editor Utilities/Add order to names")]
    static void RenameSelectionInOrder()
    {
        IEnumerable<GameObject> orderedSelectedObjects = Selection.gameObjects.OrderBy((o) => o.transform.GetSiblingIndex());
        int index = 0;
        foreach (GameObject selectedObject in orderedSelectedObjects)
            selectedObject.name = selectedObject.name + index++;
    }

    [MenuItem("CONTEXT/VideoPlayer/Rename GameObject")]
    static void VideoPlayerRenameGameObject()
    {
        foreach (var gameObject in Selection.gameObjects)
            if (gameObject.GetComponent<VideoPlayer>())
                gameObject.name = gameObject.GetComponent<VideoPlayer>().clip ? gameObject.GetComponent<VideoPlayer>().clip.name : "null";
    }
#endif
}
