using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Size : MonoBehaviour
{
    [SerializeField] private TMP_InputField _sizeRow;
    [SerializeField] private TMP_InputField _sizeCol;
    [SerializeField] private MatrixDrawer _matrixDrawer;
    [SerializeField] private IsotonicClustering _isotonicClustering;

    private void Start()
    {
        ReCreateMatrix();
        float[,] matrix = {
            { 30f, 0.6f, 2f,5f },
            { 33f, 0.6f, 2.5f,5f },
            { 50f, 1f, 2f,15f },
            { 45f, 0.8f, 2f,10f },
            { 20f, 0.2f, 1f,5f },
            { 25f, 0.6f, 1f,20f }
        };
        _matrixDrawer.SetInput(matrix, true);
        float[,] extendedMatrix = new float[_matrixDrawer.Matrix.GetLength(0), _matrixDrawer.Matrix.GetLength(1)];
        for (int row = 0; row < _matrixDrawer.Matrix.GetLength(0); row++)
        {
            for (int col = 0; col < _matrixDrawer.Matrix.GetLength(1); col++)
            {
                extendedMatrix[row, col] = _matrixDrawer.Matrix[row, col];
            }
        }
        _isotonicClustering.IsotonicSplit(extendedMatrix);
    }

    public void DrawNewMAtrix()
    {
        _matrixDrawer.SetInput(_matrixDrawer.GetMatrix(Int32.Parse(_sizeRow.text), Int32.Parse(_sizeCol.text)),true);
        float[,] extendedMatrix = new float[_matrixDrawer.Matrix.GetLength(0), _matrixDrawer.Matrix.GetLength(1)];
        for (int row = 0; row < _matrixDrawer.Matrix.GetLength(0); row++)
        {
            for (int col = 0; col < _matrixDrawer.Matrix.GetLength(1); col++)
            {
                extendedMatrix[row, col] = _matrixDrawer.Matrix[row, col];
            }
        }
        _isotonicClustering.IsotonicSplit(extendedMatrix);
    }

    public void ReCreateMatrix()
    {
        if (_matrixDrawer.Matrix == null)
        {
            _matrixDrawer.Matrix = new float[Int32.Parse(_sizeRow.text), Int32.Parse(_sizeCol.text)];
        }

        float[,] matrix = new float[Int32.Parse(_sizeRow.text), Int32.Parse(_sizeCol.text)];
        for (int row = 0; row < _matrixDrawer.Matrix.GetLength(0) && row < matrix.GetLength(0); row++)
        {

            for (int col = 0; col < _matrixDrawer.Matrix.GetLength(1) && col < matrix.GetLength(1); col++)
            {
                matrix[row, col] = _matrixDrawer.Matrix[row, col];
            }
        }
        _matrixDrawer.SetInput(matrix, true);
    }
}
