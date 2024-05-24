using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
struct Updgrade {
    public GameObject card;
    public Button button;
    public CinemachineVirtualCamera camera;
    public UnityEvent onSelect;
    public UnityEvent onDeselect;
}

public class WingsuitShop : MonoBehaviour
{
    [SerializeField] Updgrade[] upgrades;
    [SerializeField] Animator cardAnimator;
    Updgrade currentSelectedUpgrade;

    // Start is called before the first frame update
    void Start()
    {
        currentSelectedUpgrade = upgrades[0];
        foreach (var upgrade in upgrades) {
            upgrade.button.onClick.AddListener(() => {
                Debug.Log(upgrade.card.name);
                StartCoroutine(SwitchCards(upgrade));
            });
        }       
    }

    IEnumerator SwitchCards(Updgrade upgrade) {
        currentSelectedUpgrade.camera.gameObject.SetActive(false);
        upgrade.camera.gameObject.SetActive(true);    
        cardAnimator.SetTrigger("switch");
        currentSelectedUpgrade.onDeselect.Invoke();
        upgrade.onSelect.Invoke();

        yield return new WaitForSeconds(.5f);
        
        currentSelectedUpgrade.card.SetActive(false);
        upgrade.card.SetActive(true);
        currentSelectedUpgrade = upgrade;
    }

    public void StartGame() {
        StartCoroutine(StartGameCoroutine());
    }

    IEnumerator StartGameCoroutine() {
        yield return new WaitForSeconds(0.75f);
        currentSelectedUpgrade.camera.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
