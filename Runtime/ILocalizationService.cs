using System;
using System.Collections.Generic;
using com.ktgame.core;

namespace com.ktgame.services.localizations
{
	public interface ILocalizationService : IService, IInitializable
	{
		event Action OnLanguageChanged;

		string CurrentLanguage { get; }

		string CurrentLanguageCode { get; }

		List<string> AvailableLanguages { get; }

		void SetLanguage(string language);

		string GetLocalizedString(string id);

		string GetLocalizedString(string id, params object[] parameters);
	}
}
