using UnityEngine;

public class PlayerAndEnemyHealthSystem : MonoBehaviour
{
    //Currant Health is Show How Much Health Is Remain
    [HideInInspector] public float CurrantHealth;
    //Set Maximum Health 
    [SerializeField] float MaxHealth;

    private void Awake()
    {
        //Set Currant Health To MaxHealth
        CurrantHealth = MaxHealth;
    }

    //Get The Damage 
    //TackDamage(How Much Damage Player Or Enemy Will Tack)
    public void TackDamage(float damage)
    {
        // Subtrack Damage From Currant Health 
        CurrantHealth -= damage;

        //If Currant Health is Low Then 0 Then Die
        if (CurrantHealth <= 0f)
        {
            Die();
        }
    }

    //Perform Death
    void Die()
    {
        //If It Is Enemy then
        if (transform.tag=="Enemy")
        {
            //Call EnemyDie() From EnemyAI Sctipt
            transform.GetComponent<EnemyAI>().EnemyDie();

            //Destroy Enemy In 5 Sec
            Destroy(gameObject, 5f);
        }

        Debug.Log(transform.name +"is die");
    }
}
