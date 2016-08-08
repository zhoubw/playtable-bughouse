using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour {

	[System.Serializable]
	public class Column {
		public GameObject[] column = new GameObject[8];
	}

	// ultimately GameObjects that contain cells
	public Column[] row = new Column[8];

	public int currentTurn = 1; // white 1, black -1

	public GameObject oppositeBoard;
	public GameObject WhitePocket;
	public GameObject BlackPocket;
	public GameObject WhiteKing;
	public GameObject BlackKing;
	public GameObject WhiteClock;
	public GameObject BlackClock;

	public enum TurnPhase {Select, Moving};
	public TurnPhase turnPhase = TurnPhase.Select;
	public GameObject selectedPiece;

	public List<GameObject> allPieces = new List<GameObject> ();

	public GameObject get(int c, int r) {
		if (c <= 7 && c >= 0 && r <= 7 && r >= 0) {
			return this.row [r].column [c];
		}
		return null;
	}

	void Awake() {
		for (int c = 0; c < 8; c++) {
			for (int r = 0; r < 8; r++) {
				GameObject cellObject = get (c, r);
				Cell cell = cellObject.GetComponent<Cell> ();
				cell.column = c;
				cell.row = r;
				if (!cell.isEmpty()) {
					cell.piece.GetComponent<Piece> ().originalCell = cellObject;
					allPieces.Add (cell.piece);
				}
			}
		}
	}

	// start the game on this board
	public void startBoard() {
		resetHighlight ();
		//WhiteClock.GetComponent<Clock> ().resetClock ();
		//BlackClock.GetComponent<Clock> ().resetClock ();
		if (currentTurn == 1) {
			WhiteClock.GetComponent<Clock> ().startClock ();
		} else {
			BlackClock.GetComponent<Clock> ().startClock ();
		}
		//currentTurn = 1;
		turnPhase = TurnPhase.Select;
		GameManager gm = GetComponentInParent<GameManager> ();
		gm.gameRunning = true;
	}

	// stop
	public void stopBoard() {
		resetHighlight ();
		turnPhase = TurnPhase.Select;
		GameManager gm = GetComponentInParent<GameManager> ();
		gm.gameRunning = false;
		WhiteClock.GetComponent<Clock> ().stopClock ();
		BlackClock.GetComponent<Clock> ().stopClock ();
	}

	public void resetBoard() {
		/*
		List<GameObject> pieces = new List<GameObject> ();
		for (int c = 0; c < 8; c++) {
			for (int r = 0; r < 8; r++) {
				Cell cell = get (c, r).GetComponent<Cell> ();
				GameObject pieceObject = cell.piece;
				if (pieceObject != null) {
					pieces.Add (pieceObject);
				}
			}
		}

		PocketManager whiteP = WhitePocket.GetComponent<PocketManager> ();
		PocketManager blackP = BlackPocket.GetComponent<PocketManager> ();

		pieces.InsertRange (0, whiteP.KnightPocket.GetComponent<Pocket> ().pieces);
		pieces.InsertRange (0, whiteP.RookPocket.GetComponent<Pocket> ().pieces);
		pieces.InsertRange (0, whiteP.PawnPocket.GetComponent<Pocket> ().pieces);
		pieces.InsertRange (0, whiteP.QueenPocket.GetComponent<Pocket> ().pieces);
		pieces.InsertRange (0, whiteP.BishopPocket.GetComponent<Pocket> ().pieces);

		pieces.InsertRange (0, blackP.KnightPocket.GetComponent<Pocket> ().pieces);
		pieces.InsertRange (0, blackP.RookPocket.GetComponent<Pocket> ().pieces);
		pieces.InsertRange (0, blackP.PawnPocket.GetComponent<Pocket> ().pieces);
		pieces.InsertRange (0, blackP.QueenPocket.GetComponent<Pocket> ().pieces);
		pieces.InsertRange (0, blackP.BishopPocket.GetComponent<Pocket> ().pieces);

		foreach (GameObject pieceObject in pieces) {
			Piece piece = pieceObject.GetComponent<Piece> ();
			piece.moved = false;
			piece.moves = 0;
			piece.promoted = false;
			Cell oldCell = piece.GetComponentInParent<Cell> ();
			//Cell newCell = piece.originalCell.GetComponent<Cell> ();
			pieceObject.transform.SetParent (piece.originalCell.transform);
			pieceObject.transform.localPosition = new Vector3 (0, 0, -1);
			oldCell.piece = null;
		}
		//avoid collision
		foreach (GameObject pieceObject in pieces) {
			Piece piece = pieceObject.GetComponent<Piece> ();
			Cell newCell = piece.originalCell.GetComponent<Cell> ();
			newCell.piece = pieceObject;
		}
		*/

		foreach (GameObject pieceObject in allPieces) {
			Piece piece = pieceObject.GetComponent<Piece> ();
			piece.moved = false;
			piece.moves = 0;
			piece.promoted = false;
			Cell oldCell = piece.GetComponentInParent<Cell> ();
			Pocket oldPocket = piece.GetComponentInParent<Pocket> ();
			//Cell newCell = piece.originalCell.GetComponent<Cell> ();
			pieceObject.transform.SetParent (piece.originalCell.transform);
			pieceObject.transform.localPosition = new Vector3 (0, 0, -1);
			if (oldCell != null) {
				oldCell.piece = null;
			}
			if (oldPocket != null) {
				oldPocket.pieces.Remove(pieceObject);
			}
		}

		foreach (GameObject pieceObject in allPieces) {
			Piece piece = pieceObject.GetComponent<Piece> ();
			Cell newCell = piece.originalCell.GetComponent<Cell> ();
			newCell.piece = pieceObject;
		}

		turnPhase = TurnPhase.Select;
		currentTurn = 1;
		selectedPiece = null;

		WhiteClock.GetComponent<Clock> ().resetClock ();
		BlackClock.GetComponent<Clock> ().resetClock ();
		GameManager gm = GetComponentInParent<GameManager> ();
		gm.reset ();
	}

	public void resetHighlight() {
		for (int c = 0; c < 8; c++) {
			for (int r = 0; r < 8; r++) {
				Cell cell = get (c, r).GetComponent<Cell> ();
				if (cell.highlighted) {
					cell.toggleHighlight ();
				}
			}
		}
	}

	// transfer captured piece to the other board
	public void transferPiece(GameObject piece) {
		Piece p = piece.GetComponent<Piece> ();

		// the lazy way to do this
		if (p.promoted) {
			p.promoted = false;
			p.type = "P";
		}
		p.moved = false;

		GameObject pocketManager = oppositeBoard.GetComponent<Board> ().WhitePocket;;
		if (p.color == -1) {
			pocketManager = oppositeBoard.GetComponent<Board> ().BlackPocket;
		}
		PocketManager pm = pocketManager.GetComponent<PocketManager> ();
		switch (p.type) {
		case "Q":
			piece.transform.SetParent (pm.QueenPocket.transform);
			piece.transform.localPosition = new Vector3 (0, 0, -1);
			pm.QueenPocket.GetComponent<Pocket> ().add (piece);
			break;
		case "R":
			piece.transform.SetParent (pm.RookPocket.transform);
			piece.transform.localPosition = new Vector3 (0, 0, -1);
			pm.RookPocket.GetComponent<Pocket> ().add (piece);
			break;
		case "B":
			piece.transform.SetParent (pm.BishopPocket.transform);
			piece.transform.localPosition = new Vector3 (0, 0, -1);
			pm.BishopPocket.GetComponent<Pocket> ().add (piece);
			break;
		case "N":
			piece.transform.SetParent (pm.KnightPocket.transform);
			piece.transform.localPosition = new Vector3 (0, 0, -1);
			pm.KnightPocket.GetComponent<Pocket> ().add (piece);
			break;
		case "P":
			piece.transform.SetParent (pm.PawnPocket.transform);
			piece.transform.localPosition = new Vector3 (0, 0, -1);
			pm.PawnPocket.GetComponent<Pocket> ().add (piece);
			break;
		case "K":
			piece.gameObject.SetActive (false);
			GameManager gm = GetComponentInParent<GameManager> ();
			gm.kingCapture = true;
			//checkmated = true;
			break;
		}
		p.pocketed = true;
	}
		
	public GameObject getEnemyKing() {
		if (currentTurn == 1) {
			return BlackKing;
		}
		return WhiteKing;
	}

	public bool checkmate() {
		GameObject enemyKingObject = getEnemyKing ();
		Piece enemyKing = enemyKingObject.GetComponent<Piece> ();
		GameManager gm = GetComponentInParent<GameManager> ();
		if (!gm.kingCapture) {
			List<Piece> attackers = enemyKing.bughouseCheck ();
			/*
			List<bool> defended = new List<bool> ();
			foreach (Piece p in attackers) {
				defended.Add (p.inCheck ());
			}
			*/
			if (attackers.Count == 0) {
				return false;
			} else if (attackers.Count == 1) {
				List<Piece> defenders = attackers [0].inCheck (); // defending enemy king; king can't really defend
				if (defenders.Count > 0) {
					return false;
				} else if (enemyKing.canMove ()) {
					return false;
				}
			} else {
				if (enemyKing.canMove ()) {
					return false;
				}
			}
		}
		return true;
	}

	// stop all tap actions on cells (and pockets)
	public void resolveCheckmate() {
		if (checkmate ()) {
			GameManager gm = GetComponentInParent<GameManager> ();
			gm.checkmated = true;
		}
	}

	// switch clocks with turn
	public void passTurn() {
		GameManager gm = GetComponentInParent<GameManager> ();
		if (!gm.checkpoint ()) {
			gm.stopAllClocks ();
		} else {
			if (currentTurn == 1) {
				WhiteClock.GetComponent<Clock> ().stopClock ();
				BlackClock.GetComponent<Clock> ().startClock ();
			} else {
				WhiteClock.GetComponent<Clock> ().startClock ();
				BlackClock.GetComponent<Clock> ().stopClock ();
			}
			currentTurn *= -1;
		}
	}
}
