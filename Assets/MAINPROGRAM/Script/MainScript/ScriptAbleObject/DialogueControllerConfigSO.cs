using Characters;
using TMPro;
using UnityEngine;

namespace DIALOGUE
{
    [CreateAssetMenu(fileName = "Dialog Container Configuration", menuName = "Dialogue Contoller/Dialogue Configurantion Asset")]

    public class DialogueControllerConfigSO : ScriptableObject
    {
        public const float Default_FontSize_Dialogue = 11;
        public const float Default_FontSize_Name = 18;

        public CharacterConfigSO characterConfigationAsset;

        public Color defaultTextColour =  Color.white; 
        public TMP_FontAsset defaulFont;

        public float dialogueFontScale = 1f;
        public float defaultFontSize = Default_FontSize_Dialogue;
        public float defaultNameFontSize = Default_FontSize_Name;
    }
}