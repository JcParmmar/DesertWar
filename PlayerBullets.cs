using UnityEngine;

public class PlayerBullets : MonoBehaviour
{
    //Set Effects
    [Header("Partical Systems (Effects) ")]
    [SerializeField]GameObject SandPartical;
    [SerializeField] GameObject FleshPartical;
    [SerializeField] GameObject StonePartical;
    [SerializeField] GameObject MetalPartical;
    [SerializeField] GameObject WoodPartical;

    //Effect Forward is Use To set Direaction For Partical 
    //It Is Define By Shooting Script
    public Vector3 effectForward;

    //_onBulletCollision is Where the Bullet Is Collide
    Vector3 _onBulletCollision;

    //this Method Is Call When Object Is Collide With Some thing Which Have Colider
    private void OnCollisionEnter(Collision collision)
    {
        //Set _onBulletCollision Where Bullet Is Colide
        _onBulletCollision = transform.position;

        //Check Which Type Of Object Is Bullet Colide
        /*{
                Create Particals at _onBulletCollision
                Set Particals Look Forward = effectForward
                Emit Partical For Just 1 Time
                Destroy Partical After 2 Sec
        }*/

        if (collision.transform.tag== "Ground")
        {
            GameObject partical = Instantiate(SandPartical, _onBulletCollision, Quaternion.identity);
            partical.transform.forward = effectForward;
            partical.GetComponent<ParticleSystem>().Emit(1);
            Destroy(partical, 2f);
        }
        else if (collision.transform.tag == "Enemy")
        {
            GameObject partical = Instantiate(FleshPartical, _onBulletCollision, Quaternion.identity);
            partical.transform.forward = effectForward;
            partical.GetComponent<ParticleSystem>().Emit(1);
            Destroy(partical, 2f);
        }
        else if (collision.transform.tag == "Player")
        {
            GameObject partical = Instantiate(FleshPartical, _onBulletCollision, Quaternion.identity);
            partical.transform.forward = effectForward;
            partical.GetComponent<ParticleSystem>().Emit(1);
            Destroy(partical, 2f);
        }
        else if (collision.transform.tag == "Metal")
        {
            GameObject partical = Instantiate(MetalPartical, _onBulletCollision, Quaternion.identity);
            partical.transform.forward = effectForward;
            partical.GetComponent<ParticleSystem>().Emit(1);
            Destroy(partical, 2f);
        }
        else if (collision.transform.tag == "Stone")
        {
            GameObject partical = Instantiate(StonePartical, _onBulletCollision, Quaternion.identity);
            partical.transform.forward = effectForward;
            partical.GetComponent<ParticleSystem>().Emit(1);
            Destroy(partical, 2f);
        }
        else if (collision.transform.tag == "Wood")
        {
            GameObject partical = Instantiate(WoodPartical, _onBulletCollision, Quaternion.identity);
            partical.transform.forward = effectForward;
            partical.GetComponent<ParticleSystem>().Emit(1);
            Destroy(partical, 2f);
        }
        else
        {
            Debug.Log(collision.transform.name);
        }

        //Check if Collision Object Have Health System
        var healthScript = collision.transform.GetComponent<PlayerAndEnemyHealthSystem>();
        if (healthScript != null)
        {
            //If It is Then Perform Damage Operation
            healthScript.TackDamage(10f);
        }

        //Destroy Bullet After Collision
        Destroy(gameObject);
    }
}
