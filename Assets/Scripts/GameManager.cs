using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager> {
    [SerializeField]
    Image fuelGauge;

    [SerializeField]
    GameObject fuelWarning, fadeIn;

    public ObjectManager objectManager;
    public CameraController cameraController;
    CarController carController;


    public bool GasBtnPressed { get; set; }
    public bool BrakeBtnPressed { get; set; }


    void Start() {
        fadeIn.GetComponent<Animator>().SetTrigger("FadeIn");  //페이드 인 애니메이션 실행
        Initialize();
    }

    void Initialize() {
        string objName = "";
        int stageIndex = PlayerPrefs.GetInt("Stage"), vehicleIndex = PlayerPrefs.GetInt("Vehicle");
        
        //선택한 맵 불러오기
        if(stageIndex.Equals(0)) objName = "Country";
        else if(stageIndex.Equals(1)) objName = "Mars";
        else if(stageIndex.Equals(2)) objName = "Cave";
        objectManager.GetObject(objName);

        
        //선택한 차량 불러오기
        if(vehicleIndex.Equals(0)) objName = "HillClimber";
        else if(vehicleIndex.Equals(1)) objName = "Motorcycle";
        
        CarController vehicle = objectManager.GetObject(objName).GetComponent<CarController>();
        carController = vehicle;

        //카메라 조정
        cameraController.vehiclePos = vehicle.gameObject.transform;
        cameraController.SetUp();
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

    public void GasBtn(bool press) {
        GasBtnPressed = press;
    }
    public void BrakeBtn(bool press) {
        BrakeBtnPressed = press;
    }

    public void GameComplete() {

    }

    public void StartGameOver() {
        StartCoroutine(GameOver());
    }
   
    IEnumerator GameOver() {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(1);
    }
}
