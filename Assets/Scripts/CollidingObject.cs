using UnityEngine;

public class CollidingObject : MonoBehaviour {

    [SerializeField]
    private int price;

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Vehicle")) {
            if(gameObject.name.Contains("Fuel")) {  //연료 획득
                GameManager.Instance.FuelCharge();
                gameObject.SetActive(false);
            }
            else if(gameObject.name.Contains("Goal")) {  //도착지에 도달하여 게임 성공
                GameManager.Instance.GameComplete();
            }
            else if(gameObject.name.Contains("Coin")) {  //코인 획득
                GameManager.Instance.GetCoin(price);
                gameObject.SetActive(false);
            }
        }
    }
}