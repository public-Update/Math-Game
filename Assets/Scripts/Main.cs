using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using TMPro;

public class Main : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] SpriteRenderer Board;
    public TMP_Text Text_Board;
    [SerializeField] GameObject BlockTouch;
    [SerializeField] Transform[] Tables;
    [HideInInspector] public Table[] Table;
    private PostProcessVolume _postProcessVolume;
    private Bloom _bloom;

    [Header("Animation")]
    [Range(0.01f, 5)] [SerializeField] float SpeedAnimation;
    [HideInInspector] public bool OpenAnimation;
    [HideInInspector] public bool CloseAnimation;
    float DelayToContinue;
    bool End;
    [HideInInspector] public bool PrepareClosingAnimation;
    Vector2 LengtBoard = new Vector2(50,12.41f);

    [Header("Question")]
    //У правильных ответов id должен совпадать c id уравнением
    [SerializeField] List<string> Equations = new List<string>();
    [SerializeField] List<float> Answers = new List<float>();
    [SerializeField] int NumberEquations; //сколько будет примеров за всю игру
    public string CorrectEquations;
    public float CorrectAnswer;
    
    private void Start() {
       Preview.Endless += Starting; 
       Table = FindObjectsOfType<Table>();
       _postProcessVolume = FindObjectOfType<PostProcessVolume>();
       _postProcessVolume.profile.TryGetSettings(out _bloom);
    }

    private void Starting() {
      if(NumberEquations > 0) {
        Generation();
        //узнаём длину уравнения и задаём её верхнему табло
        char[] array = CorrectEquations.ToCharArray(0, CorrectEquations.Length);
        if(array.Length <= 11) LengtBoard = new Vector2(array.Length * 3.4f, 12.41f);
        else if(array.Length > 11) LengtBoard = new Vector2(array.Length * 3f, 12.41f);
        ///////////////////////////////////////////////////
        OpenAnimation = true;    
        NumberEquations--;
      }
      else {
        End = true;
       }
    }

    private void OpeningAnimation() {
        //движение табличек
        Board.size = Vector2.Lerp(Board.size, LengtBoard, SpeedAnimation * Time.deltaTime );
        Tables[0].position = Vector3.Lerp(Tables[0].position, new Vector3(0.35f,0.35f,-7.91f), SpeedAnimation * Time.deltaTime);
        Tables[1].position = Vector3.Lerp(Tables[1].position, new Vector3(1.05f,0.35f,-7.91f), SpeedAnimation * Time.deltaTime);
        Tables[2].position = Vector3.Lerp(Tables[2].position, new Vector3(-0.35f,0.35f,-7.91f), SpeedAnimation * Time.deltaTime);
        Tables[3].position = Vector3.Lerp(Tables[3].position, new Vector3(-1.05f,0.35f,-7.91f), SpeedAnimation * Time.deltaTime);

        //Узнаём что анимация уже на конце
        Vector2 offset = LengtBoard - Board.size;
        float sqrLen = offset.sqrMagnitude;
        if(sqrLen < 0.7f) {
            OpenAnimation = false;
            BlockTouch.SetActive(false);
        }
        else if(sqrLen < 5) {
          foreach (Table objects in Table) objects.state = "ShowingText";
          Text_Board.color = Color.Lerp(Text_Board.color, new Color(0,0,0,1), 15 * Time.deltaTime);
        }
        /////////////////////////////////
    }

    private void ClosingAnimation() {
        Board.size = Vector2.Lerp(Board.size, LengtBoard, SpeedAnimation * Time.deltaTime );
        Tables[0].position = Vector3.Lerp(Tables[0].position, new Vector3(0,0.35f,-7.91f), SpeedAnimation * Time.deltaTime);
        Tables[1].position = Vector3.Lerp(Tables[1].position, new Vector3(0,0.35f,-7.91f), SpeedAnimation * Time.deltaTime);
        Tables[2].position = Vector3.Lerp(Tables[2].position, new Vector3(0,0.35f,-7.91f), SpeedAnimation * Time.deltaTime);
        Tables[3].position = Vector3.Lerp(Tables[3].position, new Vector3(0,0.35f,-7.91f), SpeedAnimation * Time.deltaTime);

        Vector2 offset = Board.size - LengtBoard;
        float sqrLen = offset.sqrMagnitude;
        if(sqrLen < 0.1f) {
            CloseAnimation = false;
            Starting();
        }
    }

    private void Generation() {
        int indexEquations;
        //получаем случайное уравнение с его ответом
       indexEquations = Random.Range(0,Equations.Count);
       Text_Board.text = Equations[indexEquations];
       CorrectEquations = Equations[indexEquations];
       CorrectAnswer = Answers[indexEquations];
       //назначаем случайную табличку верным ответом
       Table[Random.Range(0,Table.Length)].Number = CorrectAnswer;
       for(int i = 0; i < Table.Length; i++) {
        if(Table[i].Number != CorrectAnswer) {
          //нзаначаем остальные таблички неверным ответом
          if(i == 0) Table[i].Number = CorrectAnswer - 1;
          if(i == 1) Table[i].Number = CorrectAnswer + 1;
          if(i == 2) Table[i].Number = CorrectAnswer + 2;
          if(i == 3) Table[i].Number = CorrectAnswer + 3;
        }
        Table[i].FillText();
       }
       //Удаляем данный вариант уравнения, что бы не было повторов
       Equations.RemoveAt(indexEquations);
       Answers.RemoveAt(indexEquations);
    }

    private void Update() {
        if(OpenAnimation) OpeningAnimation();
        if(CloseAnimation) ClosingAnimation();
        if(PrepareClosingAnimation) PreparingClosingAnimation();
        //Увеличиваем интенсивность blooma, когда конц игры
        if(End) {
         if(_bloom.diffusion.value < 8) _bloom.diffusion.value += Time.deltaTime * 3;
         if(_bloom.intensity.value < 13) _bloom.intensity.value += Time.deltaTime * 3;
        }
    }

    public void Continue() {
      LengtBoard = new Vector2(12,12.41f);
      Text_Board.text = null;
      for(int i = 0; i < Table.Length; i++) Table[i].ResetToDefualt();
      CloseAnimation = true;
    }

    private void PreparingClosingAnimation()
    {
      Text_Board.color = Color.Lerp(Text_Board.color, new Color(0,0,0,0), 15 * Time.deltaTime);
      DelayToContinue += Time.deltaTime;
      if(DelayToContinue >= 0.3f) {
        Continue();
        Text_Board.color = new Color(0,0,0,0);
        DelayToContinue = 0;
        PrepareClosingAnimation = false;
      }
    }
}
