using UnityEngine;

public class CollectibleController : MonoBehaviour
{
    [SerializeField] private int numberOfLevels = 7;
    [Space]
    [SerializeField] private GameObject[] allStickers;
    [SerializeField] private GameObject[] stickers;
        [SerializeField] private GameObject nes;
        [SerializeField] private GameObject gameboy;
        [SerializeField] private GameObject n64;
        [SerializeField] private GameObject gba;
        [SerializeField] private GameObject gamecube;
        [SerializeField] private GameObject nds;
        [SerializeField] private GameObject wii;
        [SerializeField] private GameObject n3ds;
        [SerializeField] private GameObject nswitch;
    [SerializeField] private string[] collectiblePlayerPrefsName= new[]
    {
        "Collectible NES",
        "Collectible Gameboy",
        "Collectible Nintendo 64",
        "Collectible Gameboy Advance",
        "Collectible GameCube",
        "Collectible Nintendo DS",
        "Collectible Wii",
    };

    private void Awake()
    {
        allStickers = new[] { nes, gameboy, n64, gba, gamecube, nds, wii, n3ds, nswitch };
        stickers = InitializeArray(allStickers, numberOfLevels);
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < allStickers.Length; i++)
        {
            allStickers[i].SetActive(false);
        }
        
        CheckAndSpawnCollectibleStickers();
        
        if (HaveCollectedAllAvailableCollectibles(numberOfLevels)) RewardRemainingStickers();
    }

    void CheckAndSpawnCollectibleStickers()
    {
        for (int i = 0; i < stickers.Length; i++)
        {
            if (PlayerPrefs.GetInt(collectiblePlayerPrefsName[i], 0) == 1)
            {
                stickers[i].SetActive(true);
            }
        }
    }

    void RewardRemainingStickers()
    {
        // TODO: Reward remaining stickers if all available ones are collected.
    }

    bool HaveCollectedAllAvailableCollectibles(int numberOfLevels)
    {
        for (int i = 0; i < numberOfLevels; i++)
        {
            if (PlayerPrefs.GetInt(collectiblePlayerPrefsName[i], 0) == 0) return false;
        }

        return true;
    }

    public GameObject[] InitializeArray(GameObject[] source, int size)
    {
        GameObject[] result = new GameObject[size];
        for (int i = 0; i < size && i < source.Length; i++) result[i] = source[i];

        return result;
    }
}