using UnityEngine;
using TMPro;

public class Table : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] GameObject BlockTouch;
    private Main Main;
    private Transform Tr_Text;
    private TMP_Text Text_Number;
    private Material TableMaterial;
    
    [Header("Animation")]
    public float Number;
    [HideInInspector] public string state; 
    private float DelayRestart;

    [Header("CharacterPosition")]
    [SerializeField] Vector3 PositionOfNumber;
    [SerializeField] Vector3 OldPosition;
    Vector3 centerArc;
    private int idX;
    
    private void Start() {
       Text_Number = transform.GetChild(0).GetComponent<TMP_Text>(); 
       TableMaterial = GetComponent<MeshRenderer>().material;
       Main = FindObjectOfType<Main>();
       Tr_Text = Text_Number.transform;
       OldPosition = Tr_Text.position;
    }

    public void FillText() {
        Text_Number.text = Number.ToString();
        Text_Number.fontSize = 2f;
        Tr_Text.position = OldPosition;
    }

    public void CheckOut() {
       if(!Main.CloseAnimation && !Main.OpenAnimation) {
        if(Number == Main.CorrectAnswer) {
            state = "Right";
            BlockTouch.SetActive(true);
            Main.Text_Board.text = Main.CorrectEquations.Replace("X", "<color=white>X</color>");
            Text_Number.fontSize = 2.5f;
            GettingPosCharacter();
        }
        else state = "Wrong";
       }
    }

    private void Update() {
            if(state == "Right") {
                TableMaterial.color = Color.Lerp(TableMaterial.color, Color.green, 3 * Time.deltaTime);
                //получаем дугу, по которой будем перемещать верный ответ
                 centerArc = (Tr_Text.position + PositionOfNumber) * 0.5f;
                 centerArc -= new Vector3(0.8f,0.8f,0);
                 Vector3 riseRelCenter = Tr_Text.position - centerArc;
                 Vector3 setRelCenter = PositionOfNumber - centerArc;
                 Tr_Text.position = Vector3.Slerp(riseRelCenter, setRelCenter, 4 * Time.deltaTime);
                 Tr_Text.position += centerArc;
                //////////////////////////////////////////////////
                DelayRestart += Time.deltaTime;
                if(DelayRestart >= 2f) {
                   Main.PrepareClosingAnimation = true;
                  DelayRestart = 0;
                }
            }
            else if(state == "Wrong") {
               TableMaterial.color = Color.Lerp(TableMaterial.color, Color.red, 3 * Time.deltaTime);
               Text_Number.color = Color.Lerp(Text_Number.color, new Color(0,0,0,0), 15 * Time.deltaTime);
            }
            if(state == "ShowingText") {
              Text_Number.color = Color.Lerp(Text_Number.color, new Color(0,0,0,1), 15 * Time.deltaTime);
            }

        if(Main.PrepareClosingAnimation) {
            TableMaterial.color = Color.Lerp(TableMaterial.color, Color.white, 15 * Time.deltaTime);
            if(state != "Wrong") Text_Number.color = Main.Text_Board.color;
            else if (state == "Wrong") Text_Number.color = new Color(0,0,0,0);
        }
    }

    public void ResetToDefualt()
    {
        TableMaterial.color = Color.white;
        Text_Number.color = new Color(0,0,0,0);
        Number = 0;
        state = null;
        Text_Number.text = null;
        BlockTouch.SetActive(false);
    }

    private void GettingPosCharacter()
    {
        //узнаём позицию X в тексте
        TMP_TextInfo textInfo = Main.Text_Board.textInfo;
        char[] array = Main.CorrectEquations.ToCharArray(0, Main.CorrectEquations.Length);
        for(int i = 0; i < array.Length; i++) if(array[i] == 'X') idX = i;
        TMP_CharacterInfo charInfo = textInfo.characterInfo[idX];
        PositionOfNumber = charInfo.bottomLeft;
        Vector3 worldBottomLeft = Main.Text_Board.transform.TransformPoint(PositionOfNumber);
           //корректировка вектора, поскольку TransformPoint работает не идеально
        PositionOfNumber = worldBottomLeft;
        PositionOfNumber.x += 0.064f;
        PositionOfNumber.y += 0.084f;
        PositionOfNumber.z = Tr_Text.parent.position.z - 0.01f;
    }
}
