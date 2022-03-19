using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BLock: MonoBehaviour {
    private readonly double[] moveTime = new double[] { 0, 0, 0 };     // �̵� ���� �ӵ������� �ð�, [��, ��, ��]
    private readonly bool[] pushed = new bool[] { true, true };     // �ݺ��Է� ������ ���ۿ� bool, [���, ȸ��]
    public bool isPlace = false;

    private void Update() {
        if(isPlace) {
            if(transform.childCount == 0) {
                Destroy(gameObject);
            }
            return;
        }

        bool drop = Input.GetKey(KeyCode.Space);
        bool rotate = Input.GetKey(KeyCode.UpArrow);

        if(Time.time - moveTime[1] > (Input.GetKey(KeyCode.DownArrow) ? 1 / GameManager.speed / 12 : 1 / GameManager.speed)) {
            MoveDown();
        }
        else if(Input.GetKey(KeyCode.LeftArrow) && Time.time - moveTime[0] > 0.125) {
            MoveRight();
        }
        else if(Input.GetKey(KeyCode.RightArrow) && Time.time - moveTime[2] > 0.125) {
            MoveLeft();
        }

        else if(drop && !pushed[0]) {
            while(!isPlace) {
                MoveDown();
            }
        }
        else if(rotate && !pushed[1]) {
            Rotation();
        }

        // ���°� ����
        pushed[0] = drop;
        pushed[1] = rotate;
    }

    #region �� ���� �Լ�

    private void MoveDown() {
    }
    private void MoveLeft() {
    }
    private void MoveRight() {
    }
    private void Rotation() {
    }

    #endregion


    #region �� �ý��� �Լ�
    private bool VaildMove() {  // ���� ��ǥ�� �̵��ص� �̻��� ������ üũ�ϴ� �Լ�
        foreach(Transform child in transform) {
            int x = Mathf.RoundToInt(child.position.x);
            int y = Mathf.RoundToInt(child.position.y);
            int z = Mathf.RoundToInt(child.position.z);
            if(x < 0 || x >= GameManager.width || z < 0 || z >= GameManager.width) {
                return false;
            }
            if(y < 0 || y >= GameManager.height) {
                return false;
            }
            if(GameManager.board[x, y, z] != null) {
                return false;
            }
        }
        return true;
    }

    private void PlaceBlock() {     // �ٴڿ� ���� ���� ���忡 ��ġ�ϴ� �Լ�
        foreach(Transform child in transform) {
            int x = Mathf.RoundToInt(child.position.x);
            int y = Mathf.RoundToInt(child.position.y);
            int z = Mathf.RoundToInt(child.position.z);
            GameManager.board[x, y, z] = child;
            if(y > 22) {
                GameManager.GameOver();
            }
        }
    }

    /*
    private void ClearLine() {
        foreach(Transform child in transform) {
            int y = Mathf.RoundToInt(child.position.y);
            if(!HasEmpty(y)) {
                DeleteLine(y);
                RowDown(y);
            }
        }
    }
    private bool HasEmpty(int y) {
        for(int x = 0; x < GameManager.width; x++) {
            if(GameManager.board[x, y] == null) {
                return true;
            }
        }
        return false;
    }

    private void DeleteLine(int y) {
        GameManager.score += 10 * GameManager.width;
        for(int x = 0; x < GameManager.width; x++) {
            Destroy(GameManager.board[x, y].gameObject);
            GameManager.board[x, y] = null;
        }
    }
    private void RowDown(int y) {
        for(int d_y = y; d_y < GameManager.height; d_y++) {
            for(int x = 0; x < GameManager.width; x++) {
                if(GameManager.board[x, d_y] != null) {
                    GameManager.board[x, d_y - 1] = GameManager.board[x, d_y];
                    GameManager.board[x, d_y] = null;
                    GameManager.board[x, d_y - 1].transform.position += new Vector3(0, -1, 0);
                }
            }
        }
    } */
    #endregion
}
