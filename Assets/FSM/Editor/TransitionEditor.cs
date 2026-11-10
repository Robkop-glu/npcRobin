using FSM;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Transition))]
public class TransitionDrawer : PropertyDrawer
{
    private Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0f;

        var stateMachine = property.serializedObject.targetObject as Statemachine;
        int stateCount = stateMachine?.States?.Count ?? 0;

        bool expanded = GetFoldout(property);

        SerializedProperty iterator = property.Copy();
        SerializedProperty end = iterator.GetEndProperty();

        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, end))
        {
            enterChildren = false;

            if (iterator.name == "from")
            {
                height += EditorGUIUtility.singleLineHeight; // foldout

                if (expanded)
                    height += stateCount * EditorGUIUtility.singleLineHeight;
            }
            else
            {
                height += EditorGUI.GetPropertyHeight(iterator, true);
            }

            height += EditorGUIUtility.standardVerticalSpacing;
        }

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var stateMachine = property.serializedObject.targetObject as Statemachine;

        SerializedProperty iterator = property.Copy();
        SerializedProperty end = iterator.GetEndProperty();

        float y = position.y;
        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, end))
        {
            enterChildren = false;

            float height = EditorGUI.GetPropertyHeight(iterator, true);
            Rect rect = new Rect(position.x, y, position.width, height);

            if (iterator.name == "from")
            {
                DrawFromStates(rect, property, iterator, stateMachine);
                y += GetFromHeight(property, stateMachine);
            }
            else if (iterator.name == "to")
            {
                DrawToState(rect, iterator, stateMachine);
                y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            else
            {
                EditorGUI.PropertyField(rect, iterator, true);
                y += height + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        EditorGUI.EndProperty();
    }

    private void DrawFromStates(Rect position, SerializedProperty rootProperty, SerializedProperty fromProp, Statemachine stateMachine)
    {
        if (stateMachine == null || stateMachine.States == null)
        {
            EditorGUI.LabelField(position, "No StateMachine found");
            return;
        }

        var states = stateMachine.States;

        float y = position.y;

        bool expanded = GetFoldout(rootProperty);
        expanded = EditorGUI.Foldout(
            new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight),
            expanded,
            "From States",
            true
        );
        SetFoldout(rootProperty, expanded);

        y += EditorGUIUtility.singleLineHeight;

        if (expanded)
        {
            for (int i = 0; i < states.Count; i++)
            {
                var state = states[i];
                if (state == null) continue;

                bool isSelected = Contains(fromProp, state);

                EditorGUI.BeginChangeCheck();

                bool newSelected = EditorGUI.ToggleLeft(
                    new Rect(position.x + 15, y, position.width - 15, EditorGUIUtility.singleLineHeight),
                    state.name,
                    isSelected
                );

                if (EditorGUI.EndChangeCheck())
                {
                    if (newSelected)
                        AddToArray(fromProp, state);
                    else
                        RemoveFromArray(fromProp, state);
                }

                y += EditorGUIUtility.singleLineHeight;
            }
        }
    }

    private void DrawToState(Rect position, SerializedProperty toProp, Statemachine stateMachine)
    {
        if (stateMachine == null || stateMachine.States == null)
        {
            EditorGUI.PropertyField(position, toProp);
            return;
        }

        var states = stateMachine.States;
        string[] options = states.Select(s => s != null ? s.name : "Null").ToArray();

        Object current = toProp.objectReferenceValue;
        int index = states.IndexOf(current as StateBase);
        if (index < 0) index = 0;

        EditorGUI.BeginChangeCheck();

        index = EditorGUI.Popup(position, "To", index, options);

        if (EditorGUI.EndChangeCheck())
        {
            if (states.Count > 0)
                toProp.objectReferenceValue = states[index];
        }
    }

    private float GetFromHeight(SerializedProperty property, Statemachine stateMachine)
    {
        int count = stateMachine?.States?.Count ?? 0;
        bool expanded = GetFoldout(property);

        float height = EditorGUIUtility.singleLineHeight;

        if (expanded)
            height += count * EditorGUIUtility.singleLineHeight;

        return height + EditorGUIUtility.standardVerticalSpacing;
    }

    private bool GetFoldout(SerializedProperty property)
    {
        if (!foldoutStates.TryGetValue(property.propertyPath, out bool state))
        {
            state = false;
            foldoutStates[property.propertyPath] = state;
        }
        return state;
    }

    private void SetFoldout(SerializedProperty property, bool value)
    {
        foldoutStates[property.propertyPath] = value;
    }

    private bool Contains(SerializedProperty arrayProp, Object value)
    {
        for (int i = 0; i < arrayProp.arraySize; i++)
        {
            if (arrayProp.GetArrayElementAtIndex(i).objectReferenceValue == value)
                return true;
        }
        return false;
    }

    private void AddToArray(SerializedProperty arrayProp, Object value)
    {
        int index = arrayProp.arraySize;
        arrayProp.InsertArrayElementAtIndex(index);
        arrayProp.GetArrayElementAtIndex(index).objectReferenceValue = value;
    }

    private void RemoveFromArray(SerializedProperty arrayProp, Object value)
    {
        for (int i = 0; i < arrayProp.arraySize; i++)
        {
            if (arrayProp.GetArrayElementAtIndex(i).objectReferenceValue == value)
            {
                arrayProp.DeleteArrayElementAtIndex(i);
                return;
            }
        }
    }
}