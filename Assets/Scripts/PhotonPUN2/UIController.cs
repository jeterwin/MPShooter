using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    public GameObject leaderboard;
    public LeaderboardPlayer leaderboardPlayerDisplay;

    [SerializeField] private Slider healthImage;
    [SerializeField] private TextMeshProUGUI killsText;
    [SerializeField] private TextMeshProUGUI deathsText;


    private void Awake()
    {
        Instance = this;
    }
    public void UpdateHealth(int health)
    {
        healthImage.value = health;
    }
    public void UpdateStatsDisplay(PlayerInfo player)
    {
        if(MatchManager.Instance.AllPlayers.Count > MatchManager.Instance.Index)
        {
            killsText.text = "Kills: " + player.kills;
            deathsText.text = "Deaths: " + player.deaths;
        }
        else
        {
            killsText.text = "Kills: 0";
            deathsText.text = "Deaths: 0";
        }
    }
}
