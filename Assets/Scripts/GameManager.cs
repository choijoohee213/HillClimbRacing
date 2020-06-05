using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager> {

    [SerializeField]
    private Image fuelGauge, captureImg;
    private Texture2D textureImg;
    private Sprite spriteImg;

    [SerializeField]
    private GameObject fuelWarning, fadeIn, pauseUI, gameOverUI;

    [SerializeField]
    private Text moneyText, moneyEarnedText, distanceText, totaldistanceText, gameStateText;

    [SerializeField]
    private AudioSource[] audio;

    private int totalMoney, moneyEarned = 0;

    public ObjectManager objectManager;
    public CameraController cameraController;
    private CarController carController;

    public bool GasBtnPressed { get; set; }
    public bool BrakeBtnPressed { get; set; }
    public bool isDie { get; set; }
    public bool ReachGoal { get; set; }

    private void Start() {
        Time.timeScale = 1f;
        isDie = false;
        ReachGoal = false;
        fadeIn.GetComponent<Animator>().SetTrigger("FadeIn");  //페이드 인 애니메이션 실행
        Initialize();
    }

    private void Update() {
        //뒤로가기 누르면 게임 일시정지
        if(Input.GetKeyDown(KeyCode.Escape))  
            GamePause();

        //움직인 거리 계산하여 계속해서 text 갱신
        if(!gameOverUI.activeSelf)
            distanceText.text = (int)(carController.transform.position.x - carController.StartPos.x) + "m / <color=yellow>1427m</color>";

        //게임오버/성공 후 한번 더 터치하면 게임 재시작
        if(isDie && Input.GetMouseButtonDown(0) && gameOverUI.activeSelf) 
            LoadScene(0);

        //엔진/브레이크 버튼 누를 시에 사운드 재생
        if(GasBtnPressed || BrakeBtnPressed)
            PlaySound("engine");
    }

    //게임 초기 세팅 함수
    private void Initialize() {
        string objName = "";
        int stageIndex = PlayerPrefs.GetInt("Stage"), vehicleIndex = PlayerPrefs.GetInt("Vehicle");

        //선택한 맵 불러오기
        if(stageIndex.Equals(0)) {
            objName = "Country";
            Camera.main.backgroundColor = new Color(0.5803922f, 0.8470589f, 0.937255f, 0);
        }
        else if(stageIndex.Equals(1)) {
            objName = "Mars";
            Camera.main.backgroundColor = new Color(0.8627452f, 0.6666667f, 0.6666667f, 0);
        }
        else if(stageIndex.Equals(2))
            objName = "Cave";
        objectManager.GetObject(objName);

        //선택한 차량 불러오기/오브젝트 생성
        if(vehicleIndex.Equals(0)) objName = "HillClimber";
        else if(vehicleIndex.Equals(1)) objName = "Motorcycle";
        CarController vehicle = objectManager.GetObject(objName).GetComponent<CarController>();
        carController = vehicle;

        //카메라 조정
        cameraController.vehiclePos = vehicle.gameObject.transform;
        cameraController.SetUp();

        //소유한 돈의 데이터를 불러와 text 갱신
        totalMoney = PlayerPrefs.GetInt("Money");
        moneyText.text = totalMoney.ToString();
    }

    //연료 소비 함수
    public void FuelConsume() {
        fuelGauge.fillAmount = carController.Fuel;  //움직일수록 연료 게이지를 줄어들게한다.
        if(fuelGauge.fillAmount <= 0.6f) {  //연료 게이지 색깔 조정
            fuelGauge.color = new Color(1, fuelGauge.fillAmount * 0.8f * 2f, 0, 1);  //게이지가 줄어들수록 그라데이션 효과
            
            if(fuelGauge.fillAmount <= 0.3f) {  //연료 부족 경고 애니메이션
                fuelWarning.SetActive(true);
                if(fuelGauge.fillAmount == 0f)  //연료가 다 떨어져서 게임 오버
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
        fuelGauge.fillAmount = 1;  //게이지 바 꽉 채운다
        PlaySound("refuel"); //연료충전 사운드 재생
    }

    //코인 얻었을 때 함수
    public void GetCoin(int price) {
        totalMoney += price;
        moneyEarned += price;
        moneyText.text = totalMoney.ToString(); //text에 금액 갱신
        moneyText.GetComponent<Animator>().SetTrigger("EarnMoney");  //애니메이션 
        PlaySound("coin"); //코인 사운드 재생
    }

    //엔진 버튼 함수
    public void GasBtn(bool press) {
        GasBtnPressed = press;
    }

    //브레이크 버튼 함수
    public void BrakeBtn(bool press) {
        BrakeBtnPressed = press;
    }

    //사운드 재생 함순
    public void PlaySound(string audioName) {
        switch(audioName) {
            case "cameraShutter" :
                audio[0].Play();
                break;
            case "coin":
                audio[1].Play();
                break;
            case "crack":
                audio[2].Play();
                break;
            case "refuel":
                audio[3].Play();
                break;
            case "engine":
                audio[4].Play();
                break;
        }
    }

    //게임 일시정지 함수
    public void GamePause() {
        pauseUI.SetActive(!pauseUI.activeSelf); //일시정지 UI 활성화/비활성화
        
        if(pauseUI.activeSelf) Time.timeScale = 0f;
        else Time.timeScale = 1f;
    }

    //게임오버 함수
    public void StartGameOver() {
        if(!isDie) {
            StartCoroutine(GameOver());
            isDie = true;
        }
    }

    private IEnumerator GameOver() {
        if(!ReachGoal) yield return new WaitForSeconds(4f);

        carController.moveStop = true;
        fuelWarning.SetActive(false);

        //게임 오버 시 차량의 모습을 스크린샷하여 UI 이미지로 보여줌
        yield return new WaitForEndOfFrame();
        Texture2D text = new Texture2D(Screen.width / 5, Screen.height / 3, TextureFormat.RGB24, false);
        textureImg = new Texture2D(Screen.width / 5, Screen.height / 3);
        text.ReadPixels(new Rect(-Screen.width / 2, Screen.height / 3 + 15f, Screen.width, Screen.height), 0, 0);
        text.Apply();
        textureImg = text;
        spriteImg = Sprite.Create(textureImg, new Rect(0, 0, textureImg.width, textureImg.height), new Vector2(0, 0));
        captureImg.sprite = spriteImg;

        //게임오버 UI의 텍스트 값들을 바꾸고 활성화
        if(!ReachGoal) gameStateText.text = "<color=#FF4C4C>Game Over</color>";
        else gameStateText.text = "<color=#FFFF4C>Game Complete!!</color>";
        moneyEarnedText.text = "+" + moneyEarned.ToString() + " COINS";  //게임 동안 얻은 코인 수를 보여줌
        totaldistanceText.text = " Distance : " + (int)(carController.transform.position.x - carController.StartPos.x) + "m";
        gameOverUI.SetActive(true);
        
        PlaySound("cameraShutter"); //카메라 셔터 사운드 재생
    }

    public void LoadScene(int sceneIndex) {
        PlayerPrefs.SetInt("Money", totalMoney);  //얻은 코인 데이터 저장
        SceneManager.LoadScene(sceneIndex); 
    }
}