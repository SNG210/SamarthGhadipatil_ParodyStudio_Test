using UnityEngine;
using TMPro;

public class OrbCollector : MonoBehaviour
{
    public int maxOrbs = 5;
    private int currentOrbs = 0;

    public TextMeshProUGUI orbCountText; 
    public LayerMask orbLayer; 

    void Start()
    {
        UpdateUI();
    }

    void OnCollisionEnter(Collision collision)
    {
        if ((orbLayer & (1 << collision.gameObject.layer)) != 0)
        {
            CollectOrb(collision.gameObject);
        }
    }

    void CollectOrb(GameObject orb)
    {
        currentOrbs++;
        Destroy(orb);
        UpdateUI();

        if (currentOrbs >= maxOrbs)
        {
            GameManager.Instance.GameWon(); 
        }
    }

    void UpdateUI()
    {
        if (orbCountText != null)
        {
            orbCountText.text = "Orbs: " + currentOrbs + " / " + maxOrbs;
        }
    }
}
