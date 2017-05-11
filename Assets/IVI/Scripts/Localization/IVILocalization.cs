using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class IVILocalization {
	public static readonly SystemLanguage DEFAULT_LANGUAGE = SystemLanguage.English;
	protected static SystemLanguage language = DEFAULT_LANGUAGE;
	public static SystemLanguage Language { 
		get { return language; } 
		set { 
			bool isValid;
			language = Choose2<SystemLanguage>(lang:value, isValid:out isValid, en:SystemLanguage.English, fr:SystemLanguage.French);
			if (!isValid) {
				language = DEFAULT_LANGUAGE;
				Debug.LogWarning (
					"Attempted to change language to " + value.ToString() 
					+ ", which is unsupported. Reverting back to " + DEFAULT_LANGUAGE.ToString()
				);
			}
		}
	}
	protected static T Choose2<T>(SystemLanguage lang, out bool isValid, T en, T fr) {
		isValid = true;
		switch (lang) {
		case SystemLanguage.English: return en;
		case SystemLanguage.French:  return fr;
		}
		isValid = false;
		Debug.LogWarning ("The language " + lang.ToString() + " is not supported!");
		return default(T);
	}
	protected static T Choose<T>(T en, T fr) {
		bool isValid;
		return Choose2<T>(lang:Language, isValid:out isValid, en:en, fr:fr);
	}
	protected static string ChooseText(string en, string fr) {
		return Choose<string>(en:en, fr:fr);
	}
	protected static AudioClip ChooseAudio(AudioClip en, AudioClip fr) {
		return Choose<AudioClip>(en:en, fr:fr);
	}
	protected static Texture ChooseTexture(Texture en, Texture fr) {
		return Choose<Texture>(en:en, fr:fr);
	}
}

public partial class IVILocalization {
	public static string GetText_FileExplorerMode() {
		return ChooseText(fr: "Explorateur de fichier", en: "File Explorer");
	}
	public static string GetText_Cut() {
		return ChooseText(fr: "Couper et coller", en: "Cut & paste");
	}
	public static string GetText_Copy() {
		return ChooseText(fr: "Copier et coller", en: "Copy & paste");
	}
	public static string GetText_Select() {
		return ChooseText(fr: "Selectionner", en: "Select");
	}
	public static string GetText_ThisItemCosts(string itemName, uint price) {
		return ChooseText(
			fr: "Un " + itemName + " coûte " + price + " points. Voulez-vous l'acheter ?",
			en: "Would you like to buy one " + itemName + ", which costs " + price + "?"
		);
	}
}
