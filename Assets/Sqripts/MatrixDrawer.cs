using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Profiling;
using System;

public class MatrixDrawer : MonoBehaviour
{
    public float[,] Matrix;
    [SerializeField] private TMP_InputField _matrixElement;
    [SerializeField] private bool _isInputMatrix;
    [SerializeField] private GridLayoutGroup _gridLayoutGroup;
    [SerializeField] private List<TMP_InputField> _matrix = new List<TMP_InputField>();




    public void SetInput(float[,] matrix, bool isInputMatrix)
    {
        ClearMatrix();
        Matrix = matrix;
        int numRows = matrix.GetLength(0);
        int numCols = matrix.GetLength(1);
        _gridLayoutGroup.constraintCount = numCols;
        _isInputMatrix = isInputMatrix;
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                TMP_InputField inputTt = Instantiate(_matrixElement, _gridLayoutGroup.transform);
                _matrix.Add(inputTt);
                inputTt.text = matrix[row, col] + " ";
                if (isInputMatrix)
                {
                    inputTt.interactable = true;
                }
                else
                {
                    inputTt.interactable = false;
                }
            }
        }
    }

    public float[,] GetMatrix(int row, int col)
    {
        _gridLayoutGroup.constraintCount = col;
        Matrix = new float[row, col];
        int counter = 0;
        for (int numRows = 0; numRows < col; numRows++)
        {
            for (int numCols = 0; numCols < col; numCols++)
            {
                Matrix[numRows, numCols] = float.Parse(_matrix[counter].text);
                counter++;
            }
        }
        return Matrix;
    }

    public void ClearMatrix()
    {
        for(int i = 0; i< _matrix.Count; i++)
        {
            Destroy(_matrix[i].gameObject);
        }
        _matrix.Clear();
    }
}
