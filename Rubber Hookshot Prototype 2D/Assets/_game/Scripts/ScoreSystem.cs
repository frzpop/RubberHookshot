
using UnityEngine;
using UnityEngine.UI;

public class ScoreSystem : MonoBehaviour {

	public static ScoreSystem ss;

	public LevelGeneratorNew levelGenScr;
	public Text scoreText;
	int currentScore;
	int scoreIndex;
	int scoreMultiplier = 1;
	float[] scoreCheckpoints;

	void Awake()
	{
		ss = this;
	}

	void Start()
	{
		RequestScoreCheckpoints();
	}
		
	public void UpdateScore ( Vector2 playerPos )
	{
		if ( scoreIndex < scoreCheckpoints.Length )
		{
			if ( playerPos.x > scoreCheckpoints[scoreIndex] ) 
			{
				scoreIndex++;
				currentScore += 1 * scoreMultiplier;
				scoreText.text = currentScore.ToString();
			}
		}
		else
			RequestScoreCheckpoints ();

	}

	void RequestScoreCheckpoints ()
	{
		scoreCheckpoints = levelGenScr.GetColVertsX ();
	}
}
