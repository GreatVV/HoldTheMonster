using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;
    [SerializeField]
    private float startImpulse = 4;
	// Use this for initialization
	void Start ()
	{
	    rigidbody2D = GetComponent<Rigidbody2D>();
	    InitialImpulse();
	}

    public void InitialImpulse()
    {
        var randomVector = new Vector2(
            Random.value - 0.5f,
            Random.value - 0.5f
            ) * startImpulse;
        rigidbody2D.AddForce(randomVector, ForceMode2D.Impulse);

    }
}