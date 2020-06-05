using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    [SerializeField]
    private GameObject scrollView, scrollbar, purchaseUI, fadeOut;

    [SerializeField]
    private GameObject[] Contents, Stages, Vehicles;

    private GameObject[] content;

    [SerializeField]
    private Text moneyText, cantBuyText;

    [SerializeField]
    private AudioSource audio;

    private float scroll_pos = 0, distance;
    private float[] pos;

    private int selectedMenuIndex, selectedIndex;
    private bool changeIndex = true, start = true;

    private void Start() {
        //저장된 데이터가 없을 경우 초기화
        if(!PlayerPrefs.HasKey("Stage")) {
            PlayerPrefs.SetInt("Stage", 0);
            PlayerPrefs.SetInt("Vehicle", 0);
            PlayerPrefs.SetInt("Stage_Mars", 0);
            PlayerPrefs.SetInt("Stage_Cave", 0);
            PlayerPrefs.SetInt("Vehicle_Motorcycle", 0);
            PlayerPrefs.SetInt("Money", 5000);
        }
        LoadData();
        MenuChange(1);  //초기에는 스크롤 뷰를 차량 콘텐츠로
        start = false;
    }

    //선택되었던 스테이지/차량 데이터와 돈 데이터 불러오기
    private void LoadData() {
        Stages[1].transform.GetChild(1).gameObject.SetActive(PlayerPrefs.GetInt("Stage_Mars").Equals(0));
        Stages[1].GetComponent<Button>().enabled = PlayerPrefs.GetInt("Stage_Mars").Equals(0);
        Stages[2].transform.GetChild(1).gameObject.SetActive(true);
        Stages[3].transform.GetChild(1).gameObject.SetActive(true);
        Vehicles[1].transform.GetChild(1).gameObject.SetActive(PlayerPrefs.GetInt("Vehicle_Motorcycle").Equals(0));
        Vehicles[1].GetComponent<Button>().enabled = PlayerPrefs.GetInt("Vehicle_Motorcycle").Equals(0);
        Vehicles[2].transform.GetChild(1).gameObject.SetActive(true);
        Vehicles[3].transform.GetChild(1).gameObject.SetActive(true);

        moneyText.text = PlayerPrefs.GetInt("Money").ToString();
    }

    private void Update() {
        //스크롤 뷰 사용하기
        if(Input.GetMouseButton(0)) {
            scroll_pos = scrollbar.GetComponent<Scrollbar>().value;
        }
        else {
            for(int i = 0; i < pos.Length; i++) {
                if(scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2)) {
                    scrollbar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollbar.GetComponent<Scrollbar>().value, pos[i], 0.1f);
                    selectedIndex = i;
                }
            }
            changeIndex = true;
        }

        //선택된 콘텐츠는 사이즈를 크게, 나머지 콘텐츠는 사이즈를 작게
        for(int i = 0; i < pos.Length; i++) {
            if(scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2)) {
                content[i].transform.localScale = Vector2.Lerp(content[i].transform.localScale, new Vector2(1.2f, 1.2f), 0.1f);
                for(int j = 0; j < pos.Length; j++)
                    if(j != i)
                        content[j].transform.localScale = Vector2.Lerp(content[j].transform.localScale, new Vector2(0.8f, 0.8f), 0.1f);

                if(changeIndex) {  //선택된 콘텐츠를 데이터에 저장
                    SaveSelectedData(i);
                    changeIndex = false;
                }
            }
        }
    }

    //스테이지/차량 버튼을 누르면 스크롤 뷰의 콘텐츠의 종류가 바뀌도록 한다.
    public void MenuChange(int index) {
        //잠금해제되지않은 콘텐츠가 선택된 채로 콘텐츠 종류를 바꾸려고 하면, 구매 여부를 묻도록 한다.
        if(!CheckPurchased() && !start) {
            if(!(selectedMenuIndex == 0 && selectedIndex > 1 || selectedMenuIndex == 1 && selectedIndex > 1)) {
                purchaseUI.SetActive(true);
                return;
            }
        }    
        selectedMenuIndex = index;  //선택한 콘텐츠 종류를 변수에 저장

        pos = new float[Contents[index].transform.childCount];
        distance = 1f / (pos.Length - 1f);
        for(int i = 0; i < pos.Length; i++)
            pos[i] = distance * i;

        if(index.Equals(0)) { //콘텐츠 종류를 스테이지로 변경
            content = Stages;
            scroll_pos = scrollbar.GetComponent<Scrollbar>().value = pos[PlayerPrefs.GetInt("Stage")];
        }
        else if(index.Equals(1)) {  //콘텐츠 종류를 차량으로 변경
            content = Vehicles;
            scroll_pos = scrollbar.GetComponent<Scrollbar>().value = pos[PlayerPrefs.GetInt("Vehicle")];
        }

        foreach(var obj in Contents)
            obj.SetActive(false);
        Contents[index].SetActive(true);
        scrollView.GetComponent<ScrollRect>().content = Contents[index].GetComponent<RectTransform>();
    }

    //잠금해제 되지 않은 것을 구매하고 데이터 변경.
    public void Purchase() {
        int price, moneyOwned = PlayerPrefs.GetInt("Money");
        if(selectedMenuIndex.Equals(0)) {  //스테이지
            price = int.Parse(Stages[selectedIndex].transform.GetChild(1).gameObject.transform.GetChild(1).GetComponent<Text>().text);
            if(moneyOwned - price < 0) { cantBuyText.GetComponent<Animator>().SetTrigger("warning"); return; }
            if(selectedIndex.Equals(1)) PlayerPrefs.SetInt("Stage_Mars", 1);
            
        }
        else {  //차량
            price = int.Parse(Vehicles[selectedIndex].transform.GetChild(1).gameObject.transform.GetChild(1).GetComponent<Text>().text);
            if(moneyOwned - price < 0) { cantBuyText.GetComponent<Animator>().SetTrigger("warning"); return; }
            PlayerPrefs.SetInt("Vehicle_Motorcycle", 1);
        }
        PlayerPrefs.SetInt("Money", moneyOwned - price);
        audio.Play();
        LoadData();
    }

    //선택된 콘텐츠가 잠금해제(구매) 되었는지 여부 확인
    private bool CheckPurchased() {
        if(selectedMenuIndex.Equals(0)) {
            if(selectedIndex != 0)
                return !Stages[selectedIndex].transform.GetChild(1).gameObject.activeSelf;
        }
        else { 
            if(selectedIndex != 0) 
                return !Vehicles[selectedIndex].transform.GetChild(1).gameObject.activeSelf;
        }
        return true;
    }

    //선택된 콘텐츠 인덱스를 데이터에 저장
    private void SaveSelectedData(int index) {
        if(selectedMenuIndex.Equals(0)) {
            if(!CheckPurchased()) return; 
            PlayerPrefs.SetInt("Stage", index);
        }
        else {
            if(!CheckPurchased()) return; 
            PlayerPrefs.SetInt("Vehicle", index);
        }
    }

    //게임 시작 버튼을 누르면 게임을 시작
    public void GameStart() {
        if(!CheckPurchased()) {
            if(!(selectedMenuIndex == 0 && selectedIndex > 1 || selectedMenuIndex == 1 && selectedIndex > 1)) {
                purchaseUI.SetActive(true);
                return;
            }
        }
        fadeOut.GetComponent<Animator>().SetTrigger("FadeOut");
    }
}