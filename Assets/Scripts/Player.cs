using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Player : MonoBehaviour {

    private static float health;
    public float MaxHealth = 120;
    public float StartingHealth = 100;

    private static bool Dead;
    public static float score { get; private set; }

    public float healthLossRate = .08f;
    public float healthRegenRate = 0.3f;
    public float healthRegenDelay = 0.3f;

    [Space]
    public float airTime;

    public Controller_Rover rover;

    private void Start()
    {
        rover = GetComponent<Controller_Rover>();
        health = StartingHealth;
        score = 0;
        airTime = 0;
    }

    void UpdateHealth()
    {
        if (OnGround())
        {
            health -= healthLossRate;
            GameManager.instance.UpdateHealth(-healthLossRate);

            if (airTime != 0)
                airTime = 0;
        }
        else
        {
            airTime += Time.deltaTime;

            if(airTime > healthRegenDelay)
            {
                if (health < MaxHealth)
                {
                    health += healthRegenRate;
                    GameManager.instance.UpdateHealth(healthRegenRate);
                }
            }
        }
    }

    void CheckHealth()
    {
        if (health <= 0)
        {
            MotorOff();
            Dead = true;
            GameManager.instance.EndGame();
            GameManager.playerDead = true;
        }
    }

    public static void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
    }

    void AccumulatePoints()
    {
        float dist = (Mathf.Pow(rover.CurrentSpeed_KPH, 2) / 1000);
        float points = dist * Time.fixedDeltaTime;
        score += points;
        GameManager.instance.UpdateScore(score);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            GameManager.instance.UpdateCoins(1);
            Destroy(other.gameObject);
        }
    }

    private void FixedUpdate()
    {
        UpdateHealth();
        CheckHealth();
        AccumulatePoints();
    }

    void MotorOff()
    {
        rover.RoverRigidBody.Sleep();
        rover.enabled = false;
    }

    bool OnGround()
    {
        return rover.Grounded;
    }

    IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }
}
