using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ShowIfAttribute : PropertyAttribute
{
	public string preficate = "";
	public ShowIfAttribute(string preficate)
	{
		this.preficate = preficate;
	}
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfPropertyDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		if (ShouldShow((ShowIfAttribute)attribute, property))
			return EditorGUI.GetPropertyHeight(property, label);
		else
			return -EditorGUIUtility.standardVerticalSpacing;
	}
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (ShouldShow((ShowIfAttribute)attribute, property))
			EditorGUI.PropertyField(position, property, label, true);
	}
	private List<string> Tokenize(string path)
	{
		List<string> tokens = new();
		string token = "";
		int i = 0;

		while (i < path.Length)
		{
			if (char.IsLetter(path[i]) || path[i] == '_')
			{
				while (char.IsLetterOrDigit(path[i]) || path[i] == '_')
				{
					token += path[i];
					if (++i >= path.Length)
						break;
				}
			}
			else if (char.IsDigit(path[i]))
			{
				while (char.IsDigit(path[i]))
				{
					token += path[i];
					if (++i >= path.Length)
						break;
				}
			}
			else
			{
				token += path[i];
				++i;
			}

			tokens.Add(token);
			token = "";
		}

		return tokens;
	}
	private bool ShouldShow(ShowIfAttribute attribute, SerializedProperty property)
	{
		try
		{
			object parentObject = property.serializedObject.targetObject;

			var parentPath = property.propertyPath.Substring(0, property.propertyPath.LastIndexOf('.'));

			var tokens = Tokenize(parentPath);

			for (int i = 0; i < tokens.Count; ++i)
			{
				if (tokens[i] == "Array")
				{
					i += 4; // skip "Array.data["
					parentObject = ((IEnumerable<object>)parentObject).ElementAt(int.Parse(tokens[i]));
					i += 1; // skip number and "]"
				}
				else
				{
					parentObject = parentObject.GetType().GetField(tokens[i]).GetValue(parentObject);
					++i; // skip name
				}
			}

			return (bool)parentObject.GetType().GetMethod(attribute.preficate).Invoke(parentObject, Array.Empty<object>());
		}
		catch (Exception e)
		{
			Debug.LogError($"Could not evaluate ShowIf attribute: {e}");
			return true;
		}

	}
}
#endif
