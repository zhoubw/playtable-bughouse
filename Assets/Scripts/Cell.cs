using UnityEngine;
using System;
using TouchScript.Gestures;

public class Cell : MonoBehaviour {

	public GameObject piece;
	public bool highlighted = false;
	public int column;
	public int row;

	public void OnEnable() {
		gameObject.GetComponent<MeshRenderer> ().material.color = Color.clear;
		GetComponent<TapGesture> ().Tapped += TapHandler;
	}

	public void OnDisable() {
		GetComponent<TapGesture> ().Tapped -= TapHandler;
	}

	public void TapHandler(object sender, EventArgs e) {
		GameManager gm = GetComponentInParent<GameManager> ();
		if (gm.checkpoint()) {
			Debug.LogError (column + ", " + row);
			Board board = GetComponentInParent<Board> ();

			switch (board.turnPhase) {
			case Board.TurnPhase.Select:
			//scanAll ();
				board.selectedPiece = null;
				board.resetHighlight ();
				if (piece != null) {
					if (piece.GetComponent<Piece> ().color == board.currentTurn) {
						board.selectedPiece = piece;
						piece.GetComponent<Piece> ().scan ();
						board.turnPhase = Board.TurnPhase.Moving;
					}
				}
				break;
			case Board.TurnPhase.Moving:
				if (highlighted && board.selectedPiece != null) {
					board.selectedPiece.GetComponent<Piece> ().move (this.gameObject);
					board.resolveCheckmate ();
					board.passTurn ();
					//board.currentTurn *= -1; // pass turn
					board.turnPhase = Board.TurnPhase.Select;
					board.resetHighlight ();
				} else if (piece != null) {
					if (piece.GetComponent<Piece> ().color == board.currentTurn) {
						board.resetHighlight ();
						board.selectedPiece = piece;
						piece.GetComponent<Piece> ().scan ();
					}
				} else {
					board.turnPhase = Board.TurnPhase.Select;
					board.selectedPiece = null;
					board.resetHighlight ();
				}
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

	public void toggleHighlight() {
		if (!highlighted) {
			gameObject.GetComponent<MeshRenderer> ().material.color = Color.green;
		} else {
			gameObject.GetComponent<MeshRenderer> ().material.color = Color.clear;
		}
		highlighted = !highlighted;
	}

	public bool isEmpty() {
		return piece == null;
	}
}
