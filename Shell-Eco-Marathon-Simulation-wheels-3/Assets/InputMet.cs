using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace VirtualTwin
{
    public class InputMet : MonoBehaviour
    {
        public InputField GearField ;

        public static InputMet Specific;
        public string AnswerGF;
        public string AnswerPS;
        public string AnswerCS;
        public string AnswerKv;
        public string AnswerKt;
        public string AnswerKe;
        public string AnswerR;
        public string AnswerI;
        public string FileNameAns;
        public float NumericalValue;
        public float NumericalValueP;
        public float NumericalValueC;
        public InputField PulseSpeed;
        public InputField CoastSpeed;
        public InputField FileNameInput;
        public InputField _Kt;
        public InputField _Ke;
        public InputField _R;
        public InputField _I;
       // public GameObject _FN;

        public float Kv;
        public float Ke;
        public float Kt;
        public float I;
        public float R;
        public string FN;
       
        // Start is called before the first frame update
        void Awake()
        {
            Specific = this;
            GearField = GameObject.Find("Gear Ratio").GetComponent<InputField>();
            PulseSpeed = GameObject.Find("Pulse Speed").GetComponent<InputField>();
            CoastSpeed = GameObject.Find("Coast Speed").GetComponent<InputField>();
            FileNameInput = GameObject.Find("FILE_NAME").GetComponent<InputField>();
            _Kt = GameObject.Find("_Kt").GetComponent<InputField>();
            _Ke = GameObject.Find("_Ke").GetComponent<InputField>();
            _R = GameObject.Find("_R").GetComponent<InputField>();
            _I = GameObject.Find("_I").GetComponent<InputField>();
        }
        void Start()
        {


        }
        // Update is called once per frame
        void Update()
        {

           
        }
        public void Push()
        {
            AnswerGF = GearField.text;
            if (AnswerGF == "")
            {

            }
            else
            {
                NumericalValue = float.Parse(AnswerGF);
            }
            AnswerPS = PulseSpeed.text;
            if (AnswerPS == "")
            {

            }
            else
            {
                NumericalValueP = float.Parse(AnswerPS);
            }
           
            

            AnswerCS = CoastSpeed.text;
            if (AnswerCS == "")
            {

            }
            else
            {
                NumericalValueC = float.Parse(AnswerCS);
            }

           

            AnswerKt = _Kt.text;
            if (AnswerKt == "")
            {

            }
            else
            {
                Kt = float.Parse(AnswerKt);
            }

           

            //AnswerKv = PulseSpeed.GetComponent<Text>().text;
            //Kv = float.Parse(AnswerKv);

            AnswerKe = _Ke.text;
            if (AnswerKe == "")
            {

            }
            else
            { 
                Ke = float.Parse(AnswerKe);
                
            }

           

            AnswerR = _R.text;
            if (AnswerR == "")
            {

            }
            else
            {
                R = float.Parse(AnswerR);
            }

          

            AnswerI = _I.text;
            if (AnswerI == "")
            {

            }
            else
            {
                I = float.Parse(AnswerI);
            }

            
            
            FileNameAns = FileNameInput.text;
            if (FileNameAns == "")
            {

            }
            else
            {
                FN = FileNameAns;
            }

          
        }
    }
}