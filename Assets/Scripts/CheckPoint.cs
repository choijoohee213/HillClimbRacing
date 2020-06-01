using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Vehicle")) {
            if(gameObject.name.Contains("Fuel")){  //연료 획득
                GameManager.Instance.FuelCharge();
                gameObject.SetActive(false);
            }
            else {  //도착지에 도달하여 게임 성공
                GameManager.Instance.GameComplete();
            }
            
        }
    }
}
