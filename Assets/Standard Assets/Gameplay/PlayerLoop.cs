using UnityEngine;
using System.Collections;

public class PlayerLoop : MonoBehaviour {
	
	public string mGameState = "MainMenu";
	private GameLoop loop; 
	
	private float heightBoostTimer = 0;
	private float speedBoostTimer = 0;
	
	private float mMaxSpeed = 0; 
	private float mMaxHeight = 0; 
	private float mMaxDistance = 0; 
	
	private float mph = 0;
	private float distance = 0;
	private float height = 0; 
	
	void Start () 
	{
		loop = GameObject.Find("Logic").GetComponent<GameLoop>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (mGameState == "Gameplay")
		{
			// HEIGHT!
			if (heightBoostTimer > 0)
			{
				Vector3 force = constantForce.force;
				force.y = loop.BOOST_Y_FORCE;
				constantForce.force = force;
	
				heightBoostTimer -= Time.fixedDeltaTime;
			}
			if (heightBoostTimer < 0)
			{
				heightBoostTimer = 0; 
	
				Vector3 force = constantForce.force;
				force.y = loop.DEFAULT_Y_FORCE;
				constantForce.force = force;
			}
			
			// SPEED!
			if (speedBoostTimer > 0)
			{
				Vector3 force = constantForce.force;
				force.z = loop.BOOST_Z_FORCE;
				constantForce.force = force;
	
				speedBoostTimer -= Time.fixedDeltaTime;
			}
			if (speedBoostTimer < 0)
			{
				speedBoostTimer = 0; 
	
				Vector3 force = constantForce.force;
				force.z = loop.DEFAULT_Z_FORCE;
				constantForce.force = force;
			}			
			
			mph = rigidbody.velocity.magnitude * 2.237f;
			height = transform.position.y * 3.28084f - 14.5f;
			distance = transform.position.z;
		
			if (mph > mMaxSpeed)
			{
				mMaxSpeed = mph; 
			}
			if (height > mMaxHeight)
			{
				mMaxHeight = height; 
			}
			if (distance > mMaxDistance)
			{
				mMaxDistance = distance;
			}
		}
		
		if (mGameState == "Limbo" && rigidbody.velocity.z < 5)
		{
			CheckForHighScores();
			
			if (DidPassAMission())
			{
				loop.SwitchState("Missions");
			}
			else
			{
				loop.SwitchState("PostGame");
			}
		
		}
	}
	
	void OnCollisionEnter(Collision col)
	{
		if (mGameState == "Gameplay")
		{
			// stop his downward force and make a big impact 
			constantForce.force = Vector3.zero;
			rigidbody.AddForceAtPosition(new Vector3(0,-40,-5), new Vector3(0,1,4), ForceMode.Impulse);

			// change game state
			loop.SwitchState("Limbo");
		}
	}
	
	private void CheckForHighScores()
	{
		bool needSave = false; 
		
		if(mMaxSpeed > PersistentData.mPersistentData.mUserData.BestSpeed)
		{
			PersistentData.mPersistentData.mUserData.BestSpeed = mMaxSpeed;
			needSave = true; 
		}
		if(mMaxHeight > PersistentData.mPersistentData.mUserData.BestHeight)
		{
			PersistentData.mPersistentData.mUserData.BestHeight = mMaxHeight;
			needSave = true; 
		}
		if(mMaxDistance > PersistentData.mPersistentData.mUserData.BestDistance)
		{
			PersistentData.mPersistentData.mUserData.BestDistance = mMaxDistance;
			needSave = true; 
//			StartCoroutine(PostScores(PersistentData.mPersistentData.mUserData.Id,mMaxDistance));
		}
		
		if (needSave)
		{
			PersistentData.mPersistentData.mSaveLoad.SaveUser(PersistentData.mPersistentData.mUserData);
		}
	}
	
	private	bool DidPassAMission()
	{
		bool didPass = false; 
		
		Mission missionToCheck = PersistentData.mPersistentData.mMissionData[PersistentData.mPersistentData.mUserData.Mission1Id] as Mission;
		if (missionToCheck != null)
		{
			if (CheckOperator(GetValToCheck(missionToCheck.Condition1),missionToCheck.Operator1,missionToCheck.Value1) || CheckOperator(GetValToCheck(missionToCheck.Condition2),missionToCheck.Operator2,missionToCheck.Value2))
			{
				Debug.Log ("Passed mission " + missionToCheck.Id + "! Unlocking mission " + missionToCheck.UnlockId);
				PersistentData.mPersistentData.mUserData.PrevMission1Id = PersistentData.mPersistentData.mUserData.Mission1Id;
				PersistentData.mPersistentData.mUserData.Mission1Id = missionToCheck.UnlockId;
				didPass = true; 
			}
		}

		missionToCheck = PersistentData.mPersistentData.mMissionData[PersistentData.mPersistentData.mUserData.Mission2Id] as Mission;
		if (missionToCheck != null)
		{
			if (CheckOperator(GetValToCheck(missionToCheck.Condition1),missionToCheck.Operator1,missionToCheck.Value1) || CheckOperator(GetValToCheck(missionToCheck.Condition2),missionToCheck.Operator2,missionToCheck.Value2))
			{
				Debug.Log ("Passed mission " + missionToCheck.Id + "! Unlocking mission " + missionToCheck.UnlockId);
				PersistentData.mPersistentData.mUserData.PrevMission2Id = PersistentData.mPersistentData.mUserData.Mission2Id;
				PersistentData.mPersistentData.mUserData.Mission2Id = missionToCheck.UnlockId;
				didPass = true; 
			}
		}
		
		missionToCheck = PersistentData.mPersistentData.mMissionData[PersistentData.mPersistentData.mUserData.Mission3Id] as Mission;
		if (missionToCheck != null)
		{
			if (CheckOperator(GetValToCheck(missionToCheck.Condition1),missionToCheck.Operator1,missionToCheck.Value1) || CheckOperator(GetValToCheck(missionToCheck.Condition2),missionToCheck.Operator2,missionToCheck.Value2))
			{
				Debug.Log ("Passed mission " + missionToCheck.Id + "! Unlocking mission " + missionToCheck.UnlockId);
				PersistentData.mPersistentData.mUserData.PrevMission3Id = PersistentData.mPersistentData.mUserData.Mission3Id;
				PersistentData.mPersistentData.mUserData.Mission3Id = missionToCheck.UnlockId;
				didPass = true; 
			}
		}
		
		return didPass;
	
	}
	
	float GetValToCheck(string lhs)
	{
		float valToCheck = -1.0f;

		switch (lhs)
		{
			case "distance":
				valToCheck = mMaxDistance;
				break;
				
			case "height":
				valToCheck = mMaxHeight;
				break;
			
			case "speed":
				valToCheck = mMaxSpeed;
				break;
			
			default:
				break;
		}
		
		return valToCheck;
		
	}
	
	bool CheckOperator(float lhs, string op, float rhs)
	{
		bool completed = false; 
		
		if (op != null && op != "")
		{
			switch (op)
			{
			case ">":
				if (lhs > rhs)
				{
					completed = true; 
				}
				break;
			case ">=":
				if (lhs >= rhs)
				{
					completed = true; 
				}
				break;
			case "<":
				if (lhs < rhs)
				{
					completed = true; 
				}
				break;
			case "<=":
				if (lhs <= rhs)
				{
					completed = true; 
				}
				break;
			case "!=":
				if (lhs != rhs)
				{
					completed = true; 
				}
				break;
			case "=":
			case "==":
				if (lhs == rhs)
				{
					completed = true; 
				}
				break;
			default:
				break;
			}
			
		}
		
		
		return completed;
	}
	
	public void AddHeightBoost(float time)
	{
		heightBoostTimer += time; 
	}
	
	public void AddSpeedBoost(float time)
	{
		speedBoostTimer += time; 
	}
	
    private string secretKey = "POOhm0ox5^t!$c0vo505c@#i5hk+0f0bsrs!@7pdojf0ymf#2)oPOO"; // Edit this value and make sure it's the same as the one stored on the server
    private string addScoreURL = "http://www.rowshambow.com/itctest/addscore.php?"; //be sure to add a ? to your url
    private string highscoreURL = "http://www.rowshambow.com/itctest/display.php";
	
	// remember to use StartCoroutine when calling this function!
    IEnumerator PostScores(string name, float score)
    {
        //This connects to a server side php script that will add the name and score to a MySQL DB.
        // Supply it with a string representing the players name and the players score.
        string hash = PersistentData.mPersistentData.Md5Sum(name + score + secretKey);
 
        string post_url = addScoreURL + "name=" + WWW.EscapeURL(name) + "&score=" + score + "&hash=" + hash;
 
        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done
 
        if (hs_post.error != null)
        {
            Debug.Log("There was an error posting the high score: " + hs_post.error);
        }
    }
 
    // Get the scores from the MySQL DB to display in a GUIText.
    // remember to use StartCoroutine when calling this function!
    IEnumerator GetScores()
    {
        gameObject.guiText.text = "Loading Scores";
        WWW hs_get = new WWW(highscoreURL);
        yield return hs_get;
 
        if (hs_get.error != null)
        {
            Debug.Log("There was an error getting the high score: " + hs_get.error);
        }
        else
        {
//            gameObject.guiText.text = hs_get.text; // this is a GUIText that will display the scores in game.
        }
    }	
	
}
