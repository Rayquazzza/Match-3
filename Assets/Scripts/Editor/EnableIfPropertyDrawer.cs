using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnableIfAttribute))]
public class EnableIfPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //get the attribute used on the property
        EnableIfAttribute enableIFAttribute = attribute as EnableIfAttribute;
        SerializedProperty activatorProperty = property.serializedObject.FindProperty(enableIFAttribute.FieldName);
        bool IsEnabled = false;


        if (activatorProperty != null)
        {
            IsEnabled = activatorProperty.boolValue;
        }
        else
        {
            bool didFoundValue = false;
            foreach (FieldInfo fieldInfo in property.serializedObject.targetObjects.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (fieldInfo.Name == enableIFAttribute.FieldName)
                {
                    IsEnabled = (bool)fieldInfo.GetValue(property.serializedObject.targetObject);
                    didFoundValue = true;
                    break;
                }
            }

            if (!didFoundValue)
            {
                foreach (PropertyInfo fieldInfo in property.serializedObject.targetObjects.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (fieldInfo.Name == enableIFAttribute.FieldName)
                    {
                        IsEnabled = (bool)fieldInfo.GetValue(property.serializedObject.targetObject);
                        didFoundValue = true;
                        break;
                    }
                }
            }

            if (!didFoundValue)
            {
                Debug.LogWarning("Missing Value" + enableIFAttribute.FieldName);
            }
        }

        GUI.enabled = IsEnabled;
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = true;
    }
}