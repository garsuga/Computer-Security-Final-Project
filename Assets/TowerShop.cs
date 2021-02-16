using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerShop : MonoBehaviour
{
    public GameObject TowerPrefab;
    List<GameObject> TowerEmployees;
    // Start is called before the first frame update
    void Start()
    {
        TowerEmployees = new List<GameObject>();
        Button employeeTower = GameObject.Find("EmployeeTower").GetComponent<Button>();
        employeeTower.onClick.AddListener(ApplyTower);
    }

    void ApplyTower() { TowerEmployees.Add(TowerPrefab); }
}
