using System;
using UnityEngine;

public class CollectibleController : MonoBehaviour
{
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
    [SerializeField] private string[] collectiblePlayerPrefsName;

    private void Awake()
    {
        nes = transform.Find("NES Ctrl").gameObject;
        gameboy = transform.Find("GameBoy").gameObject;
        n64 = transform.Find("Nin64").gameObject;
        gba = transform.Find("GBA").gameObject;
        gamecube = transform.Find("GameCube").gameObject;
        nds = transform.Find("Nintendo DS").gameObject;
        wii = transform.Find("Wii Remote").gameObject;
        n3ds = transform.Find("Nin 3Ds").gameObject;
        nswitch = transform.Find("Switch").gameObject;

        stickers = new[] { nes, gameboy, n64, gba, gamecube, nds, wii, n3ds, nswitch };
        collectiblePlayerPrefsName = new[]
        {
            "Collectible NES",
            "Collectible Gameboy",
            "Collectible Nintendo 64",
            "Collectible Gameboy Advance",
            "Collectible GameCube",
            "Collectible Nintendo DS",
            "Collectible Wii",
        };
    }

    private void Start()
    {
        for (int i = 0; i < stickers.Length; i++)
        {
            stickers[i].SetActive(false);
        }
        
        CheckAndSpawnCollectibleStickers();
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
}