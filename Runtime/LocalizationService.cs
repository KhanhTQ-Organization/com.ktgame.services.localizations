using System;
using System.Collections.Generic;
using com.ktgame.core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace com.ktgame.services.localizations
{
	[Service(typeof(ILocalizationService))]
	public class LocalizationService : MonoBehaviour, ILocalizationService
	{
		public int Priority => 0;

		public bool Initialized { get; set; }

		public event Action OnLanguageChanged;

		public string CurrentLanguage => _localizer.CurrentLanguage;

		public string CurrentLanguageCode => _localizer.CurrentLanguageCode;

		public List<string> AvailableLanguages => _localizer.AvailableLanguages;

		private I2Localizer _localizer;

		public UniTask OnInitialize(IArchitecture architecture)
		{
			_localizer = new I2Localizer();
			Initialized = true;
			return UniTask.CompletedTask;
		}

		public void SetLanguage(string language)
		{
			_localizer.SetLanguage(language);
			OnLanguageChanged?.Invoke();
		}

		public string GetLocalizedString(string id)
		{
			return _localizer.GetLocalizedString(id);
		}

		public string GetLocalizedString(string id, params object[] parameters)
		{
			return _localizer.GetLocalizedString(id, parameters);
		}
	}
}
