using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TallGate : MonoBehaviour
{
    [SerializeField] string calculation;
    [SerializeField] TextMeshProUGUI calculationTextMesh;
    [SerializeField] GameObject red;
    [SerializeField] GameObject blue;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals(GameConst.Data.playertag))
        {
            return;
        }

        gameObject.SetActive(false);
        var player = other.GetComponentInChildren<Player>();
        if (player == null) return;
        player.CalculateTall(calculation);
        var decrease = calculation.Contains("/") || calculation.Contains("-");
        SoundManager.Instance.PlaySFX(decrease ? "punch" : "bubble");
    }

    private void OnValidate()
    {
        if (!isActiveAndEnabled)
        {
            return;
        }

        var calculationText = calculation.Replace("*", "x").Replace("/", ":");
        calculationTextMesh.text = calculationText;
        var decrease = calculation.Contains("/") || calculation.Contains("-");
        red.gameObject.SetActive(decrease);
        blue.gameObject.SetActive(!decrease);
    }
}