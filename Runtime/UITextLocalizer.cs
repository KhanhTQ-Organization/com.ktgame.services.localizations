using System;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;
using UnityEngine.Events;

namespace com.ktgame.services.localizations
{
	public class UITextLocalizer : MonoBehaviour, ILocalizable
	{
		[SerializeField] private Text textComponent;
		[SerializeField] private LocalizedString id;
		[SerializeField] private UnityEvent onLocalized;

		private void Awake()
		{
			if (textComponent == null)
			{
				textComponent = GetComponent<Text>();
			}
		}

		private void OnEnable()
		{
			LocalizationManager.OnLocalizeEvent += OnLocalizing;
			OnLocalizing();
		}

		private void OnDisable()
		{
			LocalizationManager.OnLocalizeEvent -= OnLocalizing;
		}

		public void OnLocalizing()
		{
			if (!string.IsNullOrEmpty(id))
			{
				// var text = LocalizationManager.GetTranslation(id);
				textComponent.text = id;
			}

			onLocalized?.Invoke();
		}
	}
}
