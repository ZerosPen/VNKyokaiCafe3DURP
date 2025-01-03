using AYellowpaper.SerializedCollections;
using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Characters
{
    [System.Serializable]
    public class CharacterConfigData
    {
        public string name;
        public string alias;
        public Character.CharacterType CharType;

        public Color nameColor;
        public Color dialogueColor;

        public TMP_FontAsset nameFont;
        public TMP_FontAsset dialogueFont;

        public float FontNameScale = 1f;
        public float DialogueFontScale = 1f;

        [SerializedDictionary("Path / ID", "Sprite")]
        public SerializedDictionary<string, Sprite> sprites = new SerializedDictionary<string, Sprite>();
         
        public CharacterConfigData Copy()
        {
            CharacterConfigData result = new CharacterConfigData();

            result.name = name;
            result.alias = alias;
            result.CharType = CharType;
            result.nameFont = nameFont;
            result.dialogueFont = dialogueFont;

            result.nameColor = new Color(nameColor.r, nameColor.g, nameColor.b, nameColor.a);
            result.dialogueColor = new Color(dialogueColor.r, dialogueColor.g, dialogueColor.b, dialogueColor.a);

            result.DialogueFontScale = DialogueFontScale;
            result.FontNameScale = FontNameScale;

            return result;
        }

        private static Color defaultColor => DialogController.Instance.config.defaultTextColour;

        private static TMP_FontAsset defaultFont  => DialogController.Instance.config.defaulFont;

        public static CharacterConfigData Default
        {
            get
            {
                CharacterConfigData result = new CharacterConfigData();

                result.name = "";
                result.alias = "";
                result.CharType = Character.CharacterType.text;
                result.nameFont = defaultFont;
                result.dialogueFont = defaultFont;

                result.nameColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, defaultColor.a);
                result.dialogueColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, defaultColor.a);

                result.DialogueFontScale = 1f;
                result.FontNameScale = 1f;

                return result;
            }
        }
    }
}