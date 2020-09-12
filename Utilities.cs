using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

public static class Utilities
{
    #region Coroutines.
    #region ProgressionAnim.
    public static Coroutine ProgressionAnim(this MonoBehaviour caller, float duration, Action<float> progressionHandler, Action EndAction = null)
    {
        if (!caller.gameObject.activeInHierarchy)
            return null;
        Coroutine routine = caller.StartCoroutine(ProgressionAnim(duration, progressionHandler, EndAction));
        return routine;
    }

    static IEnumerator ProgressionAnim(float duration, Action<float> progressionHandler, Action EndAction)
    {
        float progression = 0;
        while (progression <= 1)
        {
            progressionHandler(progression);
            progression += Time.deltaTime / duration;
            yield return null;
        }
        progressionHandler(1);
        if (EndAction != null)
            EndAction();
    }
    #endregion

    #region Timer.
    public static Coroutine Timer(this MonoBehaviour caller, float duration, Action EndAction)
    {
        Coroutine routine = caller.StartCoroutine(TimerRoutine(duration, EndAction));
        return routine;
    }

    static IEnumerator TimerRoutine(float duration, Action EndAction)
    {
        yield return new WaitForSeconds(duration);
        EndAction();
    }
    #endregion

    /// <summary>
    /// Returns true if the routine is null.
    /// </summary>
    /// <param name="routine"></param>
    /// <returns></returns>
    public static bool IsRunning(this Coroutine routine)
    {
        return routine != null;
    }

    public static void StopAndEmptyCoroutine(this MonoBehaviour caller, ref Coroutine routine)
    {
        if (!routine.IsRunning())
            return;
        caller.StopCoroutine(routine);
        routine = null;
    }

    public static Coroutine WaitUntil(this MonoBehaviour caller, Func<bool> Condition, Action EndAction)
    {
        return caller.StartCoroutine(WaitUntilRoutine(Condition, EndAction));
    }

    static IEnumerator WaitUntilRoutine(Func<bool> Condition, Action EndAction)
    {
        yield return new WaitUntil(Condition);
        EndAction();
    }
    #endregion

    #region Children actions.
    #region Show/hide.
    public static void ShowChildren(this Transform transform)
    {
        transform.SetChildrenActive(true);
    }
    public static void ShowChildren(this GameObject go) { ShowChildren(go.transform); }

    public static void HideChildren(this Transform transform)
    {
        transform.SetChildrenActive(false);
    }
    public static void HideChildren(this GameObject go) { HideChildren(go.transform); }

    static void SetChildrenActive(this Transform transform, bool state)
    {
        transform.ForEachChild(delegate (Transform child) { child.gameObject.SetActive(state); });
    }
    #endregion

    #region Destroy.
    public static void DestroyChildren(this Transform transform)
    {
        if (transform == null)
            return;
        transform.ForEachChildFromEnd(delegate (Transform child)
        {
            if (!Application.isPlaying)
                UnityEngine.Object.DestroyImmediate(child.gameObject);
            else
                UnityEngine.Object.Destroy(child.gameObject);
        });
    }
    public static void DestroyChildren(this GameObject gameObject) { gameObject.transform.DestroyChildren(); }
    #endregion

    #region Foreach child.
    public static void ForEachChild(this Transform parent, Action<Transform> ActionOnChild, Func<bool> StopCondition = null)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            if (StopCondition != null && StopCondition())
                return;
            ActionOnChild(parent.GetChild(i));
        }
    }
    public static void ForEachChildFromEnd(this Transform parent, Action<Transform> ActionOnChild, Func<bool> StopCondition = null)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            if (StopCondition != null && StopCondition())
                return;
            ActionOnChild(parent.GetChild(i));
        }
    }

    public static void ForAllDescendants(this Transform parent, Action<Transform> ActionOnChild, Func<bool> StopCondition = null)
    {
        foreach (Transform child in parent)
        {
            if (StopCondition != null && StopCondition())
                return;
            ActionOnChild(child);
            ForAllDescendants(child, ActionOnChild, StopCondition);
        }
    }

    public static Transform FindRecursive(this Transform parent, string name)
    {
        Transform result = null;
        parent.ForAllDescendants(c =>
        {
            if (c.name == name)
                result = c;
        }, () => result != null);
        return result;
    }

    public static Transform FindRecursive(this Transform parent, params string[] requiredKeywords)
    {
        Transform result = null;
        parent.ForAllDescendants(c =>
        {
            bool childIsValid = true;
            for (int i = 0; i < requiredKeywords.Length && childIsValid; i++)
                if (c.name.IndexOf(requiredKeywords[i], StringComparison.InvariantCultureIgnoreCase) < 0)
                    childIsValid = false;
            if (childIsValid)
                result = c;
        }, () => result != null);
        return result;
    }
    #endregion

    /// <summary>
    /// Find a child whose name has all required keywords.
    /// </summary>
    public static Transform FindWithKeywords(this Transform parent, params string[] requiredKeywords)
    {
        foreach (Transform child in parent)
        {
            bool childIsValid = true;
            for (int i = 0; i < requiredKeywords.Length && childIsValid; i++)
                if (child.name.IndexOf(requiredKeywords[i], StringComparison.InvariantCultureIgnoreCase) < 0)
                    childIsValid = false;
            if (childIsValid)
                return child;
        }
        return null;
    }
    #endregion

    #region Reset transform.
    /// <summary>
    /// Reset localPosition, localRotation, localScale.
    /// </summary>
    public static void Reset(this Transform transform, ResetTransform parameter = ResetTransform.Position | ResetTransform.Rotation | ResetTransform.Scale)
    {
        if ((parameter & ResetTransform.Position) != 0)
            transform.localPosition = Vector3.zero;
        if ((parameter & ResetTransform.Rotation) != 0)
            transform.localRotation = Quaternion.identity;
        if ((parameter & ResetTransform.Scale) != 0)
            transform.localScale = Vector3.one;
    }

    [Flags]
    public enum ResetTransform
    {
        Position = 1 << 0,
        Rotation = 1 << 1,
        Scale = 1 << 2
    }
    #endregion

    /// <summary>
    /// Set the name of this component's gameObject from the name of this component.
    /// </summary>
    /// <param name="mono"></param>
    public static void RenameFromType(this MonoBehaviour mono, bool onlyIfDefaultName = false)
    {
        if (onlyIfDefaultName && mono.gameObject.name != "GameObject")
            return;
        mono.gameObject.name = mono.GetType().Name.ToTitle();
    }

    #region Add, get, duplicate components.
    public static T GetOrAddComponent<T>(this Component component) where T : Component
    {
        return component.gameObject.GetOrAddComponent<T>();
    }
    public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
    {
        if (obj.GetComponent<T>() != null)
            return obj.GetComponent<T>();
        else
            return obj.gameObject.AddComponent<T>();
    }

    public static T GetComponentInParent<T>(this Component component, bool includeInactive)
    {
        return component.gameObject.GetComponentInParent<T>(includeInactive);
    }
    public static T GetComponentInParent<T>(this GameObject obj, bool includeInactive)
    {
        Transform objParent = obj.transform.parent;
        while (objParent)
        {
            if (objParent.GetComponent<T>() != null)
                return objParent.GetComponent<T>();
            objParent = objParent.parent;
        }
        return default(T);
    }

    public static T GetComponentUpwards<T>(this MonoBehaviour mono, bool includeInactive = false)
    {
        T[] parentComponents = mono.GetComponentsInParent<T>(includeInactive);
        T componentOnMono = mono.GetComponent<T>();
        if (parentComponents.Length == 0)
            return default(T);
        if (componentOnMono == null)
            return parentComponents[0];
        for (int i = 0; i < parentComponents.Length; i++)
            if (!parentComponents[i].Equals(componentOnMono))
                return parentComponents[i];
        return default(T);
    }

    public static void Get<T>(this MonoBehaviour monoB, ref T component)
    {
        component = monoB.GetComponent<T>();
    }

    #region Duplicate a component.
    public static T GetCopyOf<T>(this Component comp, T other) where T : Component
    {
        Type type = comp.GetType();
        if (type != other.GetType())
            return null;
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
        PropertyInfo[] pinfos = type.GetProperties(flags);
        foreach (var pinfo in pinfos)
        {
            if (pinfo.CanWrite)
            {
                try
                {
                    pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                }
                catch { }
            }
        }
        FieldInfo[] finfos = type.GetFields(flags);
        foreach (var finfo in finfos)
        {
            finfo.SetValue(comp, finfo.GetValue(other));
        }
        return comp as T;
    }

    public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
    {
        return go.AddComponent<T>().GetCopyOf(toAdd) as T;
    }
    #endregion
    #endregion

    public static void ForeachEnumValue<T>(Action<T> ActionWithValue, Func<bool> BreakCondition = null)
    {
        foreach (T enumItem in Enum.GetValues(typeof(T)))
        {
            ActionWithValue(enumItem);
            if (BreakCondition != null && BreakCondition())
                return;
        }
    }

    #region Array utilities.
    /// <summary>
    /// Increments a number. If (number > arrayLength), number = 0.
    /// </summary>
    public static void LimitedIncrement(ref int index, int arrayLength)
    {
        index++;
        if (index >= arrayLength)
            index = 0;
    }
    /// <summary>
    /// Decrements a number. If (number < 0), number = arrayLength - 1.
    /// </summary>
    public static void LimitedDecrement(ref int index, int arrayLength)
    {
        index--;
        if (index < 0)
            index = arrayLength - 1;
    }

    public static void EnumerateFromEnd<T>(this T[] array, Action<T> ActionWithItem)
    {
        for (int i = array.Count() - 1; i >= 0; i--)
            ActionWithItem(array[i]);
    }

    public static void EnumerateFromEnd<T>(this T[] array, Action<int> ActionWithIndex)
    {
        for (int i = array.Count() - 1; i >= 0; i--)
            ActionWithIndex(i);
    }
    #endregion

    #region Dictionaries operations.
    public static KeyValuePair<TKey, TValue> SetKey<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, TKey key)
    {
        return new KeyValuePair<TKey, TValue>(key, kvp.Value);
    }

    public static KeyValuePair<TKey, TValue> SetValue<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, TValue value)
    {
        return new KeyValuePair<TKey, TValue>(kvp.Key, value);
    }

    public static Y GetOrCreateValue<T, Y>(this Dictionary<T, Y> dictionary, T key) where Y : new()
    {
        if (!dictionary.ContainsKey(key))
            dictionary.Add(key, new Y());
        return dictionary[key];
    }
    #endregion

    #region IsBetween.
    /// <summary>
    /// Returns whether a number is between a [inclusive] and b [inclusive].
    /// </summary>
    public static bool IsBetween(this float number, float a, float b)
    {
        return (number >= a && number <= b) || (number <= a && number >= b);
    }
    /// <summary>
    /// Returns whether a number is between vector.x [inclusive] and vector.y [inclusive].
    /// </summary>
    public static bool IsBetween(this float number, Vector2 minMax)
    {
        return (number >= minMax.x && number <= minMax.y) || (number <= minMax.x && number >= minMax.y);
    }
    /// <summary>
    /// Returns whether a number is between a [inclusive] and b [inclusive].
    /// </summary>
    public static bool IsBetween(this int number, float a, float b)
    {
        return (number >= a && number <= b) || (number <= a && number >= b);
    }
    /// <summary>
    /// Returns whether a number is between vector.x [inclusive] and vector.y [inclusive].
    /// </summary>
    public static bool IsBetween(this int number, Vector2 minMax)
    {
        return (number >= minMax.x && number <= minMax.y) || (number <= minMax.x && number >= minMax.y);
    }
    #endregion
    #region Remapping.
    /// <summary>
    /// Get the progression of a value between two numbers.
    /// </summary>
    public static float Progression(this float value, float from, float to)
    {
        return (value - from) / (to - from);
    }
    /// <summary>
    /// Get the progression of a value between two numbers.
    /// </summary>
    public static float Progression(this float value, Vector2 minMax)
    {
        return Progression(value, minMax.x, minMax.y);
    }

    /// <summary>
    /// Maps a value going from -1 to 1 to a value going from 0 to 1.
    /// </summary>
    /// <param name="value">Value to remap.</param>
    /// <returns></returns>
    public static float To01(this float value)
    {
        return (value + 1) * .5f;
    }
    /// <summary>
    /// Maps a Vector2 going from -1 to 1 to a Vector2 going from 0 to 1.
    /// </summary>
    /// <param name="value">Vector2 to remap.</param>
    /// <returns></returns>
    public static Vector2 To01(this Vector2 value)
    {
        return (value + Vector2.one) * .5f;
    }
    /// <summary>
    /// Maps a value going from 0 to 1 to a value going from -1 to 1.
    /// </summary>
    /// <param name="value">Value to remap.</param>
    /// <returns></returns>
    public static float ToMin1_1(this float value)
    {
        return value * 2 - 1;
    }
    /// <summary>
    /// Maps a value going from 0 to 1 to a value going from -1 to 1.
    /// </summary>
    /// <param name="value">Value to remap.</param>
    /// <returns></returns>
    public static Vector2 ToMin1_1(this Vector2 value)
    {
        return (value * 2) - Vector2.one;
    }

    public static int IndexFromProgression(float progression, int slices)
    {
        return (int)Mathf.Clamp(progression / (1 / (float)slices), 0, slices - 1);
    }
    #endregion

    public static float RoundDecimals(this float number, int decimalPlaces)
    {
        return (float)Math.Round(number, decimalPlaces);
    }

    #region Vector operations.
    #region Set Vector2 values.
    /// <summary>
    /// Changes X in the Vector and returns the Vector.
    /// </summary>
    public static Vector2 SetX(this Vector2 vector2, float value)
    {
        return new Vector2(value, vector2.y);
    }
    /// <summary>
    /// Changes Y in the Vector and returns the Vector.
    /// </summary>
    public static Vector2 SetY(this Vector2 vector2, float value)
    {
        return new Vector2(vector2.x, value);
    }
    #endregion
    #region Set Vector3 values.
    /// <summary>
    /// Changes X in the Vector and returns the Vector.
    /// </summary>
    public static Vector3 SetX(this Vector3 vector3, float value)
    {
        return new Vector3(value, vector3.y, vector3.z);
    }
    /// <summary>
    /// Changes Y in the Vector and returns the Vector.
    /// </summary>
    public static Vector3 SetY(this Vector3 vector3, float value)
    {
        return new Vector3(vector3.x, value, vector3.z);
    }
    /// <summary>
    /// Changes Z in the Vector and returns the Vector.
    /// </summary>
    public static Vector3 SetZ(this Vector3 vector3, float value)
    {
        return new Vector3(vector3.x, vector3.y, value);
    }
    #endregion

    public static Vector2 Vector2Center(Vector2 A, Vector2 B)
    {
        return ((B - A) * .5f) + A;
    }

    public static bool Contains(this Vector2 area, Vector3 position)
    {
        return position.x.IsBetween(-area.x * .5f, area.x * .5f) && position.z.IsBetween(-area.y * .5f, area.y * .5f);
    }

    public static T GetClosestItemFromPosition<T>(this List<T> items, Vector3 targetPosition, Func<bool, T> ValidItemCondition = null) where T : Component
    {
        if (items.Count < 1)
            return null;
        if (items.Count == 1)
        {
            if (ValidItemCondition == null || ValidItemCondition(items[0]))
                return items[0];
            else
                return null;
        }
        int closestIndex = 0;
        float smallestDistance = Vector3.Distance(items[closestIndex].transform.position, targetPosition);
        for (int i = 1; i < items.Count; i++)
            if ((ValidItemCondition == null || ValidItemCondition(items[0])) && Vector3.Distance(items[i].transform.position, targetPosition) < smallestDistance)
            {
                smallestDistance = Vector3.Distance(items[i].transform.position, targetPosition);
                closestIndex = i;
            }
        return items[closestIndex];
    }

    public static Vector2 PointOnCircle(float radius, float angle)
    {
        Vector2 pos;
        pos.x = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        pos.y = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        return pos;
    }
    #endregion

    #region Set Color values.
    /// <summary>
    /// Changes R in the Color and returns the Color.
    /// </summary>
    public static Color SetR(this Color color, float value)
    {
        return new Color(value, color.g, color.b, color.a);
    }
    /// <summary>
    /// Changes G in the Color and returns the Color.
    /// </summary>
    public static Color SetG(this Color color, float value)
    {
        return new Color(color.r, value, color.b, color.a);
    }
    /// <summary>
    /// Changes B in the Color and returns the Color.
    /// </summary>
    public static Color SetB(this Color color, float value)
    {
        return new Color(color.r, color.g, value, color.a);
    }
    /// <summary>
    /// Changes A in the Color and returns the Color.
    /// </summary>
    public static Color SetA(this Color color, float value)
    {
        return new Color(color.r, color.g, color.b, value);
    }
    #endregion

    #region String operations.
    /// <summary>
    /// Adds a space before single uppercase letters.
    /// </summary>
    /// <param name="inputObject"></param>
    /// <returns></returns>
    public static string ToTitle(this object inputObject)
    {
        string inputString = inputObject.ToString();
        if (string.IsNullOrEmpty(inputString))
            return inputString;
        if (inputString.Length < 2)
            return inputString;
        for (int i = 1; i < inputString.Length; i++)
            if (char.IsUpper(inputString[i]) && (!char.IsUpper(inputString[i - 1]) || (i != inputString.Length - 1 && !char.IsUpper(inputString[i + 1]))))
            {
                inputString = inputString.Insert(i, " ");
                i++;
            }
        return inputString;
    }

    /// <summary>
    /// Returns the first word of a string with whitespaces.
    /// </summary>
    public static string FirstWord(this string input)
    {
        return input.Split(' ')[0];
    }

    #region ToStringMK.
    /// <summary>
    /// Converts an large number: 10000000 becomes 10M, 1500 becomes 1.5K.
    /// </summary>
    /// <returns>The converted number.</returns>
    public static string ToStringMK(this int number) { return ((long)number).ToStringMK(); }
    /// <summary>
    /// Converts an large number: 10000000 becomes 10M, 1500 becomes 1.5K.
    /// </summary>
    /// <returns>The converted number.</returns>
    public static string ToStringMK(this long number)
    {
        // Get the suffix that we need.
        string stringNumber = number.ToString();
        string suffix = "";
        if (stringNumber.Length > 6)
            suffix = "M";
        else if (stringNumber.Length > 3)
            suffix = "K";
        else
            return stringNumber;

        // Get the sliced number with commas.
        string slicedNumber = stringNumber.Substring(0, stringNumber.Length - (suffix == "M" ? 6 : 3));
        if (slicedNumber.Length > 4)
            return float.Parse(slicedNumber).ToString("n", new System.Globalization.NumberFormatInfo { NumberGroupSeparator = " " }).Replace(".00", string.Empty)
                + suffix;
        slicedNumber += "." + stringNumber.Substring(slicedNumber.Length - 1, 2);
        slicedNumber = float.Parse(slicedNumber).ToString("n", new System.Globalization.NumberFormatInfo { NumberGroupSeparator = " " }) + suffix;
        return slicedNumber;
    }
    #endregion

    /// <summary>
    /// Get all the content after the separator.
    /// </summary>
    /// <param name="value">String to get the content from.</param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static string SplitLast(this string value, char separator)
    {
        string[] strings = value.Split(separator);
        return strings[strings.Length - 1];
    }

    public static string CapitalizeFirstLetter(this string input)
    {
        return char.ToUpper(input[0]) + input.Substring(1);
    }

    public static char ToABC(this int index)
    {
        return (char)(65 + index);
    }

    public static int FromABC(this char @char)
    {
        return @char - 65;
    }
    #endregion

    public static float Average(this float[] floats)
    {
        return floats.ToList().Average();
    }

    public static float Average(List<float> floats)
    {
        if (floats.Count == 0)
            return 0;
        float sum = 0;
        for (int i = 0; i < floats.Count - 1; i++)
            sum = floats[i + 1] - floats[i];
        return sum / floats.Count;
    }

    /// <summary>
    /// Return 1 if true or -1 if false.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int Direction(this bool value)
    {
        return value ? 1 : -1;
    }

    public static bool EqualsAny<T>(this T value, params T[] values)
    {
        foreach (var v in values)
            if (value.Equals(v))
                return true;
        return false;
    }

    #region Screen RaycastAll to object.
    public static void ScreenRaycastAllToGameObject(GameObject gameObject, Camera cam, out RaycastHit? hitResult)
    {
        RaycastHit[] hits = Physics.RaycastAll(cam.ScreenPointToRay(Input.mousePosition));
        if (hits.Length > 0)
            foreach (RaycastHit hit in hits)
                if (hit.collider.gameObject == gameObject)
                {
                    hitResult = hit;
                    return;
                }
        hitResult = null;
    }

    public static bool IsGameObjectUnderMouse(GameObject gameObject, Camera cam)
    {
        RaycastHit? hitResult;
        ScreenRaycastAllToGameObject(gameObject, cam, out hitResult);
        return hitResult != null;
    }
    #endregion

    /// <summary>
    /// Clears all sources and adds a new source with transform.
    /// </summary>
    /// <returns>The new constraint.</returns>
    public static ConstraintSource SetSource(this IConstraint constraint, Transform transform)
    {
        ConstraintSource source = new ConstraintSource();
        source.sourceTransform = transform;
        source.weight = 1;
        constraint.SetSources(new List<ConstraintSource>());
        constraint.AddSource(source);
        return source;
    }

    #region Find and invoke a method by name.
    public static bool FindAndInvokeMethod<T>(this MonoBehaviour target, string methodName)
    {
        // Try to get the method and call it if we found it.
        MethodInfo targetMethod = target.GetType().GetMethod(methodName);
        if (targetMethod == null)
            return false;
        targetMethod.Invoke(target, null);
        return true;
    }

    public static bool FindAndInvokeMethod<T, U>(this T target, string methodName, U parameter)
    {
        // Try to get the method of type U.
        MethodInfo targetMethod = target.GetType().GetMethod(methodName, new Type[] { typeof(U) });
        if (targetMethod != null)
        {
            targetMethod.Invoke(target, new object[] { parameter });
            return true;
        }

        // Try to get the method by its name only, and send it our parameter.
        try
        {
            targetMethod = target.GetType().GetMethod(methodName);
            targetMethod.Invoke(target, new object[] { parameter });
            return true;
        }
        catch { return false; }
    }
    #endregion

    public static void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public static bool IsSceneLoaded(int sceneBuildIndex)
    {
        int countLoaded = SceneManager.sceneCount;

        for (int i = 0; i < countLoaded; i++)
            if (SceneManager.GetSceneAt(i).buildIndex == sceneBuildIndex)
                return true;

        return false;
    }

    public static float GetVolume(this Bounds bounds)
    {
        return bounds.size.x * bounds.size.y * bounds.size.z;
    }

    #region Maintain scale.
    public static void MaintainScale(Transform transform, float targetScale) => MaintainScale(transform, Vector3.one * targetScale);
    public static void MaintainScale(Transform transform, Vector3 targetScale)
    {
        transform.localScale = Vector3.one;
        transform.localScale = MathUtilities.Divide(targetScale, transform.lossyScale);
    }
    public static void MaintainScale(Transform transform, float targetScale, bool x, bool y, bool z)
    {
        Vector3 previousLocalScale = transform.localScale;
        transform.localScale = Vector3.one;
        Vector3 targetLocalScale = MathUtilities.Divide(Vector3.one * targetScale, transform.lossyScale);
        transform.localScale = new Vector3(x ? targetLocalScale.x : previousLocalScale.x, y ? targetLocalScale.y : previousLocalScale.y, z ? targetLocalScale.z : previousLocalScale.z);
    }
    #endregion
}