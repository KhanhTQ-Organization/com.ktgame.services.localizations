using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;
using TMPro;

namespace com.ktgame.services.localizations
{
	public class I2Localizer
	{
		public string CurrentLanguage => LocalizationManager.CurrentLanguage;

		public string CurrentLanguageCode => LocalizationManager.CurrentLanguageCode;

		public List<string> AvailableLanguages => LocalizationManager.GetAllLanguages();

		private readonly TMP_FontAsset _mainFontAsset;

		public I2Localizer()
		{
			_mainFontAsset = TMP_Settings.defaultFontAsset;
			MainFontAssetChanged(LocalizationManager.CurrentLanguage);
		}

		public void SetLanguage(string language)
		{
			MainFontAssetChanged(language);
			var code = LocalizationManager.GetLanguageCode(language);
			LocalizationManager.SetLanguageAndCode(language, code, true, true);
		}

		public string GetLocalizedString(string id)
		{
			return LocalizationManager.GetTranslation(id);
		}

		public string GetLocalizedString(string id, params object[] parameters)
		{
			var translation = LocalizationManager.GetTranslation(id);
			if (string.IsNullOrEmpty(translation)) return string.Empty;
			return string.Format(translation, parameters);
		}

		private void MainFontAssetChanged(string language)
		{
			if (_mainFontAsset == null)
			{
				return;
			}

			for (var i = _mainFontAsset.fallbackFontAssetTable.Count - 1; i >= 0; i--)
			{
				var fallbackFontAssetExist = _mainFontAsset.fallbackFontAssetTable[i];
				if (fallbackFontAssetExist.atlasPopulationMode == AtlasPopulationMode.Static)
				{
					_mainFontAsset.fallbackFontAssetTable.Remove(fallbackFontAssetExist);
					Resources.UnloadAsset(fallbackFontAssetExist);
				}
			}

			LoadFontAsync(language).Forget();
		}

		private async UniTaskVoid LoadFontAsync(string language)
		{
			var request = Resources.LoadAsync<TMP_FontAsset>($"{TMP_Settings.defaultFontAssetPath}/{language}");
			await request;
			
			var fallbackFontAsset = request.asset as TMP_FontAsset;
			if (fallbackFontAsset != null && fallbackFontAsset != _mainFontAsset)
			{
				_mainFontAsset.fallbackFontAssetTable.Add(fallbackFontAsset);
				_mainFontAsset.fallbackFontAssetTable.Sort(new TMPFontAssetComparable());
				TMPro_EventManager.ON_FONT_PROPERTY_CHANGED(true, _mainFontAsset);
			}
		}

		private class TMPFontAssetComparable : IComparer<TMP_FontAsset>
		{
			public int Compare(TMP_FontAsset x, TMP_FontAsset y)
			{
				var isStatic1 = x != null && x.atlasPopulationMode == AtlasPopulationMode.Static;
				var isStatic2 = y != null && y.atlasPopulationMode == AtlasPopulationMode.Static;

				if (isStatic1 == isStatic2)
				{
					return 0;
				}

				return isStatic1 ? -1 : 1;
			}
		}
	}
}
