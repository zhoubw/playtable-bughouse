using UnityEngine;

public class GameManager : MonoBehaviour {
	public Board boardA;
	public Board boardB;

	public bool checkmated = false;
	public bool kingCapture = false;
	public bool flagged = false;
	public bool gameRunning = false;

	/*
	void Awake(){
		boardA.startBoard ();
		boardB.startBoard ();
	}
	*/

	public void start() {
		boardA.startBoard ();
		boardB.startBoard ();
	}

	public void stop() {
		boardA.stopBoard ();
		boardB.stopBoard ();
	}

	public void resetBoards() {
		boardA.resetBoard ();
		boardB.resetBoard ();
	}

	public bool checkpoint() {
		return !(checkmated || kingCapture || flagged || !gameRunning);
	}

	public void reset() {
		checkmated = false;
		kingCapture = false;
		flagged = false;
		gameRunning = false;
	}

	public void stopAllClocks() {
		boardA.WhiteClock.GetComponent<Clock> ().stopClock();
		boardA.BlackClock.GetComponent<Clock> ().stopClock();
		boardB.WhiteClock.GetComponent<Clock> ().stopClock();
		boardB.BlackClock.GetComponent<Clock> ().stopClock();
	}
}