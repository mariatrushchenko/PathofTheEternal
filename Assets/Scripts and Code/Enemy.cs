using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [System.Serializable]   // make class readable
    public class EnemyStats
    {
        public Animator animator;
        public int maxHealth = 100;

        private int _currentHealth;
        public int currentHealth
        {
            get { return _currentHealth; }
            set { _currentHealth = Mathf.Clamp(value, 0, maxHealth); }
        }

        // initialization (sort of like a constructor)
        public void Init()
        {
            currentHealth = maxHealth;
        }
    }

    // create instance of class so we can do something with it
    public EnemyStats enemyStats = new EnemyStats();

    [SerializeField] HealthBar enemyHealthBar;

    [Header("Quest: OPTIONAL")]
    public bool questEnemy;
    public string typeOfEnemyName;

    [Header("Player Coin Gain For Killing")]
    [SerializeField] int coinGainOnKill = 1;

    [Header("Parent GameObject: Ignore if there is a isDead animation")]
    [SerializeField] GameObject parentGameObject;

    private void OnDestroy()
    {
        if (parentGameObject != null)
            Destroy(parentGameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        enemyStats.Init();

        enemyHealthBar.SetMaxHealth(enemyStats.maxHealth);
        enemyHealthBar.SetCurrentHealth(enemyStats.maxHealth);
    }

   public void DamageEnemy(int damage)
    {
        enemyStats.currentHealth -= damage;
        enemyHealthBar.SetCurrentHealth(enemyStats.currentHealth);

        if (enemyStats.currentHealth <= 0)
        {
            GameMaster gm = GameMaster.gm;

            // this is so that we can change values for different enemies 
            if (coinGainOnKill > 0)
                gm.coinGain = coinGainOnKill;

            // check if there is a death animation. If not, spawn default particles
            bool hasIsDead = gm.FindParameter("isDead", enemyStats.animator);
            if (hasIsDead == true)
            {
                StartCoroutine(gm.GainCoin());
                enemyStats.animator.SetBool("isDead", true);
            }
            else           
                gm.KillEnemy(this);    
        }
    }
}
