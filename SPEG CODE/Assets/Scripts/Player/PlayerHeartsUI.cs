using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    private PlayerStats _wiredHealthSource;

    private void Awake()
    {
        if (_heartTemplate != null)
            _heartTemplate.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        WireSubscription();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnwireSubscription();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        WireSubscription();
    }

    private void ResolvePlayerStats()
    {
        if (PlayerController.Instance != null)
            _playerStats = PlayerController.Instance.GetComponent<PlayerStats>();
    }

    private void WireSubscription()
    {
        UnwireSubscription();
        ResolvePlayerStats();
        if (_playerStats == null || _heartTemplate == null || _heartsContainer == null) return;

        _playerStats.HealthChanged += OnHealthChanged;
        _wiredHealthSource = _playerStats;
        BuildHearts(_playerStats.MaxHealth);
        Refresh(_playerStats.CurrentHealth, _playerStats.MaxHealth);
    }

    private void UnwireSubscription()
    {
        var s = _wiredHealthSource;
        _wiredHealthSource = null;
        if (s != null)
            s.HealthChanged -= OnHealthChanged;
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
            var img = _heartImages[i];
            if (img != null)
                DestroyImmediate(img.gameObject);
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
