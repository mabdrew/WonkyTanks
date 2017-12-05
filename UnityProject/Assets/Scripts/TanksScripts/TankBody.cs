using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEngine.Time;
using TankMessages;
using LevelMessages;
using UIMessages;
using MapMessages;
using System;


public class TankBody : MonoBehaviour {

    // private Rigidbody rigBod;

    public float PlayerSpeed;
    public float RotateSpeed;
    public float StrafeSpeed;

    public KeyCode Forward;
    public KeyCode Backward;
    public KeyCode Left;
    public KeyCode Right;
    public KeyCode StrafeLeft;
    public KeyCode StrafeRight;


    GameObject OwningGame;
    int CurrentFrame; //Q : figure out how to fix framerates. Better way to track frames?
    public byte TankID;

    private float Stamina;
    private float health;
	private float TankRot;
    private bool isFinishActive;

    [SerializeField] private Text FailureText;

    [SerializeField] private Text isFinishActiveText;

    public byte GetTankID()
    {
        return TankID;
    }

    // Use this for initialization
    void Start () 
    {
        Forward = KeyCode.W;
        Backward = KeyCode.S;
        Left = KeyCode.A;
        Right = KeyCode.D;
        StrafeLeft = KeyCode.Z;
        StrafeRight = KeyCode.X;
		TankRot = 0f;
        //on start up find your parent game
        GameUtilities.FindGame(ref OwningGame);

        StrafeSpeed = 2.0f;
        Stamina = 100.0f;
        health = 100.0f;

        IsFinishActive = false;
    }

    void TurnTank(TankComponentMovementMsg msg) //rotate Left or Right
    {
        if(msg.TankID==TankID)
        {
            if(msg.Direction)
				transform.Rotate (Vector3.up, RotateSpeed * Time.deltaTime);
            else
				transform.Rotate (Vector3.up, -RotateSpeed * Time.deltaTime);
        }
    }

    void MoveTank(TankComponentMovementMsg msg)
    {   
        if(msg.TankID==TankID)
        {
            if (msg.Direction)
            {   //forward
                transform.position += transform.forward * Time.deltaTime * PlayerSpeed;
            }
            else
            {   //backward
                transform.position -= transform.forward * Time.deltaTime * PlayerSpeed;
            }
        }
    }

	void SyncTankPosition(SyncTankPositionMsg msg)
	{	//uses quaternion and position info to set tank transform
		if (msg.TankID == TankID) {
			Vector3 NewPos;
			Quaternion NewRot = new Quaternion (msg.xQuat,msg.yQuat,msg.zQuat,msg.wQuat);

			NewPos.x = msg.xPos;
			NewPos.y = msg.yPos;
			NewPos.z = msg.zPos;

			transform.position = NewPos;
			transform.rotation = NewRot;
		}
	}

    void StrafeTank(TankComponentMovementMsg msg)
    {   
        //dir tid, fno
        if(msg.TankID==TankID)
        {
            if(msg.Direction)
            {//strafeleft
                if (Stamina > 0.0f)
                {
                    transform.position -= transform.right * Time.deltaTime * StrafeSpeed;
                    Stamina -= (2.0f + Math.Abs(Stamina / 100f) + .01f);
                    UpdateHealthOrStamina(Stamina, 0.0f, 100.0f, 1);
                }
            }
            else
            {//straferight
                if (Stamina > 0.0f)
                {
                    transform.position += transform.right * Time.deltaTime * StrafeSpeed;
                    Stamina -= (2.0f + Math.Abs(Stamina / 100f) + .01f);
                    UpdateHealthOrStamina(Stamina, 0.0f, 100.0f, 1);
                }
            }
        }
    }

    private void UpdateHealthOrStamina(float currentValue, float minValue, float maxValue, int barID)
    {
        UpdateBar msg = new UpdateBar(currentValue, minValue, maxValue, barID);
        GameUtilities.Broadcast ("HandleBar", msg);
    }


    void CheckMoveTank()
    {
        if(Input.GetKey(Forward))
        {   //forward
            TankComponentMovementMsg msg = new TankComponentMovementMsg(TankID, CurrentFrame, true);
            GameUtilities.Broadcast ("MoveTank", msg);
        }
        if(Input.GetKey(Backward))
        {   //backward
            TankComponentMovementMsg msg = new TankComponentMovementMsg(TankID, CurrentFrame, false);
            GameUtilities.Broadcast ("MoveTank", msg);
        }
    }

    void CheckTurnTank()
    {
        if (Input.GetKey(Left))
        {
            TankComponentMovementMsg msg = new TankComponentMovementMsg(TankID, CurrentFrame, false);
            GameUtilities.Broadcast ("TurnTank", msg);
        }
        if (Input.GetKey(Right))
        {
            TankComponentMovementMsg msg = new TankComponentMovementMsg(TankID, CurrentFrame, true);
            GameUtilities.Broadcast ("TurnTank", msg);
        }
    }

    void CheckStrafeTank()
    {
        if (Input.GetKey(StrafeLeft))
        {
            TankComponentMovementMsg msg = new TankComponentMovementMsg(TankID, CurrentFrame, true);
            GameUtilities.Broadcast ("StrafeTank", msg);
        }
        if (Input.GetKey(StrafeRight))
        {
            TankComponentMovementMsg msg = new TankComponentMovementMsg(TankID, CurrentFrame, false);
            GameUtilities.Broadcast ("StrafeTank", msg);
        }
    }

    void MoveTank()
    {   
        CheckMoveTank();
        CheckTurnTank();
        CheckStrafeTank();
    }

    void DamageTank(DamageTankMsg dmsg)
    {
        if(dmsg.TankID==TankID)
        {
            health -= dmsg.Amount;
            UpdateHealthOrStamina(health, 0.0f, 100.0f, 0);
        }
    }

    void HealStamina()
    {
        if (Stamina > 100.0f)
        {
            Stamina = 100.0f;
            UpdateHealthOrStamina(Stamina, 0.0f, 100.0f, 1);
        }
            
        else if (Stamina < 100.0f)
        {
            Stamina += Math.Abs(Stamina / 100f) + .01f;
            UpdateHealthOrStamina(Stamina, 0.0f, 100.0f, 1);
        }
        if (Stamina < -5.0f)
        {
            Stamina = 0.0f;
            UpdateHealthOrStamina(Stamina, 0.0f, 100.0f, 1);
        }
            
        //print(Stamina);
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        CurrentFrame = Time.frameCount;
        //madbrew : what about order on recieving end?
        MoveTank();
        HealStamina();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            DamageTankMsg msg = new DamageTankMsg(0, CurrentFrame, 20.0f);
            GameUtilities.Broadcast ("DamageTank", msg);
        }

        if(health <= 0.0f)
        {
            FailLevel();
        }

    }

    private void FailLevel()
    {
        GameUtilities.Broadcast("DisableMovement", "");
        FailureText.gameObject.SetActive(true);
        Invoke("EndLevel", 5.0f);

    }

    private void EndLevel()
    {
        GameUtilities.Broadcast ("LoadNext", new LoadNextSceneMsg(SceneName.Title));
    }

    void OnTriggerEnter(Collider theOther)
    {
        if (theOther.gameObject.CompareTag("Finish") && IsFinishActive == true)
        {
            EndLevel ();
        }
    }

    public bool IsFinishActive
    {
        get
        {
            return isFinishActive;
        }

        set
        {
            isFinishActive = value;
            if(isFinishActive == true)
            {
                isFinishActiveText.gameObject.SetActive(true);
                Invoke("DisableText", 2.5f);
            }
        }
    }

    private void SetIsFinishActive(IsFinishActiveMsg msg)
    {
        IsFinishActive = msg.isActive;
    }

    private void DisableText()
    {
        isFinishActiveText.gameObject.SetActive(false);
    }


    void DisableMovement(string emptyMessage) //is there a better way to do this?
    {
        Forward = KeyCode.F;
        Backward = KeyCode.F;
        StrafeLeft = KeyCode.F;
        StrafeRight = KeyCode.F;
    }
}
