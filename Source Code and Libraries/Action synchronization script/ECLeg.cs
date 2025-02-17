using UnityEngine;
using System.Collections;
using Extend;

namespace Actor
{
    /// <summary>
    /// 
    /// </summary>
	public class ECLeg : MonoBehaviour 
	{
        /// <summary>
        /// 角1旋转角度
        /// </summary>
        private float theta_1;
        /// <summary>
        /// 角2旋转角度
        /// </summary>
        private float theta_2;
        /// <summary>
        /// 臂1长
        /// </summary>
        private float l_1 = 7.5f;
        /// <summary>
        /// 臂2长
        /// </summary>
        private float l_2 = 8f;
        /// <summary>
        /// 轴1
        /// </summary>
        private ArticulationBody shaft1;
        /// <summary>
        /// 轴2
        /// </summary>
        private ArticulationBody shaft2;

        private Vector3 oldtheta1;
        private Vector3 oldtheta2;
        /// <summary>
        /// 目标坐标
        /// </summary>
        public Vector2 aimPos;

        private void Awake()
        {
            shaft1 = transform.GetComponent<ArticulationBody>();
            shaft2 = transform.GetChild(0).GetComponent<ArticulationBody>();
        }

        private void Start ()
        {
            CalculatingAngle(aimPos.x, aimPos.y);
        }

        public void CalculatingAngle(float aimX, float aimY)
        {
            float L = Mathf.Sqrt(Mathf.Pow(aimX, 2) + Mathf.Pow(aimY, 2));
            float AE = (Mathf.Pow(l_1, 2) - Mathf.Pow(l_2, 2) + Mathf.Pow(L, 2)) / (2 * L);
            float x0 = AE / L * aimX;
            float y0 = AE / L * aimY;
            float CE = Mathf.Sqrt(Mathf.Pow(l_1, 2) - Mathf.Pow(x0, 2) - Mathf.Pow(y0, 2));
            float K = -1 * aimX / aimY;
            float EF = CE / Mathf.Sqrt(1 + Mathf.Pow(K, 2));
            float x1 = x0 - EF;
            float y1 = y0 + K * (x1 - x0);
            float x2 = x0 + EF;
            float y2 = y0 + K * (x2 - x0);
            //Debug.Log($"There is two solution: ({x1}, {y1}), ({x2}, {y2})");

            float distance = Vector2.Distance(new Vector2(0, 0), new Vector2(aimX, aimY));
            if (distance > l_1 + l_2)
            {
                return;
            }
            else
            {
                if (aimX > x1 && x2 >= x1)
                {
                    theta_1 = Mathf.Atan(x1 / y1) * 180 / Mathf.PI;
                    theta_2 = -1 * Mathf.Atan((y1 - aimY) / (x1 - aimX)) * 180 / Mathf.PI + (90 - theta_1);
                    Debug.Log($"aimx{aimX}");
                    Debug.Log($"x1{x1}");
                }
                else if(aimX > x2)
                {
                    theta_1 = Mathf.Atan(x2 / y2) * 180 / Mathf.PI;
                    theta_2 = -1 * Mathf.Atan((y2 - aimY) / (x2 - aimX)) * 180 / Mathf.PI + (90 - theta_1);
                    Debug.Log($"aimx{aimX}");
                    Debug.Log($"x2{x2}");
                }
                Debug.Log($"角1{theta_1}");
                Debug.Log($"角2{theta_2}");
                if (theta_1 > 150 || theta_1 < 0 || theta_2 > 150 || theta_2 < 0)
                {
                    Debug.LogWarning("???");
                    return;
                }
            }
            
            RotateTo(shaft1, 45f - theta_1);
            RotateTo(shaft2, 95f - theta_2);
        }

        private void RotateTo(ArticulationBody articulation, float primaryAxisRotation)
        {
            var drive = articulation.zDrive;
            drive.target = primaryAxisRotation;
            articulation.zDrive = drive;
        }
    }
}

