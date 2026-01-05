using UnityEngine;
using TMPro;
using System.Collections;

public class WalletUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI moneyText;      // Teks untuk saldo (Uang: Rp ...)
    public TextMeshProUGUI rewardPopup;    // Teks untuk hadiah (+Rp ...)

    void OnEnable()
    {
        // Berlangganan ke event perubahan uang dari script Wallet Anda
        Wallet.OnMoneyChanged += UpdateUI;
    }

    void OnDisable()
    {
        // Melepas langganan agar tidak terjadi error saat ganti scene
        Wallet.OnMoneyChanged -= UpdateUI;
    }

    void Start()
    {
        // Tampilkan saldo saat ini di awal permainan
        UpdateUI(Wallet.currentMoney);
        if (rewardPopup != null) rewardPopup.gameObject.SetActive(false);
    }

    public void UpdateUI(int totalMoney)
    {
        if (moneyText != null)
            moneyText.text = "Rp " + totalMoney.ToString();

        // Aktifkan popup jika ada penambahan uang (totalMoney bukan nol)
        if (rewardPopup != null && totalMoney > 0)
        {
            StopAllCoroutines();
            StartCoroutine(ShowRewardEffect());
        }
    }

    IEnumerator ShowRewardEffect()
    {
        rewardPopup.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.0f); // Tampilkan selama 2 detik
        rewardPopup.gameObject.SetActive(false);
    }
}