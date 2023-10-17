using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    [SerializeField] private Slider healthImage;

    private void Awake()
    {
        Instance = this;
    }
    public void UpdateHealth(int health)
    {
        healthImage.value = health;
    }
}
