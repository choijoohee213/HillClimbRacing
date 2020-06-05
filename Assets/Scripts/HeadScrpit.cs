using UnityEngine;

public class HeadScrpit : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision) {
        //머리가 땅에 닿아서 게임오버
        if(collision.gameObject.CompareTag("Platform") && !GameManager.Instance.isDie) {
            GameManager.Instance.PlaySound("crack");
            GameManager.Instance.StartGameOver();
        }
    }
}