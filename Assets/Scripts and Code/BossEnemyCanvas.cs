using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossEnemyCanvas : MonoBehaviour
{
    [SerializeField] string bossName;
    [SerializeField] Enemy enemyScript;

    [SerializeField] Text bossNameTEXT;
    [SerializeField] Text bossHealthTEXT;

    private void Start()
    {
        bossNameTEXT.text = bossName;
    }

    // Update is called once per frame
    void Update()
    {
        bossHealthTEXT.text = enemyScript.enemyStats.currentHealth + "/" + enemyScript.enemyStats.maxHealth;
    }
}
