using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Piece : MonoBehaviour {

	public string type;
	public int color; // 1 for white, -1 for black
	public bool pocketed = false;

	//stuff for pawns
	//I'm too lazy for inheritance
	public bool moved = false;
	public int moves = 0;
	public bool promoted = false;
	public GameObject targetedPawn;

	public GameObject originalCell;

	public void scan() {
		Board board = GetComponentInParent<Board> ();
		Cell cell = GetComponentInParent<Cell> ();
		board.resetHighlight ();
		switch (type) {
		case "Q":
			recursiveScanQ (board, cell.column, cell.row);
			break;
		case "R":
			recursiveScanOrtho (board, cell.column, cell.row);
			break;
		case "B":
			recursiveScanDiag (board, cell.column, cell.row);
			break;
		case "N":
			scanN (board, cell.column, cell.row);
			break;
		case "K":
			scanK (board, cell.column, cell.row);
			break;
		case "P":
			scanP (board, cell.column, cell.row);
			break;
		default:
			break;
		}
	}

	// bishop and rook scan at the same time
	public void recursiveScanQ(Board b, int c, int r) {
		// account for other pieces later
		recursiveScanDiag(b, c, r);
		recursiveScanOrtho(b, c, r);
	}

	// bishop ray scan
	public void recursiveScanDiag(Board b, int c, int r) {
		recursiveScan (b, c + 1, r + 1, 1, 1);
		recursiveScan (b, c - 1, r - 1, -1, -1);
		recursiveScan (b, c + 1, r - 1, 1, -1);
		recursiveScan (b, c - 1, r + 1, -1, 1);
	}

	// rook ray scan
	public void recursiveScanOrtho(Board b, int c, int r) {
		recursiveScan (b, c + 1, r, 1, 0);
		recursiveScan (b, c - 1, r, -1, 0);
		recursiveScan (b, c, r - 1, 0, -1);
		recursiveScan (b, c, r + 1, 0, 1);
	}

	// hardcoded for knight move scan
	public void scanN(Board b, int c, int r) {
		scanCell (b, c + 1, r + 2);
		scanCell (b, c - 1, r + 2);
		scanCell (b, c + 1, r - 2);
		scanCell (b, c - 1, r - 2);
		scanCell (b, c + 2, r + 1);
		scanCell (b, c - 2, r + 1);
		scanCell (b, c + 2, r - 1);
		scanCell (b, c - 2, r - 1);
	}

	// hardcoded for king move scan
	public void scanK(Board b, int c, int r) {
		scanCell (b, c, r + 1);
		scanCell (b, c, r - 1);
		scanCell (b, c + 1, r);
		scanCell (b, c + 1, r + 1);
		scanCell (b, c + 1, r - 1);
		scanCell (b, c - 1, r);
		scanCell (b, c - 1, r + 1);
		scanCell (b, c - 1, r - 1);
	}

	// hardcoded for pawn move scan
	public void scanP(Board b, int c, int r) {
		targetedPawn = null;
		scanCell (b, c, r + color);
		if (!moved) {
			scanCell (b, c, r + color * 2);
		}
		scanCellP (b, c + 1, r + color);
		scanCellP (b, c - 1, r + color);
		scanCellEP (b, c + 1, r);
		scanCellEP (b, c - 1, r);
	}

	// ray scan
	public void recursiveScan(Board b, int c, int r, int colInc, int rowInc) {
		if (scanCell (b, c, r)) {
			recursiveScan (b, c + colInc, r + rowInc, colInc, rowInc);
		}
	}

	// scan one cell, not for pawn capture
	public bool scanCell(Board b, int c, int r) {
		if (c <= 7 && c >= 0 && r <= 7 && r >= 0) {
			Cell cell = b.get(c, r).GetComponent<Cell>();
			if (cell.isEmpty ()) {
				cell.toggleHighlight ();
				return true;
			} else if (cell.piece.GetComponent<Piece> ().color != this.color && this.type != "P") {
				cell.toggleHighlight ();
				return false;
			}
		}
		return false;
	}

	// scan capture for pawn
	public bool scanCellP(Board b, int c, int r) {
		if (c <= 7 && c >= 0 && r <= 7 && r >= 0) {
			Cell cell = b.get(c, r).GetComponent<Cell>();
			if (cell.isEmpty ()) {
				return false;
			} else if (cell.piece.GetComponent<Piece> ().color != this.color) {
				cell.toggleHighlight ();
				return true;
			}
		}
		return false;
	}

	// scan for en passant
	public bool scanCellEP(Board b, int c, int r) {
		if (c <= 7 && c >= 0 && r <= 7 && r >= 0) {
			Cell cell = b.get(c, r).GetComponent<Cell>();
			if (cell.isEmpty ()) {
				return false;
			} else if (cell.piece.GetComponent<Piece> ().color != this.color &&
				cell.piece.GetComponent<Piece>().type == "P" &&
				cell.piece.GetComponent<Piece>().moves == 1 && 
				(cell.row == 3 || cell.row == 4) &&
				cell.piece.GetComponent<Piece>().moved) {
				Cell higherCell = b.get(c, r + color).GetComponent<Cell>();
				if (higherCell != null) {
					targetedPawn = cell.piece;
					higherCell.toggleHighlight ();
					return true;
				}
			}
		}
		return false;
	}

	// scan for drop
	public void scanDrop() {
		//piece.scan
		Board board = GetComponentInParent<Board> ();

		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				GameObject cellObject = board.get (i, j);
				Cell cell = cellObject.GetComponent<Cell> ();
				if (cell.piece == null) {
					if (j == 0 || j == 7) {
						if (type != "P") {
							cell.toggleHighlight ();
						}
					} else {
						cell.toggleHighlight ();
					}
				}
			}
		}
	}

	// A king doesn't scan recursively to move, but to check for attackers.
	// Technically all pieces can be "checked"; this is to prove that a checking piece can be taken to protect a king.

	public Piece scanCellK(Board b, int c, int r) {
		if (c <= 7 && c >= 0 && r <= 7 && r >= 0) {
			Cell cell = b.get(c, r).GetComponent<Cell>();
			if (cell.isEmpty ()) {
				return null;
			} else if (cell.piece.GetComponent<Piece> ().color != this.color) {
				return cell.piece.GetComponent<Piece>();
			}
		}
		return null;
	}

	// scan for knight moves
	public Piece[] scanNCheck(Board b, int c, int r) {

		Piece[] scannedKnightMoves = {
			scanCellK (b, c + 1, r + 2),
			scanCellK (b, c - 1, r + 2),
			scanCellK (b, c + 1, r - 2),
			scanCellK (b, c - 1, r - 2),
			scanCellK (b, c + 2, r + 1),
			scanCellK (b, c - 2, r + 1),
			scanCellK (b, c + 2, r - 1),
			scanCellK (b, c - 2, r - 1)
		};
		return scannedKnightMoves;
	}

	// scan for all cells around the piece
	public Piece[] scanKCheck(Board b, int c, int r) {
		Piece[] scannedContacts = {
			scanCellK (b, c, r + 1),
			scanCellK (b, c, r - 1),
			scanCellK (b, c + 1, r),
			scanCellK (b, c + 1, r + 1),
			scanCellK (b, c + 1, r - 1),
			scanCellK (b, c - 1, r),
			scanCellK (b, c - 1, r + 1),
			scanCellK (b, c - 1, r - 1)
		};
		return scannedContacts;
	}

	//scan recursively
	public Piece recursiveScanK(Board b, int c, int r, int colInc, int rowInc) {
		if (scanCell (b, c, r)) {
			return recursiveScanK (b, c + colInc, r + rowInc, colInc, rowInc);
		} else {
			return scanCellK (b, c, r);
		}
	}

	// bishop ray scan for check
	public Piece[] recursiveScanDiagK(Board b, int c, int r) {
		Piece[] scanned = {
			recursiveScanK (b, c + 1, r + 1, 1, 1),
			recursiveScanK (b, c - 1, r - 1, -1, -1),
			recursiveScanK (b, c + 1, r - 1, 1, -1),
			recursiveScanK (b, c - 1, r + 1, -1, 1)
		};
		return scanned;
	}

	// rook ray scan for check
	public Piece[] recursiveScanOrthoK(Board b, int c, int r) {
		Piece[] scanned = {
			recursiveScanK (b, c + 1, r, 1, 0),
			recursiveScanK (b, c - 1, r, -1, 0),
			recursiveScanK (b, c, r - 1, 0, -1),
			recursiveScanK (b, c, r + 1, 0, 1)
		};
		return scanned;
	}

	// list of pieces attacking this one
	public List<Piece> inCheck(int c, int r) {
		Board board = GetComponentInParent<Board> ();
		//Cell cell = GetComponentInParent<Cell> ();
		Piece[] knightChecks = scanNCheck (board, c, r);
		Piece[] orthoChecks = recursiveScanOrthoK(board, c, r);
		Piece[] diagChecks = recursiveScanDiagK(board, c, r);
		List<Piece> attackers = new List<Piece> ();

		Piece topLeft = scanCellK (board, c - 1, r + color);
		Piece topRight = scanCellK (board, c + 1, r + color);
		if (topLeft != null && topLeft.type == "P") {
			attackers.Add (topLeft);
		}
		if (topRight != null && topRight.type == "P") {
			attackers.Add (topRight);
		}
		foreach (Piece p in knightChecks) {
			if (p != null) {
				if (p.type == "N") {
					attackers.Add (p);			
				}
			}
		}
		foreach (Piece p in diagChecks) {
			if (p != null) {
				if (p.type == "B") {
					attackers.Add (p);
				}
			}
		}
		foreach (Piece p in orthoChecks) {
			if (p != null) {
				if (p.type == "R") {
					attackers.Add (p);
				}
			}
		}
		return attackers;

	}
	// actual check
	public List<Piece> inCheck() {
		//Board board = GetComponentInParent<Board> ();
		Cell cell = GetComponentInParent<Cell> ();
		return inCheck (cell.column, cell.row);
	}

	public Cell checkSafety(int c, int r) {
		Board board = GetComponentInParent<Board> ();
		if (c <= 7 && c >= 0 && r <= 7 && r >= 0) {
			if (inCheck (c, r).Count == 0) {
				return board.get (c, r).GetComponent<Cell>();
			}
		}
		return null;
	}

	// king checks for available moves for himself
	public bool canMove() {
		//Board board = GetComponentInParent<Board> ();
		Cell myCell = GetComponentInParent<Cell> ();
		int c = myCell.column;
		int r = myCell.row;
		Cell[] scannedContacts = {
			checkSafety (c, r + 1),
			checkSafety (c, r - 1),
			checkSafety (c + 1, r),
			checkSafety (c + 1, r + 1),
			checkSafety (c + 1, r - 1),
			checkSafety (c - 1, r),
			checkSafety (c - 1, r + 1),
			checkSafety (c - 1, r - 1)
		};

		foreach (Cell cell in scannedContacts) {
			if (cell != null) {
				if (cell.isEmpty ()) {
					return true;
				} else {
					Piece occupy = cell.piece.GetComponent<Piece> ();
					if (occupy.color != this.color) {
						return true;
					}
				}
			}
		}
		return false;
	}

	// this is the one used for the king
	// returns a list of attackers for the king that would cause checkmate in bughouse

	public List<Piece> bughouseCheck() {
		Board board = GetComponentInParent<Board> ();
		Cell cell = GetComponentInParent<Cell> ();
		Piece[] knightChecks = scanNCheck (board, cell.column, cell.row);
		Piece[] contactChecks = scanKCheck (board, cell.column, cell.row);
		Piece[] diagChecks = { contactChecks [3], contactChecks [4], contactChecks [6], contactChecks [7] };
		Piece[] orthoChecks = { contactChecks [0], contactChecks [1], contactChecks [2], contactChecks [5] };
		List<Piece> attackers = new List<Piece> ();

		foreach (Piece p in knightChecks) {
			if (p != null) {
				if (p.type == "N") {
					attackers.Add (p);			
				}
			}
		}
		foreach (Piece p in contactChecks) {
			if (p != null) {
				if (p.type == "Q") {
					attackers.Add (p);
				}
			}
		}
		foreach (Piece p in diagChecks) {
			if (p != null) {
				if (p.type == "B") {
					attackers.Add (p);
				}
			}
		}
		foreach (Piece p in orthoChecks) {
			if (p != null) {
				if (p.type == "R") {
					attackers.Add (p);
				}
			}
		}
		Piece topLeft = scanCellK (board, cell.column - 1, cell.row + color);
		Piece topRight = scanCellK (board, cell.column + 1, cell.row + color);
		if (topLeft != null && topLeft.type == "P") {
			attackers.Add (topLeft);
		}
		if (topRight != null && topRight.type == "P") {
			attackers.Add (topRight);
		}
		return attackers;
	}

	// move, capture and transfer pieces, along with checking for mate
	public void move(GameObject cell) {
		if (!pocketed) {
			moved = true;
			moves += 1;
			Debug.LogError ("move start");
			Cell myCell = GetComponentInParent<Cell> ();
			Cell newCell = cell.GetComponent<Cell> ();
			myCell.piece = null;

			GameObject capturedPiece = newCell.piece;
			newCell.piece = this.gameObject;
			this.gameObject.transform.SetParent (cell.transform);
			this.gameObject.transform.localPosition = new Vector3 (0, 0, -1);

			// the lazy no-image-transform way to do this
			if (type == "P" && (newCell.row == 7 || newCell.row == 0)) {
				this.promoted = true;
				type = "Q";
			}

			if (targetedPawn != null && capturedPiece == null && targetedPawn.GetComponentInParent<Cell>().column == cell.GetComponent<Cell>().column) {
				capturedPiece = targetedPawn;
				targetedPawn.gameObject.GetComponentInParent<Cell> ().piece = null;
			}
			if (capturedPiece != null) {
				//send to the right pocket
				Board board = GetComponentInParent<Board> ();
				board.transferPiece (capturedPiece);
			}
			targetedPawn = null;
		} else {
			pocketed = false;
			Pocket myPocket = GetComponentInParent<Pocket> ();
			myPocket.remove (this.gameObject);
			Cell newCell = cell.GetComponent<Cell> ();
			newCell.piece = this.gameObject;
			this.gameObject.transform.SetParent (cell.transform);
			this.gameObject.transform.localPosition = new Vector3 (0, 0, -1);
		}
	}

	/*
	public void drop(GameObject cell) {

	}
	*/
}
