using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image _healthFill;
        [SerializeField] private Color _fullHealthColor = Color.green;
        [SerializeField] private Color _midHealthColor = Color.yellow;
        [SerializeField] private Color _lowHealthColor = Color.red;

        public void UpdateHealth(float percentage)
        {
            if (_healthFill != null)
            {
                _healthFill.fillAmount = percentage;
                _healthFill.color = GetColorForPercentage(percentage);
            }
        }

        private Color GetColorForPercentage(float percentage)
        {
            if (percentage > 0.6f)
            {
                return _fullHealthColor;
            }
            else if (percentage > 0.3f)
            {
                return _midHealthColor;
            }
            else
            {
                return _lowHealthColor;
            }
        }
    }
}
