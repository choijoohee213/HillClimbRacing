using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager> {

    [SerializeField]
    private Image fuelGauge;

    [SerializeField]
    private GameObject fuelWarning, fadeIn, pauseUI, gameOverUI;

    [SerializeField]
    private Text moneyText, moneyEarnedText;

    private int totalMoney, moneyEarned = 0;

    public ObjectManager objectManager;
    public CameraController cameraController;
    private CarController carController;

    public bool GasBtnPressed { get; set; }
    public bool BrakeBtnPressed { get; set; }

    private void Start() {
        Time.timeScale = 1f;
        fadeIn.GetComponent<Animator>().SetTrigger("FadeIn");  //페이드 인 애니메이션 실행
        Initialize();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape))
            GamePause();
    }

    private void Initialize() {
        string objName = "";
        int stageIndex = PlayerPrefs.GetInt("Stage"), vehicleIndex = PlayerPrefs.GetInt("Vehicle");

        //선택한 맵 불러오기
        if(stageIndex.Equals(0))
            objName = "Country";
        else if(stageIndex.Equals(1))
            objName = "Mars";
        else if(stageIndex.Equals(2))
            objName = "Cave";
        objectManager.GetObject(objName);

        //선택한 차량 불러오기
        if(vehicleIndex.Equals(0))
            objName = "HillClimber";
        else if(vehicleIndex.Equals(1))
            objName = "Motorcycle";

        CarController vehicle = objectManager.GetObject(objName).GetComponent<CarController>();
        carController = vehicle;

        //카메라 조정
        cameraController.vehiclePos = vehicle.gameObject.transform;
        cameraController.SetUp();

        totalMoney = PlayerPrefs.GetInt("Money");
        moneyText.text = totalMoney.ToString();
    }

    public void FuelConsume() {
        fuelGauge.fillAmount = carController.Fuel;  //움직일수록 연료 게이지를 줄어들게한다.
        if(fuelGauge.fillAmount <= 0.6f) {  //연료 게이지 색깔 조정
            fuelGauge.color = new Color(1, fuelGauge.fillAmount * 0.8f * 2f, 0, 1);
            if(fuelGauge.fillAmount <= 0.3f) {  //연료 부족 경고 애니메이션
                fuelWarning.SetActive(true);
                if(fuelGauge.fillAmount <= 0.01f)
                    StartGameOver();
            }
        }
        else {
            fuelGauge.color = new Color((1f - fuelGauge.fillAmount) * 2f, 1, 0, 1);
            fuelWarning.SetActive(false);
        }
    }

    //연료를 획득하면 연료 게이지를 꽉 채운다.
    public void FuelCharge() {
        carController.Fuel = 1;
        fuelGauge.fillAmount = 1;
    }

    public void GetCoin(int price) {
        totalMoney += price;
        moneyEarned += price;
        moneyText.text = totalMoney.ToString();
    }

    public void GasBtn(bool press) {
        GasBtnPressed = press;
    }

    public void BrakeBtn(bool press) {
        BrakeBtnPressed = press;
    }

    public void GamePause() {
        pauseUI.SetActive(!pauseUI.activeSelf);
        
        if(pauseUI.activeSelf) Time.timeScale = 0f;
        else Time.timeScale = 1f;
    }

    public void GameComplete() {
    }

    public void StartGameOver() {
        StartCoroutine(GameOver());
    }

    private IEnumerator GameOver() {
        yield return new WaitForSeconds(5f);
        moneyEarnedText.text = "+" + moneyEarned.ToString() + "COINS";
        gameOverUI.SetActive(true);
        LoadScene(1);
    }

    public void LoadScene(int sceneIndex) {
        PlayerPrefs.SetInt("Money", totalMoney);
        SceneManager.LoadScene(sceneIndex);
    }
}