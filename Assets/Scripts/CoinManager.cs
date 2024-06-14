using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public int currentCoins = 0;
    public TMPro.TMP_Text coinText;

    void Start()
    {
        UpdateCoinText();
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        UpdateCoinText();
    }

    public bool SpendCoins(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            UpdateCoinText();
            return true;
        }
        return false;
    }

    void UpdateCoinText()
    {
        coinText.text = "Coins: " + currentCoins.ToString();
    }
}
