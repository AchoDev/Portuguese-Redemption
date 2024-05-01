using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
public class EnterableDoor : MonoBehaviour
{

    GameObject player;
    GameObject indicator;
    Fading blackScreen;
    bool openable = false;

    
    public Transform destination;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            openable = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            openable = false;
            indicator.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        blackScreen = GameObject.Find("blackscreen").GetComponent<Fading>();
        indicator = player.gameObject.transform.Find("indicator").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(!openable) return;

        indicator.SetActive(true);

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(Enter());
        }
    }

    IEnumerator Enter()
    {
        yield return blackScreen.FadeIn();
        yield return new WaitForSeconds(0.3f);
        Vector3 pos = destination.position;
        pos.z = player.transform.position.z;
        player.transform.position = pos;
        yield return blackScreen.FadeOut();
    }

    void Reset()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = true;
    }    
}
