using UnityEngine;
using System.Collections;

public class Hook : MonoBehaviour
{

	public GameObject player;
	bool clicked = false;


	/*void Update ()
	{
		if (Input.GetMouseButtonDown(0) && clicked)
		{
			UnHookFromMe();
			clicked = false;
		}
	}

	void OnMouseOver()
	{
		if (Input.GetMouseButtonDown(0) && !clicked)
		{
			HookToMe();
			StartCoroutine("Bool");
		}
		else if (Input.GetMouseButtonDown(0) && clicked)
		{
			clicked = false;
			UnHookFromMe();
		}

	}

	/*public void HookToMe ()
	{
		player.GetComponent<Player>().Hook(transform.position);
	}

	public void UnHookFromMe ()
	{
		player.GetComponent<Player>().Unook();
	}
	IEnumerator Bool()
	{
		yield return new WaitForSeconds(0.1f);
		clicked = true;
	}*/
}
