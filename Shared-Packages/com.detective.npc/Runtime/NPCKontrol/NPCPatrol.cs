using UnityEngine;
using UnityEngine.AI;

public class NPCPatrol : MonoBehaviour
{
    [SerializeField] private Transform[] rotaNoktalari;
    [SerializeField] private float beklemeSuresi = 2f;

    private NavMeshAgent agent;
    private int index;
    private float timer;
    private bool bekliyor;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (rotaNoktalari.Length > 0)
            agent.SetDestination(rotaNoktalari[0].position);
    }

    private void Update()
    {
        if (rotaNoktalari.Length == 0) return;

        if (bekliyor)
        {
            timer += Time.deltaTime;

            if (timer >= beklemeSuresi)
            {
                bekliyor = false;
                SonrakiNokta();
            }

            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            bekliyor = true;
            timer = 0f;
        }
    }

    private void SonrakiNokta()
    {
        index++;
        if (index >= rotaNoktalari.Length)
            index = 0;

        agent.SetDestination(rotaNoktalari[index].position);
    }

    public void Dur()
    {
        agent.isStopped = true;
    }

    public void Devam()
    {
        agent.isStopped = false;
    }
}