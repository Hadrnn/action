using UnityEngine;
using UnityEngine.UI;

public class BaseCapture : MonoBehaviour 
{
    public const float pointsDelta = 10f;
    public float pointsToCapture = 100;

    public const float staticIncrease = 0.1f;
    public const float staticDecrease = 0.3f;

    public Light teamLight;

    public Slider CaptureSlider;
    public Image CaptureImage;


    public int occupantTeam { get; set; }
    public int contesterTeam { get; set; }


    private float currentPoints;
    private int contesterCount;
    private int occupantCount;
    private bool isCaptured;

    private void Awake()
    {
        currentPoints = 0;

        contesterTeam = -1;
        contesterCount = 0;

        occupantTeam = -1;
        occupantCount = 0;

        CaptureSlider.maxValue = pointsToCapture;
        CaptureSlider.minValue = 0;

        Color captureColor = Color.white;
        captureColor.a = 0.5f;
        CaptureImage.color = captureColor;

        isCaptured = false;
    }   

    private void OnTriggerEnter(Collider other)
    {
        TankShooting tank = other.GetComponent<TankShooting>();
        if (!tank) return;

        InfoCollector.Team.TankHolder holder = tank.tankHolder;

        //Debug.Log("On enter");
        //Debug.Log(holder.team.teamNumber);

        if (holder.team.teamNumber == occupantTeam)
        {
            ++occupantCount;
            //Debug.Log("Added an occupant");
            return;
        };

        if (contesterTeam == -1)
        {
            contesterTeam = holder.team.teamNumber;
            ++contesterCount;
            //Debug.Log("Added a new contester");

        }
        else if (contesterTeam == holder.team.teamNumber)
        {
            //Debug.Log("Added a contester");
            ++contesterCount;
        }
    }

    private void FixedUpdate()
    {
        //Debug.Log(currentPoints);
        CaptureSlider.value = currentPoints;

        if (occupantTeam == -1 && contesterTeam == -1) return;

        if (occupantCount != 0 && contesterCount != 0)
        {
            //Debug.Log("Contest");
            //Debug.Log(occupantCount);
            //Debug.Log(contesterCount);  
            return;
        }

        if (occupantTeam != contesterTeam && occupantCount == 0 && contesterCount != 0)
        {
            currentPoints -= pointsDelta * contesterCount * Time.fixedDeltaTime;
            //Debug.Log("Uncapturing a point, team " + contesterTeam.ToString() + " with " + contesterCount.ToString() + " contesters");

            if (currentPoints < 0)
            {
                //Debug.Log("Team lost a point, point neutral");
                isCaptured = false;
                occupantTeam = contesterTeam;
                occupantCount = contesterCount;
                contesterTeam = -1;
                contesterCount = 0;

                teamLight.color = Color.white;

                Color sliderColor;
                if (occupantTeam == GameSingleton.GetInstance().playerTeam) sliderColor = Color.blue;
                else sliderColor = Color.red;

                sliderColor.a = 0.5f;

                CaptureImage.color = sliderColor;

                //Debug.Log("Occupant" + occupantTeam.ToString());
                //Debug.Log("Contester" + contesterTeam.ToString());

            }
            return;
        }

        if (occupantCount != 0 && contesterCount == 0)
        {
            if (currentPoints < pointsToCapture)
            {
                currentPoints += pointsDelta * occupantCount * Time.fixedDeltaTime;
            }
            else
            {
                //Debug.Log("Point captured");
                isCaptured = true;

                if (occupantTeam == GameSingleton.GetInstance().playerTeam)
                    teamLight.color = Color.blue;
                else teamLight.color = Color.red;
            }
        }

        if (occupantCount == 0 && contesterCount == 0)
        {
            if (isCaptured && currentPoints < pointsToCapture)
            {
                //Debug.Log("Gaining point");
                currentPoints += pointsDelta * staticIncrease * Time.fixedDeltaTime;
                return;
            }
            if(!isCaptured && currentPoints > 0)
            {
                currentPoints -= pointsDelta * staticDecrease * Time.fixedDeltaTime;
                return;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        InfoCollector.Team.TankHolder holder = other.GetComponent<TankShooting>().tankHolder;

        //Debug.Log("On exit");
        //Debug.Log(holder.team.teamNumber);

        if (holder.team.teamNumber == occupantTeam)
        {
            --occupantCount;
            //Debug.Log("Removed an occupant");
            return;
        };

        if (holder.team.teamNumber == contesterTeam)
        {
            //Debug.Log("Removed a contester");
            --contesterCount;
        };
    }
}
