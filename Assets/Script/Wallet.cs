using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;

public class Wallet : MonoBehaviour
{
    public static int currentMoney = 0;

    public static event System.Action<int> OnMoneyChanged;

    public static void AddMoney(int amount)
    {
        if (amount <= 0) return;

        currentMoney += amount;
        OnMoneyChanged?.Invoke(currentMoney);
        Debug.Log($"[WALLET] +{amount}, Total: {currentMoney}");
    }

    public static bool DeductMoney(int amount)
    {
        if (amount <= 0) return false;

        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            OnMoneyChanged?.Invoke(currentMoney);
            Debug.Log($"[WALLET] -{amount}, Total: {currentMoney}");
            return true;
        }

        Debug.Log("[WALLET] Gagal mengurangi uang (tidak cukup)");
        return false;
    }
}
