using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Inventory UI")]
    public GameObject missionListContainer;
    public GameObject missionIconPrefab;
    public GameObject missionPanel;
    public GameObject durabilityPanel;

    [Header("Dialogue UI")]
    public GameObject panelSimple;
    public GameObject panelMission;

    public TMP_Text textSimpleName;
    public TMP_Text textSimpleContent;
    public TMP_Text textMissionName;
    public TMP_Text textMissionContent;

    [Header("Timer UI")]
    public TMP_Text timerText; // Drag UI Text ke slot ini

    public Image imageSimpleNPC;
    public Image imageMissionNPC;

    private PlayerMovement player;
    private MissionManager missionManager;

    private PickupPoint currentPickupRef;
    private bool isMissionPanelOpen = false;

    void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();
        missionManager = FindObjectOfType<MissionManager>();
    }

    void OnEnable()
    {
        PlayerMovement.OnInventoryChanged += UpdateMissionList;
    }

    void OnDisable()
    {
        PlayerMovement.OnInventoryChanged -= UpdateMissionList;
    }

    void Start()
    {
        missionPanel?.SetActive(false);
        panelSimple?.SetActive(false);
        panelMission?.SetActive(false);
        durabilityPanel?.SetActive(false);

        UpdateMissionList();
    }

    // ================= INVENTORY =================

    public void ToggleMissionPanel()
    {
        isMissionPanelOpen = !isMissionPanelOpen;
        missionPanel.SetActive(isMissionPanelOpen);

        if (isMissionPanelOpen)
            UpdateMissionList();
    }

    public void UpdateMissionList()
    {
        if (player == null || missionListContainer == null) return;

        foreach (Transform child in missionListContainer.transform)
            Destroy(child.gameObject);

        foreach (int id in player.activeDeliveryIDs)
        {
            GameObject icon = Instantiate(missionIconPrefab, missionListContainer.transform);

            TMP_Text txt = icon.GetComponentInChildren<TMP_Text>();
            if (txt != null) txt.text = $"Paket {id}";

            Button btn = icon.GetComponent<Button>();
            if (btn != null)
            {
                int cachedID = id;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnMissionIconClicked(cachedID));
            }
        }
    }

    public void OnMissionIconClicked(int missionID)
    {
        missionManager?.UpdateDestinationMarker(missionID);
        player?.SelectPackageToView(missionID);
        durabilityPanel?.SetActive(true);
    }

    // ================= DIALOG =================

    public void OpenSimpleDialogue(string npcName, string content, Sprite npcSprite)
    {
        panelSimple.SetActive(true);
        textSimpleName.text = npcName;
        textSimpleContent.text = content;
        imageSimpleNPC.sprite = npcSprite;

        PauseGame();
    }

    public void OpenMissionOffer(string npcName, string content, PickupPoint pickup, Sprite npcSprite)
    {
        currentPickupRef = pickup;

        panelMission.SetActive(true);
        textMissionName.text = npcName;
        textMissionContent.text = content;
        imageMissionNPC.sprite = npcSprite;

        PauseGame();
    }

    public void AcceptMissionFromUI()
    {
        currentPickupRef?.AcceptMission();
        CloseAllDialogues();
    }

    public void RejectMissionFromUI()
    {
        missionManager?.GenerateNewMission();
        CloseAllDialogues();
    }

    public void CloseAllDialogues()
    {
        panelSimple.SetActive(false);
        panelMission.SetActive(false);
        currentPickupRef = null;
        ResumeGame();
    }

    void Update()
    {
        int currentID = player.GetCurrentlySelectedPackageID();

    // Cek apakah ada paket yang dipilih DAN paket tersebut memiliki timer
    if (currentID != -1 && player.packageTimerRegistry.ContainsKey(currentID))
    {
        timerText.gameObject.SetActive(true); // Pastikan teks aktif
        float timeLeft = player.packageTimerRegistry[currentID];
        timerText.text = "Sisa Waktu: " + Mathf.CeilToInt(timeLeft).ToString() + "s";
        timerText.color = timeLeft < 10f ? Color.red : Color.white;
    }
    else
    {
        // Jika tidak ada paket aktif atau tidak ada paket yang dipilih, sembunyikan timer
        timerText.gameObject.SetActive(false);
    }
    }

    // ================= PAUSE =================

    void PauseGame()
    {
        player?.StopMoving();
        Time.timeScale = 0f;
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}
