using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadScrpit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {
        //머리가 땅에 닿아서 게임오버
        if(collision.gameObject.CompareTag("Platform")) {
            GameManager.Instance.StartGameOver();
        }
    }
}

