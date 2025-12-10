using UnityEngine;

public class DiceRollScript : MonoBehaviour
{
    Rigidbody rBody;
    Vector3 position;
    [SerializeField] private float maxRandForceValue, startRollingForce;
    float forceX, forceY, forceZ;
    public string diceFaceNum;
    public bool isLanded = false;
    public bool firstThrow = false;
    private Vector3 startPosition;

    void Awake()
    {
        startPosition = transform.position;
        Initialize();
    }

    private void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
        rBody.isKinematic = true;
        position = transform.position;
        transform.rotation = new Quaternion(
            Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360), 0);
    }

    private void RollDice()
    {
        rBody.isKinematic = false;
        forceX = Random.Range(0, maxRandForceValue);
        forceY = Random.Range(0, maxRandForceValue);
        forceZ = Random.Range(0, maxRandForceValue);
        rBody.AddForce(Vector3.up * Random.Range(800, startRollingForce));
        rBody.AddTorque(forceX, forceY, forceZ);
    }

    public void ResetDice()
    {
        transform.position = startPosition;
        firstThrow = false;
        isLanded = false;
        Initialize();
    }

    void Update()
    {
        if(rBody != null)
        {
            if(Input.GetMouseButtonDown(0) && isLanded ||
                Input.GetMouseButtonDown(0) && !firstThrow)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if(Physics.Raycast(ray, out hit))
                {
                    if(hit.collider != null && hit.collider.gameObject == this.gameObject)
                    {
                        if (!firstThrow)
                        {
                            firstThrow = true;
                        }
                        RollDice();
                    }
                }

                firstThrow = true;
                isLanded = false;
                RollDice();
            }
        }
    }
}
