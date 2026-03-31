using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHeartsUI : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private RectTransform _heartsContainer;
    [SerializeField] private Image _heartTemplate;
    [SerializeField] private Sprite _fullHeartSprite;
    [SerializeField] private Sprite _emptyHeartSprite;
    [SerializeField] private int _healthPerHeart = 1;

    private List<Image> _heartImages = new List<Image>();
    private int _maxHealthShown = -1;

    private void Awake()
    {
        if (_playerStats == null && PlayerController.Instance != null)
        {
            _playerStats = PlayerController.Instance.GetComponent<PlayerStats>();
        }

        _heartTemplate.gameObject.SetActive(false);

        BuildHearts(_playerStats.MaxHealth);
        Refresh(_playerStats.CurrentHealth, _playerStats.MaxHealth);
    }

    private void OnEnable()
    {
        _playerStats.HealthChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        _playerStats.HealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int currentHealth, int maxHealth)
    {
        if (maxHealth != _maxHealthShown)
        {
            BuildHearts(maxHealth);
        }
        Refresh(currentHealth, maxHealth);
    }

    private void BuildHearts(int maxHealth)
    {
        for (int i = _heartImages.Count - 1; i >= 0; i--)
        {
            Destroy(_heartImages[i].gameObject);
        }
        _heartImages.Clear();

        int heartCount = (maxHealth + _healthPerHeart - 1) / _healthPerHeart;
        for (int i = 0; i < heartCount; i++)
        {
            Image heart = Instantiate(_heartTemplate, _heartsContainer);
            heart.gameObject.name = "Heart_" + i;
            heart.gameObject.SetActive(true);
            _heartImages.Add(heart);
        }

        _maxHealthShown = maxHealth;
    }

    private void Refresh(int currentHealth, int maxHealth)
    {
        int clampedHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        int fullHeartCount = (clampedHealth + _healthPerHeart - 1) / _healthPerHeart;
        for (int i = 0; i < _heartImages.Count; i++)
        {
            _heartImages[i].sprite = i < fullHeartCount ? _fullHeartSprite : _emptyHeartSprite;
        }
    }
}
