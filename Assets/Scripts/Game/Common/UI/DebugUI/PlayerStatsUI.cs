using System.Collections;
using System.Collections.Generic;
using Game.Common.Settings;
using TMPro;
using UnityEngine;

public class PlayerStatsUI : MonoBehaviour
{

    public MasterSettings masterSettings;
    public Transform VerticalListContainer;
    public GameObject TextPrefab;

    public List<GameObject> currentUIText = new List<GameObject>();
    
    // Update is called once per frame
    void Update()
    {
        if (masterSettings == null)
        {
            return;
        }
        
        
        UpdateUI();
    }


    void UpdateUI()
    {
        //Constant destruction and recreation of list UI
        // Could be optimized but does not require it
        // Ease of not hooking events
        int curPlayerCount = 0;
        foreach (var playerID in masterSettings.GetPlayerIDs())
        {

            uint ID = playerID.ID;
            int loadout = -1;
            
            var playerGameData = masterSettings.playerGameDataRegistry.Get(playerID);
            if (playerGameData != null)
            {
                loadout = playerGameData.shipCreationData.shipType;
            }


            string statsText = $"ID: {ID} - L: {loadout}";
            if (curPlayerCount > currentUIText.Count - 1)
            {
                InstantiateStatsText(statsText);
            }
            else
            {
                SetStatsText(curPlayerCount, statsText);
            }
            
            curPlayerCount++;
            
        }

        //Cleans up excess player text ui
        for (int i = currentUIText.Count - 1;  i > curPlayerCount; i--)
        {
            Destroy(currentUIText[i]);
            currentUIText.RemoveAt(i);
        }
        
        
    }

    public void InstantiateStatsText(string text)
    {
        var go = Instantiate(TextPrefab, VerticalListContainer);
        go.GetComponent<TextMeshProUGUI>().text = text;
        
        currentUIText.Add(go);
    }

    public void SetStatsText(int id, string text)
    {
        currentUIText[id].GetComponent<TextMeshProUGUI>().text = text;
    }
}
