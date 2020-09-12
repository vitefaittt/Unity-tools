SerializedObject so = new SerializedObject(this);
SerializedProperty ratiosProperty = so.FindProperty("ratios");
EditorGUILayout.PropertyField(ratiosProperty);
so.ApplyModifiedProperties();