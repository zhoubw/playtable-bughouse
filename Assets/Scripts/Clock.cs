using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Clock : MonoBehaviour {
	public static float defaultTime = 180000f;
	public Text text;
	public float timeRemaining = 180000f;
	public bool running = false;

	void Start() {
		//running = true;
		//testText.text = timeRemaining + "";
		text.color = Color.green;
		updateText ();
	}

	void Update() {
		if (running) {
			timeRemaining -= Time.deltaTime * 1000f;

			updateText ();
			//testText.text = timeRemaining + "";

			if (timeRemaining <= 0) {
				timeRemaining = 0;
				updateText ();
				//testText.text = timeRemaining + "";
				stopClock();
				GameManager gm = GetComponentInParent<GameManager> ();
				gm.flagged = true;
				Board board = GetComponentInParent<Board> ();
				board.stopBoard ();
			}

		}
	}

	public void updateText() {
		setColor ();
		int minutes = (int) (timeRemaining / (1000 * 60)) % 60;
		int seconds = (int) (timeRemaining / 1000) % 60;
		int ms = (int) timeRemaining % 1000;
		string minuteStr = minutes + "";
		/*
		if (minuteStr.Length == 1) {
			minuteStr = "0" + minuteStr;
		}
		*/
		string secondStr = seconds + "";

		if (secondStr.Length == 1) {
			secondStr = "0" + secondStr;
		}

		string msStr = ms + "";
		if (msStr.Length > 2) {
			msStr = msStr.Substring (0, 2);
		}

		if (msStr.Length == 1) {
			msStr = "0" + msStr;
		}

		text.text = minuteStr + ":" + secondStr + ":" + msStr;
	}

	public void startClock() {
		running = true;
	}

	public void stopClock() {
		running = false;
	}

	public void setColor() {
		if (timeRemaining < 60000f) {
			text.color = Color.red;
		} else if (timeRemaining < 120000f) {
			text.color = Color.yellow;
		} else {
			text.color = Color.green;
		}
	}

	public void resetClock() {
		stopClock ();
		timeRemaining = defaultTime;
		updateText ();
	}
}