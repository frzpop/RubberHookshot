using UnityEngine;
using System.Collections;

public class Anchor : MonoBehaviour
{

	public Player2D player;
	bool clicked = false;

	void Start ()
	{
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player2D>();
	}

	public void AnchorToMe ()
	{
		player.GetComponent<Player2D>().Anchor(transform.position);
	}
}
