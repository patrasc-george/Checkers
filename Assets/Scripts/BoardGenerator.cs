using System;
using System.Collections;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardGenerator : MonoBehaviour
{
    public GameObject whiteSquare;
    public GameObject blackSquare;
    public GameObject pawnWhite;
    public GameObject pawnBlack;
    public GameObject rookWhite;
    public GameObject rookBlack;
    public GameObject knightWhite;
    public GameObject knightBlack;
    public GameObject bishopWhite;
    public GameObject bishopBlack;
    public GameObject queenWhite;
    public GameObject queenBlack;
    public GameObject kingWhite;
    public GameObject kingBlack;
    public Material selectionMaterial;
    public TextMeshProUGUI blackText;
    public TextMeshProUGUI whiteText;
    public GameObject moveEffectPrefab;
    [SerializeField]
    private AudioClip selectSoundClip;
    private int blackScore = 16;
    private int whiteScore = 16;
    private Piece selectedPiece;
    private Tile[,] tiles = new Tile[8, 8];

    public class Tile
    {
        public GameObject gameObject;
        public Piece piece;
        public int x, y;

        public Tile(GameObject gameObject, int x, int y)
        {
            this.gameObject = gameObject;
            this.piece = null;
            this.x = x;
            this.y = y;
        }
    }

    public abstract class Piece
    {
        public GameObject gameObject;
        public Material color;
        public int x;
        public int y;

        public Piece(GameObject gameObject, Material color, int x, int y)
        {
            this.gameObject = gameObject;
            this.color = color;
            this.x = x;
            this.y = y;
        }

        public abstract void Move(int x, int y);
    }
    public class Pawn : Piece
    {
        public Pawn(GameObject gameObject, Material color, int x, int y)
            : base(gameObject, color, x, y) { }

        public override void Move(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Rook : Piece
    {
        public Rook(GameObject gameObject, Material color, int x, int y)
            : base(gameObject, color, x, y) { }

        public override void Move(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Knight : Piece
    {
        public Knight(GameObject gameObject, Material color, int x, int y)
            : base(gameObject, color, x, y) { }

        public override void Move(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Bishop : Piece
    {
        public Bishop(GameObject gameObject, Material color, int x, int y)
            : base(gameObject, color, x, y) { }

        public override void Move(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Queen : Piece
    {
        public Queen(GameObject gameObject, Material color, int x, int y)
            : base(gameObject, color, x, y) { }

        public override void Move(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class King : Piece
    {
        public King(GameObject gameObject, Material color, int x, int y)
            : base(gameObject, color, x, y) { }

        public override void Move(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    void Start()
    {
        GenerateBoard();
    }

    void GenerateBoard()
    {
        MeshRenderer meshRenderer = whiteSquare.GetComponent<MeshRenderer>();
        float tileSize = meshRenderer.bounds.size.x;

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                CreateTile(x, y, tileSize);
            }
        }
        PlacePieces(tileSize);
        UpdateScoreDisplay();
    }

    void CreateTile(int x, int y, float tileSize)
    {
        GameObject tilePrefab = (x + y) % 2 == 0 ? whiteSquare : blackSquare;
        GameObject tileGameObject = Instantiate(tilePrefab, new Vector3(x * tileSize, 0, y * tileSize), Quaternion.identity);
        tileGameObject.transform.parent = this.transform;
        Tile tile = new Tile(tileGameObject, x, y);
        tiles[x, y] = tile;
    }

    void PlacePieces(float tileSize)
    {
        for (int x = 0; x < 8; x++)
        {
            CreatePiece(x, 1, pawnBlack, tileSize, typeof(Pawn));
            CreatePiece(x, 6, pawnWhite, tileSize, typeof(Pawn));
        }

        CreatePiece(0, 0, rookBlack, tileSize, typeof(Rook));
        CreatePiece(7, 0, rookBlack, tileSize, typeof(Rook));
        CreatePiece(0, 7, rookWhite, tileSize, typeof(Rook));
        CreatePiece(7, 7, rookWhite, tileSize, typeof(Rook));

        CreatePiece(1, 0, knightBlack, tileSize, typeof(Knight));
        CreatePiece(6, 0, knightBlack, tileSize, typeof(Knight));
        CreatePiece(1, 7, knightWhite, tileSize, typeof(Knight));
        CreatePiece(6, 7, knightWhite, tileSize, typeof(Knight));

        CreatePiece(2, 0, bishopBlack, tileSize, typeof(Bishop));
        CreatePiece(5, 0, bishopBlack, tileSize, typeof(Bishop));
        CreatePiece(2, 7, bishopWhite, tileSize, typeof(Bishop));
        CreatePiece(5, 7, bishopWhite, tileSize, typeof(Bishop));

        CreatePiece(3, 0, queenBlack, tileSize, typeof(Queen));
        CreatePiece(3, 7, queenWhite, tileSize, typeof(Queen));

        CreatePiece(4, 0, kingBlack, tileSize, typeof(King));
        CreatePiece(4, 7, kingWhite, tileSize, typeof(King));
    }

    void CreatePiece(int x, int y, GameObject piecePrefab, float tileSize, System.Type pieceType)
    {
        GameObject pieceGameObject = Instantiate(piecePrefab, new Vector3(x * tileSize, 0.1f, y * tileSize), Quaternion.identity);
        pieceGameObject.transform.parent = tiles[x, y].gameObject.transform;

        Piece piece = (Piece)System.Activator.CreateInstance(pieceType, pieceGameObject, pieceGameObject.GetComponent<Renderer>().material, x, y);
        tiles[x, y].piece = piece;
    }

    void UpdateScoreDisplay()
    {
        whiteText.text = "White: " + whiteScore.ToString();
        blackText.text = "Black: " + blackScore.ToString();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ProcessClick();
        }
    }

    void ProcessClick()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            HandleInteraction(hit.collider.gameObject);
        }
    }

    void HandleInteraction(GameObject hitObject)
    {
        if (hitObject.CompareTag("Piece"))
        {
            HandlePieceSelection(hitObject);
        }
        if (hitObject.CompareTag("Square"))
        {
            HandlePieceMove(hitObject);
        }
    }

    void HandlePieceSelection(GameObject hitObject)
    {
        Piece hitPiece = FindPiece(hitObject);
        if (hitPiece != null)
        {
            if (selectedPiece != null)
            {
                selectedPiece.gameObject.GetComponent<Renderer>().material = selectedPiece.color;
                if (selectedPiece == hitPiece)
                {
                    selectedPiece = null;
                    return;
                }
            }

            selectedPiece = hitPiece;
            selectedPiece.gameObject.GetComponent<Renderer>().material = selectionMaterial;

            Instantiate(moveEffectPrefab, selectedPiece.gameObject.transform.position, Quaternion.identity);
            AudioSource audioSource = selectedPiece.gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = selectedPiece.gameObject.AddComponent<AudioSource>();
                audioSource.clip = selectSoundClip;
                audioSource.playOnAwake = false;
            }
            audioSource.Play();
        }
    }

    public Piece FindPiece(GameObject hitObject)
    {
        for (int y = 0; y < tiles.GetLength(0); y++)
        {
            for (int x = 0; x < tiles.GetLength(1); x++)
            {
                Tile tile = tiles[x, y];
                if (tile.piece != null && tile.piece.gameObject == hitObject)
                {
                    return tile.piece;
                }
            }
        }
        return null;
    }

    void HandlePieceMove(GameObject hitObject)
    {
        Tile tile = FindTile(hitObject);
        Vector3 direction = tile.gameObject.transform.position - selectedPiece.gameObject.transform.position;
        PerformMove(selectedPiece, tile, direction);
    }

    public Tile FindTile(GameObject hitObject)
    {
        for (int y = 0; y < tiles.GetLength(0); y++)
        {
            for (int x = 0; x < tiles.GetLength(1); x++)
            {
                Tile tile = tiles[x, y];
                if (tiles[x, y].gameObject == hitObject)
                {
                    return tile;
                }
            }
        }
        return null;
    }

    void PerformMove(Piece piece, Tile tile, Vector3 direction)
    {
        int moveDistance = 2;

        if (tile != null && tile.piece == null && (Math.Abs(direction.z) >= 0 && Math.Abs(direction.z) <= moveDistance) && (Math.Abs(direction.x) >= 0 || Math.Abs(direction.x) <= moveDistance))
        {
            Tile currentTile = tiles[piece.x, piece.y];
            if (currentTile != null)
            {
                currentTile.piece = null;
            }

            piece.Move(tile.x, tile.y);

            piece.gameObject.transform.position = tile.gameObject.transform.position;
            piece.gameObject.transform.SetParent(tile.gameObject.transform);
            tile.piece = piece;
            piece.gameObject.GetComponent<Renderer>().material = piece.color;
        }
    }
}
