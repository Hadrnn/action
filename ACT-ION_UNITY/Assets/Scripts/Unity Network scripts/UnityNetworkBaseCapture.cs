using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class UnityNetworkBaseCapture : NetworkBehaviour
{
    public const float pointsDelta = 10f;
    public float pointsToCapture = 100;

    public const float staticIncrease = 0.1f;
    public const float staticDecrease = 0.3f;

    public Light teamLight;

    public Slider CaptureSlider;
    public Image CaptureImage;

    public NetworkVariable<int> occupantTeam { get; set; }
    public NetworkVariable<int> contesterTeam { get; set; }


    private NetworkVariable<float> currentPoints;
    private NetworkVariable<int> contesterCount;
    private NetworkVariable<int> occupantCount;
    private NetworkVariable<bool> isCaptured;

    private const int empty = -1;

    private void Awake()
    {
        currentPoints = new NetworkVariable<float>(0);

        contesterTeam = new NetworkVariable<int>(empty);
        contesterCount = new NetworkVariable<int>(0);

        occupantTeam = new NetworkVariable<int>(empty);
        occupantCount = new NetworkVariable<int>(0);

        CaptureSlider.maxValue = pointsToCapture;
        CaptureSlider.minValue = 0;

        Color captureColor = Color.white;
        captureColor.a = 0.5f;
        CaptureImage.color = captureColor;

        isCaptured = new NetworkVariable<bool>(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        UnityNetworkTankShooting tank = other.GetComponent<UnityNetworkTankShooting>();
        if (!tank) return;

        InfoCollector.Team.TankHolder holder = tank.tankHolder;

        //Debug.Log("On enter");
        //Debug.Log(holder.team.teamNumber);

        if (holder.team.teamNumber == occupantTeam.Value)
        {
            ++occupantCount.Value;
            //Debug.Log("Added an occupant");
            return;
        };

        if (contesterTeam.Value == empty)
        {
            contesterTeam.Value = holder.team.teamNumber;
            ++contesterCount.Value;
            //Debug.Log("Added a new contester");

        }
        else if (contesterTeam.Value == holder.team.teamNumber)
        {
            //Debug.Log("Added a contester");
            ++contesterCount.Value;
        }
    }

    private void FixedUpdate()
    {
        //Debug.Log(currentPoints);
        CaptureSlider.value = currentPoints.Value;

        if (!IsServer) return;

        if (occupantTeam.Value == empty && contesterTeam.Value == empty) return;

        if (occupantCount.Value != 0 && contesterCount.Value != 0)
        {
            //Debug.Log("Contest");
            //Debug.Log(occupantCount);
            //Debug.Log(contesterCount);  
            return;
        }

        if (occupantTeam != contesterTeam && occupantCount.Value == 0 && contesterCount.Value != 0)
        {
            currentPoints.Value -= pointsDelta * contesterCount.Value * Time.fixedDeltaTime;
            //Debug.Log("Uncapturing a point, team " + contesterTeam.ToString() + " with " + contesterCount.ToString() + " contesters");

            if (currentPoints.Value < 0)
            {
                //Debug.Log("Team lost a point, point neutral");
                isCaptured.Value = false;
                occupantTeam = contesterTeam;
                occupantCount = contesterCount;
                contesterTeam.Value = empty;
                contesterCount.Value = 0;


                //// GONE TO CLIENT RPC
                //teamLight.color = Color.white;
                //
                //Color sliderColor;
                //if (occupantTeam.Value == GameSingleton.GetInstance().playerTeam) sliderColor = Color.blue;
                //else sliderColor = Color.red;
                //
                //sliderColor.a = 0.5f;
                //
                //CaptureImage.color = sliderColor;

                SetSliderColorClientRpc(occupantTeam.Value);



                //Debug.Log("Occupant" + occupantTeam.ToString());
                //Debug.Log("Contester" + contesterTeam.ToString());

            }
            return;
        }

        if (occupantCount.Value != 0 && contesterCount.Value == 0)
        {
            if (currentPoints.Value < pointsToCapture)
            {
                currentPoints.Value += pointsDelta * occupantCount.Value * Time.fixedDeltaTime;
            }
            else
            {
                //Debug.Log("Point captured");
                isCaptured.Value = true;

                //if (occupantTeam.Value == GameSingleton.GetInstance().playerTeam)
                //    teamLight.color = Color.blue;
                //else teamLight.color = Color.red;

                SetTeamLightClientRpc(occupantTeam.Value);
            }
        }

        if (occupantCount.Value == 0 && contesterCount.Value == 0)
        {
            if (isCaptured.Value && currentPoints.Value < pointsToCapture)
            {
                //Debug.Log("Gaining point");
                currentPoints.Value += pointsDelta * staticIncrease * Time.fixedDeltaTime;
                return;
            }
            if (!isCaptured.Value && currentPoints.Value > 0)
            {
                currentPoints.Value -= pointsDelta * staticDecrease * Time.fixedDeltaTime;
                return;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) return;

        InfoCollector.Team.TankHolder holder = other.GetComponent<TankShooting>().tankHolder;

        //Debug.Log("On exit");
        //Debug.Log(holder.team.teamNumber);

        if (holder.team.teamNumber == occupantTeam.Value)
        {
            --occupantCount.Value;
            //Debug.Log("Removed an occupant");
            return;
        };

        if (holder.team.teamNumber == contesterTeam.Value)
        {
            //Debug.Log("Removed a contester");
            --contesterCount.Value;
        };
    }

    [ClientRpc]
    private void SetSliderColorClientRpc(int occupantTeam_)
    {
        teamLight.color = Color.white;

        Color sliderColor;
        if (occupantTeam_ == GameSingleton.GetInstance().playerTeam) sliderColor = Color.blue;
        else sliderColor = Color.red;

        sliderColor.a = 0.5f;

        CaptureImage.color = sliderColor;
    }

    [ClientRpc]
    private void SetTeamLightClientRpc(int occupantTeam_)
    {
        if (occupantTeam_ == GameSingleton.GetInstance().playerTeam)
            teamLight.color = Color.blue;
        else teamLight.color = Color.red;
    }
}
