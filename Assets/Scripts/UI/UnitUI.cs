using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitUI : MonoBehaviour
{
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI unitName;

    [SerializeField] private Unit unit;
    [SerializeField] private float heightAboveUnit = 2;

    // Start is called before the first frame update
    void Start()
    {
        unit.HealthChanged += UpdateHpBar;
        unit.TakenDamage += UpdateHpBar;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(unit.transform.position.x, unit.transform.position.y + heightAboveUnit, unit.transform.position.z);
    }

    void UpdateHpBar(int currentHealth)
    {
        hpBar.fillAmount = (float) currentHealth / unit.MaxHealth;
    }

    public void SetName(string name)
    {
        unitName.text = name;
    }
}
