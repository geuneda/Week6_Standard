using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static ObjectPool;
using Random = UnityEngine.Random;

public enum UpgradeOption
{
    MaxHealth,
    AttackPower,
    Speed,
    Knockback,
    AttackDelay,
    NumberOfProjectiles,
    Count
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private GameObject pausePanel;
    
    public static GameManager Instance;
    [SerializeField] private string playerTag;
    
    [SerializeField] private CharacterStat defaultStat;
    [SerializeField] private CharacterStat rangedStat;

    public Transform Player {  get; private set; }
    public ObjectPool ObjectPool { get; private set; }
    public ParticleSystem EffectParticle;

    private HealthSystem playerHealthSystem;

    [SerializeField] private TextMeshProUGUI waveTxt;
    [SerializeField] private Slider hpGaugeSlider;
    [SerializeField] private GameObject gameOverUI;

    [SerializeField] private int currentWaveIndex = 0;
    private int currentSpawnCount = 0;
    private int waveSpawnCount = 0;
    private int waveSpawnPosCount = 0;
    
    public float spawnInterval = .5f;
    public List<GameObject> enemyPrefebs = new List<GameObject>();

    [SerializeField] private Transform spawnPositionsRoot;
    private List<Transform> spawnPositions = new List<Transform>();

    [SerializeField] private List<GameObject> rewards = new List<GameObject>();
    private void Awake()
    {
        if(Instance != null) Destroy(gameObject);
        Instance = this;

        Player = GameObject.FindGameObjectWithTag(playerTag).transform;
        ObjectPool = GetComponent<ObjectPool>();
        EffectParticle = GameObject.FindGameObjectWithTag("Particle").GetComponent<ParticleSystem>();
        
        playerHealthSystem = Player.GetComponent<HealthSystem>();
        playerHealthSystem.OnDamage += UpdateHealthUI;
        playerHealthSystem.OnHeal += UpdateHealthUI;
        playerHealthSystem.OnDeath += GameOver;

        UpgradeStatInit();

        for (int i = 0; i < spawnPositionsRoot.childCount; i++)
        {
            spawnPositions.Add(spawnPositionsRoot.GetChild(i));
        }
    }

    private void UpgradeStatInit()
    {
        defaultStat.statsChangeType = StatsChangeType.Add;
        defaultStat.attackSO = Instantiate(defaultStat.attackSO);
        
        rangedStat.statsChangeType = StatsChangeType.Add;
        rangedStat.attackSO = Instantiate(rangedStat.attackSO);
    }

    private void Start()
    {
        StartCoroutine(StartNextWave());
        
        if(pausePanel.activeInHierarchy)
        { pausePanel.SetActive(false); }
    }

    private IEnumerator StartNextWave()
    {
        while (true)
        {
            if (currentSpawnCount == 0)
            {
                UpdateWaveUI();
                
                yield return new WaitForSeconds(2f);

                ProcessWaveConditions();

                yield return StartCoroutine(SpawnEnemiesInWave());

                currentWaveIndex++;
            }

            yield return null;
        }
    }

    private IEnumerator SpawnEnemiesInWave()
    {
        for (int i = 0; i < waveSpawnCount; i++)
        {
            int posIdx = Random.Range(0, spawnPositions.Count);
            for (int j = 0; j < waveSpawnCount; j++)
            {
                SpawnEnemyAtPosition(posIdx);
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }

    private void SpawnEnemyAtPosition(int posIdx)
    {
        int prefabIdx = Random.Range(0, enemyPrefebs.Count);
        GameObject enemy = Instantiate(enemyPrefebs[prefabIdx], spawnPositions[posIdx].position, Quaternion.identity);
        enemy.GetComponent<CharacterStatHandler>().AddStatModifier(defaultStat);
        enemy.GetComponent<CharacterStatHandler>().AddStatModifier(rangedStat);
        enemy.GetComponent<HealthSystem>().OnDeath += OnEnemyDeath;
        currentSpawnCount++;
    }

    private void OnEnemyDeath()
    {
        currentSpawnCount--;
    }

    private void ProcessWaveConditions()
    {
        if (currentWaveIndex % 20 == 0)
        {
            RandomUpgrade();
        }

        if (currentWaveIndex % 10 == 0)
        {
            IncreaseSpawnPositions();
            // 10, 30, 50 에 부여되는 랜덤 디버프
            ApplyRandomDamage();
        }

        if (currentWaveIndex % 5 == 0)
        {
            CreateReward();
        }

        if (currentWaveIndex % 3 == 0)
        {
            IncreaseWaveSpawnCount();
        }
    }

    private void IncreaseWaveSpawnCount()
    {
        waveSpawnCount++;
    }

    private void CreateReward()
    {
        int selectedRewardIndex = Random.Range(0, rewards.Count);
        int randomPositionIndex = Random.Range(0, spawnPositions.Count);
        
        GameObject obj = rewards[selectedRewardIndex];
        Instantiate(obj, spawnPositions[randomPositionIndex].position, Quaternion.identity);
    }

    private void IncreaseSpawnPositions()
    {
        waveSpawnPosCount = waveSpawnCount + 1 > spawnPositions.Count ? waveSpawnPosCount : waveSpawnPosCount + 1;
        waveSpawnCount = 0;
    }

    private void RandomUpgrade()
    {
        UpgradeOption option = (UpgradeOption)Random.Range(0, (int)UpgradeOption.Count);
        switch (option)
        {
            case UpgradeOption.MaxHealth:
                defaultStat.maxHealth += 2;
                break;
            case UpgradeOption.AttackPower:
                defaultStat.attackSO.power += 1;
                break;
            case UpgradeOption.Speed:
                defaultStat.speed += 0.1f;
                break;
            case UpgradeOption.Knockback:
                defaultStat.attackSO.isOnKnockBack = true;
                defaultStat.attackSO.knockbackPower += 1;
                defaultStat.attackSO.knockbackTime = 0.1f;
                break;
            case UpgradeOption.AttackDelay:
                defaultStat.attackSO.delay -= 0.05f;
                break;
            case UpgradeOption.NumberOfProjectiles:
                RangedAttackSO rangedAttackData = rangedStat.attackSO as RangedAttackSO;
                if (rangedAttackData) rangedAttackData.numberOfProjectilesPerShot += 1;
                break;
            
            default:
                break;
        }
    }

    private void ApplyRandomDamage()
    {
        float randomDamagePercent = Random.Range(0f, 0.5f);
        int damageAmount = Mathf.RoundToInt(playerHealthSystem.MaxHealth * randomDamagePercent);
        
        playerHealthSystem.ChangeHealth(-damageAmount);
    }

    private void GameOver()
    {
        gameOverUI.SetActive(true);
    }

    private void UpdateHealthUI()
    {
        hpGaugeSlider.value = playerHealthSystem.CurrentHealth / playerHealthSystem.MaxHealth;
    }

    private void UpdateWaveUI()
    {
        waveTxt.text = (currentWaveIndex + 1).ToString();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
    }
}
