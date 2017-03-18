using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
	// TODO REVIEW
	// Have material live under text
	// move stencil mask into effects *make an efects top level element like there is
	// paragraph and character

	/// <summary>
	/// Editor class used to edit UI Labels.
	/// </summary>

	[CustomEditor (typeof(HandwrittenText), true)]
	[CanEditMultipleObjects]
	public class HandwrittenTextEditor : TextEditor
	{
		SerializedProperty numberOfLettersToShow;

		protected override void OnEnable ()
		{
			base.OnEnable ();
			numberOfLettersToShow = serializedObject.FindProperty ("numberOfLettersToShow");
		}

		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();
			serializedObject.Update ();
			EditorGUILayout.PropertyField (numberOfLettersToShow);
			serializedObject.ApplyModifiedProperties ();
		}
	}
}
