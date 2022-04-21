using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VirtualTwin
{
  
    public class State : MonoBehaviour
    {  
    public int state;
    public string StatePause = "p";
    public string StateGo = "o";
    public string ResetGo = "r";
        public static State Instance;

        public GameObject box;
        public GameObject box2;
        public GameObject box3;
        public GameObject box4;
        public GameObject box5;
        public GameObject box6;
        public GameObject box7;

        public InputField TextBox;
        public InputField TextBox2;
        public InputField TextBox3;
        public InputField TextBox4;
        public InputField TextBox5;
        public InputField TextBox6;
        public InputField TextBox7;
       
        public string KeyString;
        // Start is called before the first frame update
        void Awake()
        {
            Instance = this;
            box = GameObject.Find("Gear Ratio");
            box2 = GameObject.Find("Pulse Speed");
            box3 = GameObject.Find("Coast Speed");
            box4 = GameObject.Find("_Kt");
            box5 = GameObject.Find("_Ke");
            box6 = GameObject.Find("_R");
            box7 = GameObject.Find("_I");
            TextBox = box.GetComponent<InputField>() ;
            TextBox2 = box2.GetComponent<InputField>();
            TextBox3 = box3.GetComponent<InputField>();
            TextBox4 = box4.GetComponent<InputField>();
            TextBox5 = box5.GetComponent<InputField>();
            TextBox6 = box6.GetComponent<InputField>();
            TextBox7 = box7.GetComponent<InputField>();

        }
        void Start()
        {
            state = 0;
           
        }

        // Update is called once per frame
        void Update()
        {
         //state zero- start   
            if (Input.GetKey(StatePause))
            {
                state = 1;
               // Debug.Log(state);
            }
            if (Input.GetKey(StateGo))
            {
                state = 2;
              //  Debug.Log(state);
            }
            if (Input.GetKey(ResetGo))
            {
                state = 3;
                //Debug.Log(state);
                Application.LoadLevel(Application.loadedLevel);
            }
            switch(state)
            {
                case 2:
                    Time.timeScale = 1;
                    // box.SetActive(false);
                    TextBox.interactable = false;
                    TextBox2.interactable = false;
                    TextBox3.interactable = false;
                    TextBox4.interactable = false;
                    TextBox5.interactable = false;
                    TextBox6.interactable = false;
                    TextBox7.interactable = false;
                    // TextBox.enabled = false;
                    break;
                case 1:
                    Time.timeScale = 0;
                    // box.SetActive(false);
                    TextBox.interactable = false;
                    TextBox2.interactable = false;
                    TextBox3.interactable = false;
                    TextBox4.interactable = false;
                    TextBox5.interactable = false;
                    TextBox6.interactable = false;
                    TextBox7.interactable = false;
                    // TextBox.enabled = false;
                    break;
                case 0:
                    Time.timeScale = 0;
                    TextBox.interactable = true;
                    TextBox2.interactable = true ;
                    TextBox3.interactable = true;
                    // TextBox.enabled = true;
                    TextBox4.interactable = true;
                    TextBox5.interactable = true;
                    TextBox6.interactable = true;
                    TextBox7.interactable = true;

                    break;

            }
        }
        
    }
}