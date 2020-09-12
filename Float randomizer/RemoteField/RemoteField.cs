using System;
using System.Reflection;
using UnityEngine;

public abstract class RemoteField<T>
{
    public GameObject targetGameObject;
    public Component targetComponent;
    string hiddenFieldName;
    [SerializeField]
    string fieldName = "";

    string[] splitFieldName;
    MemberInfo[] memberInfo;

    char[] split = { '.' };

    void GetFieldInfo()
    {
        hiddenFieldName = fieldName;
        if (targetComponent == null)
        {
            memberInfo = new MemberInfo[0];
            return;
        }
        splitFieldName = fieldName.Split(split);
        if (splitFieldName.Length < 1)
            return;
        memberInfo = new MemberInfo[splitFieldName.Length];
        var type = targetComponent.GetType();
        for (int i = 0; i < memberInfo.Length; i++)
        {
            MemberInfo[] tempArray = type.GetMember(splitFieldName[i], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (tempArray.Length < 1)
                return;
            memberInfo[i] = tempArray[0];
            if (tempArray[0].MemberType == MemberTypes.Property)
                type = ((PropertyInfo)tempArray[0]).PropertyType;
            else if (tempArray[0].MemberType == MemberTypes.Field)
                type = ((FieldInfo)tempArray[0]).FieldType;
            else
                return;
        }
    }

    #region Get/set the value.
    T value;
    public T GetValue()
    {
        value = default(T);
        if (fieldName != hiddenFieldName)
            GetFieldInfo();
        object currentTarget = targetComponent;

        for (int i = 0; i < memberInfo.Length; i++)
        {
            if (memberInfo[i] == null)
                return default(T);

            if (memberInfo[i].MemberType == MemberTypes.Field)
                currentTarget = ((FieldInfo)memberInfo[i]).GetValue(currentTarget);
            else if (memberInfo[i].MemberType == MemberTypes.Property)
                currentTarget = ((PropertyInfo)memberInfo[i]).GetValue(currentTarget, null);
            else
                return default(T);

            if (currentTarget == null)
                break;
        }

        try
        {
            value = (T)Convert.ChangeType(currentTarget, typeof(T));
        }
        catch { }

        return value;
    }

    public void SetValue(T value)
    {
        if (fieldName != hiddenFieldName)
            GetFieldInfo();
        object currentTarget = targetComponent;

        for (int i = 0; i < memberInfo.Length; i++)
        {
            if (memberInfo[i].MemberType == MemberTypes.Field)
                currentTarget = ((FieldInfo)memberInfo[i]).GetValue(currentTarget);
            else if (memberInfo[i].MemberType == MemberTypes.Property)
                currentTarget = ((PropertyInfo)memberInfo[i]).GetValue(currentTarget, null);

            if (currentTarget == null)
                break;
        }

        try
        {
            if (memberInfo[memberInfo.Length - 1].MemberType == MemberTypes.Field)
                ((FieldInfo)memberInfo[memberInfo.Length - 1]).SetValue(targetComponent, value);
            else if (memberInfo[memberInfo.Length - 1].MemberType == MemberTypes.Property)
                ((PropertyInfo)memberInfo[memberInfo.Length - 1]).SetValue(targetComponent, value);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    #endregion
}

[Serializable]
public class RemoteFloatField : RemoteField<float> { }