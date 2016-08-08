using UnityEngine;
using System;
using System.Collections.Generic;
using TouchScript.Gestures;

public class Pocket : MonoBehaviour {

	public List<GameObject> pieces = new List<GameObject> ();

	public void OnEnable() {
		gameObject.GetComponent<MeshRenderer> ().material.color = Color.clear;
		GetComponent<TapGesture> ().Tapped += TapHandler;
	}

	public void OnDisable() {
		GetComponent<TapGesture> ().Tapped -= TapHandler;
	}

	// acts just like a cell
	public void TapHandler(object sender, EventArgs e) {
		GameManager gm = GetComponentInParent<GameManager> ();
		if (gm.checkpoint()) {
			Board board = GetComponentInParent<Board> ();
			switch (board.turnPhase) {
			case Board.TurnPhase.Select:
				if (!isEmpty ()) {
					GameObject piece = pieces [0];
					//scanAll ();
					board.selectedPiece = null;
					board.resetHighlight ();
					if (piece != null) {
						if (piece.GetComponent<Piece> ().color == board.currentTurn) {
							board.selectedPiece = piece;
							piece.GetComponent<Piece> ().scanDrop ();
							board.resolveCheckmate ();
							board.turnPhase = Board.TurnPhase.Moving;
						}
					}
				}
				break;
			case Board.TurnPhase.Moving:
			// associate pockets with the player?
				board.turnPhase = Board.TurnPhase.Select;
				board.selectedPiece = null;
				board.resetHighlight ();
				break;
			default:
				break;
			}
		}
	}

	/*
	public void scanAll() {
		//piece.scan
		Board board = GetComponentInParent<Board> ();

		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				GameObject cell = board.get (i, j);
				cell.GetComponent<Cell> ().toggleHighlight ();
			}
		}
	}
	*/

	public void add(GameObject piece) {
		pieces.Add (piece);
	}

	public void remove(GameObject piece) {
		pieces.Remove (piece);
	}

	public bool isEmpty() {
		return pieces.Count == 0;
	}
}

