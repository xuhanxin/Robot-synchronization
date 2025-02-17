using UnityEngine;
using System.Collections;
using Extend;
using App;
using Data;
using RosMessageTypes.Geometry;
using Unity.Robotics.ROSTCPConnector;
using RosVelocity = RosMessageTypes.PuppyControl.VelocityMsg;
using System.Collections.Generic;
using UI;
using SceneComponent;

namespace Actor
{
    /// <summary>
    /// 
    /// </summary>
	public class ACPuppyPI : MonoSingleton<ACPuppyPI>
    {
        /// <summary>
        /// 真实模型根节点
        /// </summary>
        protected Transform exercise;

        /// <summary>
        /// 四组运动目标
        /// </summary>
        protected Transform feetMove;

        /// <summary>
        /// 重心
        /// </summary>
        protected Transform zhongxin;

        /// <summary>
        /// 相机目标
        /// </summary>
        protected Transform cameraTarget;

        /// <summary>
        /// 前相机
        /// </summary>
        public Camera frontCamera;

        /// <summary>
        /// 后相机
        /// </summary>
        protected Camera backCamera;

        /// <summary>
        /// body
        /// </summary>
        protected Transform body;

        /// <summary>
        /// 坐标
        /// </summary>
        protected Vector3 bodyPos;

        /// <summary>
        /// 旋转角
        /// </summary>
        protected Vector3 bodyRot;

        /// <summary>
        /// 力效率
        /// </summary>
        protected float ratio = 0.001f;

        /// <summary>
        /// 姿态同步
        /// </summary>
        public Field<bool> poseSynchronization = new Field<bool>(true);

        protected virtual void Awake()
        {
            exercise = transform.FindTnf2("Exercise");
            feetMove = transform.FindTnf2("feetMove");
            zhongxin = transform.FindTnf2("zhongxin");
            frontCamera = transform.FindComp<Camera>("FrontCamera");
            backCamera = transform.FindComp<Camera>("BackCamera");
            cameraTarget = transform.FindTnf2("cameraTarget");
            body = transform.FindTnf2("body");
            bodyPos = body.localPosition;
            bodyRot = body.localEulerAngles;
            body.GetComponent<ArticulationBody>().centerOfMass = zhongxin.localPosition;
        }

        protected virtual void Start()
        {
            ROSConnection.GetOrCreateInstance().Subscribe<PolygonMsg>("/puppy_control/legs_coord", Polygon);
            //ROSConnection.GetOrCreateInstance().Subscribe<RosVelocity>("/puppy_control/velocity/autogait", Velocity);
        }

        protected void FixedUpdate()
        {
            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    body.GetComponent<ArticulationBody>().AddForce(new Vector3(10000, 0, 0));
            //}
            //if (Input.GetKeyDown(KeyCode.D))
            //{
            //    body.GetComponent<ArticulationBody>().AddForce(new Vector3(-10000, 0, 0));
            //}
            //if (Input.GetKeyDown(KeyCode.W))
            //{
            //    body.GetComponent<ArticulationBody>().AddForce(new Vector3(0, 0, 10000));
            //}
            //if (Input.GetKeyDown(KeyCode.S))
            //{
            //    body.GetComponent<ArticulationBody>().AddForce(new Vector3(0, 0, -10000));
            //}
            if (poseSynchronization)
            {
                //在Factory和ClassRoom打开，在ExhibitionStand关闭
                SECExhibitionStand.Instance.uiPosition.Refresh(feetMove);
                //transform.FindTnf2("body").GetComponent<ArticulationBody>().velocity = Vector3.zero;
                transform.FindTnf2("body").GetComponent<ArticulationBody>().angularVelocity = new Vector3(0, transform.FindTnf2("body").GetComponent<ArticulationBody>().angularVelocity.y, 0);
                //transform.FindTnf2("fl1").GetComponent<ArticulationBody>().velocity = Vector3.zero;
                //transform.FindTnf2("fl1").GetComponent<ArticulationBody>().angularVelocity = Vector3.zero;
                ////transform.FindTnf2("fl2").GetComponent<ArticulationBody>().velocity = Vector3.zero;
                //transform.FindTnf2("fl2").GetComponent<ArticulationBody>().angularVelocity = Vector3.zero;
                ////transform.FindTnf2("fr1").GetComponent<ArticulationBody>().velocity = Vector3.zero;
                //transform.FindTnf2("fr1").GetComponent<ArticulationBody>().angularVelocity = Vector3.zero;
                ////transform.FindTnf2("fr2").GetComponent<ArticulationBody>().velocity = Vector3.zero;
                //transform.FindTnf2("fr2").GetComponent<ArticulationBody>().angularVelocity = Vector3.zero;
                ////transform.FindTnf2("bl1").GetComponent<ArticulationBody>().velocity = Vector3.zero;
                //transform.FindTnf2("bl1").GetComponent<ArticulationBody>().angularVelocity = Vector3.zero;
                ////transform.FindTnf2("bl2").GetComponent<ArticulationBody>().velocity = Vector3.zero;
                //transform.FindTnf2("bl2").GetComponent<ArticulationBody>().angularVelocity = Vector3.zero;
                ////transform.FindTnf2("br1").GetComponent<ArticulationBody>().velocity = Vector3.zero;
                //transform.FindTnf2("br1").GetComponent<ArticulationBody>().angularVelocity = Vector3.zero;
                ////transform.FindTnf2("br2").GetComponent<ArticulationBody>().velocity = Vector3.zero;
                //transform.FindTnf2("br2").GetComponent<ArticulationBody>().angularVelocity = Vector3.zero;
                transform.FindComp<ECLeg>("fl1").CalculatingAngle(feetMove.FindTnf2("fl").localPosition.x, feetMove.FindTnf2("fl").localPosition.z);
                transform.FindComp<ECLeg>("fr1").CalculatingAngle(feetMove.FindTnf2("fr").localPosition.x, feetMove.FindTnf2("fr").localPosition.z);
                transform.FindComp<ECLeg>("bl1").CalculatingAngle(feetMove.FindTnf2("bl").localPosition.x, feetMove.FindTnf2("bl").localPosition.z);
                transform.FindComp<ECLeg>("br1").CalculatingAngle(feetMove.FindTnf2("br").localPosition.x, feetMove.FindTnf2("br").localPosition.z);
            }
            cameraTarget.localPosition = new Vector3(body.localPosition.x, cameraTarget.localPosition.y, body.localPosition.z);
        }

        protected void Polygon(PolygonMsg polygonMessage)
        {
            Debug.Log(polygonMessage.ToString());
            feetMove.FindTnf2("fr").localPosition = new Vector3(polygonMessage.points[0].x, polygonMessage.points[0].y, polygonMessage.points[0].z);
            feetMove.FindTnf2("fl").localPosition = new Vector3(polygonMessage.points[1].x, polygonMessage.points[1].y, polygonMessage.points[1].z);
            feetMove.FindTnf2("br").localPosition = new Vector3(polygonMessage.points[2].x, polygonMessage.points[2].y, polygonMessage.points[2].z);
            feetMove.FindTnf2("bl").localPosition = new Vector3(polygonMessage.points[3].x, polygonMessage.points[3].y, polygonMessage.points[3].z);
        }

        protected void Velocity(RosVelocity velocityMessage)
        {
            Debug.LogWarning(velocityMessage.x);
            body.localPosition += body.right * velocityMessage.x * ratio;
        }

        protected IEnumerator Fuwei()
        {
            poseSynchronization.data = false;
            body.GetComponent<ArticulationBody>().enabled = false;
            transform.FindTnf2("fl1").GetComponent<ArticulationBody>().enabled = false;
            transform.FindTnf2("fr1").GetComponent<ArticulationBody>().enabled = false;
            transform.FindTnf2("bl1").GetComponent<ArticulationBody>().enabled = false;
            transform.FindTnf2("br1").GetComponent<ArticulationBody>().enabled = false;
            transform.FindTnf2("fl2").GetComponent<ArticulationBody>().enabled = false;
            transform.FindTnf2("fr2").GetComponent<ArticulationBody>().enabled = false;
            transform.FindTnf2("bl2").GetComponent<ArticulationBody>().enabled = false;
            transform.FindTnf2("br2").GetComponent<ArticulationBody>().enabled = false;
            yield return new WaitForSeconds(0.1f);
            body.localPosition = bodyPos;
            body.localEulerAngles = bodyRot;
            body.GetComponent<ArticulationBody>().enabled = true;
            yield return new WaitForSeconds(0.5f);
            transform.FindTnf2("fl1").GetComponent<ArticulationBody>().enabled = true;
            transform.FindTnf2("fr1").GetComponent<ArticulationBody>().enabled = true;
            transform.FindTnf2("bl1").GetComponent<ArticulationBody>().enabled = true;
            transform.FindTnf2("br1").GetComponent<ArticulationBody>().enabled = true;
            transform.FindTnf2("fl2").GetComponent<ArticulationBody>().enabled = true;
            transform.FindTnf2("fr2").GetComponent<ArticulationBody>().enabled = true;
            transform.FindTnf2("bl2").GetComponent<ArticulationBody>().enabled = true;
            transform.FindTnf2("br2").GetComponent<ArticulationBody>().enabled = true;
            poseSynchronization.data = true;
        }
    }
}

