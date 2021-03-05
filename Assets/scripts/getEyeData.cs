using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {
            public class getEyeData : MonoBehaviour
            {
                public Text targetText;
                public Text positionText;
                public GameObject panel;
                public Text centerText;

                public Vector2 initLeftEye;
                public Vector2 initRightEye;
                public Vector2 leftEye;
                public Vector2 rightEye;

                //init time
                private float initTime;

                //視線方向関連
                private Ray ray;
                public int LengthOfRay = 25;
                //エラー回避のために0を入れておく
                private Vector3 gazeHitPosition = Vector3.zero;

                public float lefteye_pupil_diameter;
                public float righteye_pupil_diameter;

                private bool isStart = false;

                //csv
                public string filename = "eyeData";
                StreamWriter sw;
                // Start is called before the first frame update
                void Start()
                {
                    sw = new StreamWriter(@"" + filename + ".csv", false);
                    string[] s1 = { "positon_left_x", "position_left_y", "position_right_x", "position_right_y", "diameter_left", "diameter_right","gaze_position_x", "gaze_position_y", "gaze_position_z", "time" };
                    string s2 = string.Join(",", s1);
                    sw.WriteLine(s2);
                }

                // Update is called once per frame
                void Update()
                {

                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        Debug.Log("start");
                        initTime = Time.time;
                        isStart = true;
                        Destroy(panel);
                        Destroy(centerText);

                        //initialize
                        SRanipal_Eye_v2.GetPupilPosition(EyeIndex.LEFT, out initLeftEye);
                        SRanipal_Eye_v2.GetPupilPosition(EyeIndex.RIGHT, out initRightEye);
                    }

                    if(Input.GetKeyDown(KeyCode.Escape))
                    {
                        //recoding stop
                        Debug.Log("recoding finish");
                        isStart = false;
                        
                    }

                    if (isStart)
                    {

                        //視線
                        Vector3 GazeOriginCombinedLocal, GazeDirectionCombinedLocal;
                        //視線の視点および方向を取得
                        SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal);
                        //
                        Vector3 GazeDirectionCombined = Camera.main.transform.TransformDirection(GazeDirectionCombinedLocal);
                        RaycastHit hit;

                        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.position + GazeDirectionCombined * LengthOfRay, out hit))
                        {
                            //string objectName = hit.collider.gameObject.name;
                            gazeHitPosition = hit.point;
                        }
                        

                        //left pupil diametre
                        SRanipal_Eye_v2.GetPupilDiametre(EyeIndex.LEFT, out lefteye_pupil_diameter);
                        //right
                        SRanipal_Eye_v2.GetPupilDiametre(EyeIndex.RIGHT, out righteye_pupil_diameter);

                        //Debug.Log(lefteye_pupil_diameter);
                        targetText.text = "Pupil Diamiter\nLeft: " + lefteye_pupil_diameter.ToString() + "mm\nRight: " + righteye_pupil_diameter.ToString() + "mm";

                        //get position data
                        SRanipal_Eye_v2.GetPupilPosition(EyeIndex.LEFT, out leftEye);
                        SRanipal_Eye_v2.GetPupilPosition(EyeIndex.RIGHT, out rightEye);
                        leftEye -= initLeftEye;
                        rightEye -= initRightEye;

                        positionText.text = "Pupil Position\nLeft: (" + leftEye.x + ", " + leftEye.y + ")\nRight: (" + rightEye.x + "," + rightEye.y + ")";

                        float currentTime = Time.time;

                        //記録
                        string[] str = { "" + leftEye.x, "" + leftEye.y, "" + rightEye.x, "" + rightEye.y, "" + lefteye_pupil_diameter, "" + righteye_pupil_diameter, "" + gazeHitPosition.x, "" + gazeHitPosition.y, "" + gazeHitPosition.z, "" + (currentTime - initTime) };
                        string str2 = string.Join(",", str);
                        sw.WriteLine(str2);

                    }
                }
            }
        }
    }
}


