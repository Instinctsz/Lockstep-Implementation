using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitUI : MonoBehaviour
{
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI unitName;

    private Unit unit;

    // Start is called before the first frame update
    void Start()
    {
        unit = GetComponentInParent<Unit>();
        unit.HealthChanged += UpdateHpBar;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateHpBar(int currentHealth)
    {
        hpBar.fillAmount = (float)currentHealth / unit.MaxHealth;
    }

    public void SetName(string name)
    {
        unitName.text = name;
    }
}
