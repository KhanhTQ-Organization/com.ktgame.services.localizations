using TMPro;
using I2.Loc;
using UnityEngine;
using UnityEngine.Events;

namespace com.ktgame.services.localizations
{
	public class UITextMeshProLocalizer : MonoBehaviour, ILocalizable
	{
		[SerializeField] private TextMeshProUGUI textComponent;
		[SerializeField] private LocalizedString id;
		[SerializeField] private UnityEvent onLocalized;

		private void Awake()
		{
			if (textComponent == null)
			{
				textComponent = GetComponent<TextMeshProUGUI>();
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
				textComponent.SetText(id);
			}

			onLocalized?.Invoke();
		}
	}
}
