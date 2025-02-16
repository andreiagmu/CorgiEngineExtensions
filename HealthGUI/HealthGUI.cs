using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace HealthGUI
{
    public sealed class HealthGUI : MonoBehaviour, MMEventListener<CorgiEngineEvent>
    {
        [Tooltip("how much health a heart represents")]
        public float HeartValue = 10;
        [Tooltip("the sprite to use when the heart is full")]
        public Sprite HeartFull;
        [Tooltip("show empty hearts or remove them?")]
        public bool ShowEmptyHearts = true;
        [Tooltip("the sprite to use when the heart is empty")] [MMCondition("ShowEmptyHearts")]
        public Sprite HeartEmpty;
        [Tooltip("the size of the heart to display")]
        public Vector2 HeartSize = new Vector2(50, 50);
        
        private Health _health;
        private List<Image> _hearts;

        private void OnEnable()
        {
            if (LevelManager.HasInstance && LevelManager.Instance.Players != null) Initialize();
            this.MMEventStartListening();
        }
        
        private void OnDisable()
        {
            this.MMEventStopListening();
            _health.OnHit -= UpdateHearts;
            _health.OnRevive -= UpdateHearts;
        }
        
        private void Initialize()
        {
            _health = LevelManager.Instance.Players[0].GetComponent<Health>();
            _health.OnHit += UpdateHearts;
            _health.OnRevive += UpdateHearts;
            var hearts = (int)Mathf.Ceil(_health.MaximumHealth / HeartValue);
            _hearts = new List<Image>();
            foreach (Transform child in transform) Destroy(child.gameObject);

            for (var i = 0; i < hearts; i++)
            {
                var heart = new GameObject();
                heart.transform.SetParent(transform);
                heart.name = "Heart" + i;

                var heartImage = heart.AddComponent<Image>();
                heartImage.sprite = HeartFull;

                heart.MMGetComponentNoAlloc<RectTransform>().localScale = Vector3.one;
                heart.MMGetComponentNoAlloc<RectTransform>().sizeDelta = HeartSize;

                _hearts.Add(heartImage);
            }
            
            UpdateHearts();
        }

        private void UpdateHearts()
        {
            var fullHearts = (int)Mathf.Ceil(_health.CurrentHealth / HeartValue);
            var hearts = (int)Mathf.Ceil(_health.MaximumHealth / HeartValue);
            for (var i = 0; i < fullHearts; i++)
            {
                _hearts[i].sprite = HeartFull;
                _hearts[i].enabled = true;
            }
            for (var i = fullHearts; i < hearts; i++)
            {
                _hearts[i].sprite = HeartEmpty;
                _hearts[i].enabled = ShowEmptyHearts;
            }
        }

        public void OnMMEvent(CorgiEngineEvent corgiEngineEvent)
        {
            if (corgiEngineEvent.EventType == CorgiEngineEventTypes.LevelStart)
                Initialize();
        }
    }
}
