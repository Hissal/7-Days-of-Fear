using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEManager : MonoBehaviour
{
    [SerializeField] private GameObject qtePrefab;
    [SerializeField] private Transform canvas;
    KeyCode[] keyCodes = new KeyCode[] { KeyCode.Q, KeyCode.T, KeyCode.E };

    public QTE GenerateQTE(Vector2 positionOnScreen, float time)
    {
        GameObject newQTE = Instantiate(qtePrefab, canvas);
        newQTE.transform.localPosition = positionOnScreen;
        QTE qte = newQTE.GetComponent<QTE>();
        qte.Init(GetRandomKeyCode(keyCodes), time);
        return qte;
    }
    
    private KeyCode GetRandomKeyCode(KeyCode[] keyCodes)
    {
        // Return a random keyCode of either Q, T & E
        KeyCode randomKeyCode = keyCodes[Random.Range(0, keyCodes.Length)];
        return randomKeyCode;
    }

    public static QTEManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
