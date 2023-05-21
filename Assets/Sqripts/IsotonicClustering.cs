using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IsotonicClustering : MonoBehaviour
{
    [SerializeField] private MatrixDrawer _sumMatrix;
    [SerializeField] private MatrixDrawer _normalMatrix;
    [SerializeField] private MatrixDrawer _distanceMatrix1;
    [SerializeField] private TMP_Text _critDistance1;
    [SerializeField] private TMP_Text _claster1;

    [SerializeField] private MatrixDrawer _isomorfMatrix;
    [SerializeField] private MatrixDrawer _distanceMatrix2;
    [SerializeField] private TMP_Text _critDistance2;
    [SerializeField] private TMP_Text _claster2;



    public void IsotonicSplit(float[,] matrix)
    {

        int numRows = matrix.GetLength(0);
        int numCols = matrix.GetLength(1);

        Debug.Log("Вхідна матриця:");
        PrintMatrix(matrix);

        float[] columnSums = new float[numCols];
        for (int col = 0; col < numCols; col++)
        {
            for (int row = 0; row < numRows; row++)
            {
                columnSums[col] += matrix[row, col];
            }
        }

        Debug.Log("Сумма матриці:");
        float[,] sumMatrix = new float[numRows + 1, numCols];
        for (int col = 0; col < numCols; col++)
        {
            for (int row = 0; row < numRows; row++)
            {
                sumMatrix[row, col] = matrix[row, col];
            }
        }
        for (int col = 0; col < numCols; col++)
        {
            sumMatrix[sumMatrix.GetLength(0) - 1, col] = columnSums[col];
        }
        _sumMatrix.SetInput(sumMatrix, false);
        // Нормалізуємо шкали за формулами для ізотонічної розбивки
        for (int col = 0; col < numCols; col++)
        {
            float sum = columnSums[col];
            for (int row = 0; row < numRows; row++)
            {
                matrix[row, col] /= sum;
            }
        }
        Debug.Log("Нормалізуємо матриця:");
        PrintMatrix(matrix);

        float[] rowSums = new float[numRows];
        for (int row = 0; row < numRows; row++)
        {
            float sum = 0;
            for (int col = 0; col < numCols; col++)
            {
                sum += matrix[row, col];
            }
            rowSums[row] = sum;
        }

        // Додати стовпчик Wij
        float[,] extendedMatrix = new float[numRows, numCols + 1];
        for (int row = 0; row < numRows; row++)
        {
            extendedMatrix[row, numCols] = rowSums[row];
            for (int col = 0; col < numCols; col++)
            {
                extendedMatrix[row, col] = matrix[row, col];
            }
        }

        Debug.Log("Матриця зі стовпчиком Wij:");
        PrintMatrix(extendedMatrix);
        _normalMatrix.SetInput(extendedMatrix, false);

        // Створити нову матрицю на основі стовпчика Wij
        int cols = extendedMatrix.GetLength(0) + 1;
        float[,] newMatrix = new float[extendedMatrix.GetLength(0), cols];
        for (int row = 0; row < numRows; row++)
        {
            float minValue = float.MaxValue;
            for (int rowCol = 0; rowCol < cols - 1; rowCol++)
            {
                float diff = Math.Abs(extendedMatrix[row, numCols] - extendedMatrix[rowCol, numCols]);
                newMatrix[row, rowCol] = diff;
                if (diff < minValue && diff != 0)
                {
                    minValue = diff;
                }
            }
            newMatrix[row, cols - 1] = minValue;
        }

        Debug.Log("Нова матриця:");
        PrintMatrix(newMatrix);
        _distanceMatrix1.SetInput(newMatrix, false);


        // Створити кластери за допомогою метода куль
        //критична відстань r
        float maxValue = float.MinValue;
        for (int row = 0; row < numRows; row++)
        {
            if (newMatrix[row, cols - 1] > maxValue)
            {
                maxValue = newMatrix[row, cols - 1];
            }
        }
        Debug.Log("maxValue: " + maxValue);
        _critDistance1.text = "Критична відстань = " + maxValue;
        float[,] endMatrix = new float[extendedMatrix.GetLength(0), extendedMatrix.GetLength(0) + 1];
        for (int row = 0; row < newMatrix.GetLength(0); row++)
        {
            int counter = 0;
            for (int col = 0; col < newMatrix.GetLength(1) - 1; col++)
            {
                if (newMatrix[row, col] < maxValue)
                {
                    endMatrix[row, col] = 1;
                    counter++;
                }
            }
            endMatrix[row, newMatrix.GetLength(1) - 1] = counter;
        }
        Debug.Log("endMatrix: ");
        PrintMatrix(endMatrix);

        Debug.Log("Кластери");
        int moRowIndex = FindMoRow(endMatrix);
        _claster1.text = "";
        while (moRowIndex >= 0)
        {
            DrawClaster(ref endMatrix, moRowIndex,_claster1);
            moRowIndex = FindMoRow(endMatrix);
        }

        Debug.Log("ізморфною розбивка");
        int colsIsomorf = extendedMatrix.GetLength(1);
        float[,] newMatrixIsomorf = new float[extendedMatrix.GetLength(0), colsIsomorf - 1];
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < colsIsomorf - 1; col++)
            {
                float multi = Math.Abs(extendedMatrix[row, col] / extendedMatrix[row, colsIsomorf - 1]);
                newMatrixIsomorf[row, col] = multi;
            }
        }

        Debug.Log("ізморф 1");
        PrintMatrix(newMatrixIsomorf);
        _isomorfMatrix.SetInput(newMatrixIsomorf, false);

        float[,] newMatrixIsomorfEnd = new float[newMatrixIsomorf.GetLength(0), newMatrixIsomorf.GetLength(0)];
        for (int rowPrimal = 0; rowPrimal < newMatrixIsomorf.GetLength(0); rowPrimal++)
        {
            for (int row = 0; row < newMatrixIsomorf.GetLength(0); row++)
            {
                float valueResult = 0;
                for (int col = 0; col < newMatrixIsomorf.GetLength(1); col++)
                {
                    valueResult += Mathf.Pow(newMatrixIsomorf[rowPrimal, col] - newMatrixIsomorf[row, col], 2);
                }
                newMatrixIsomorfEnd[rowPrimal, row] = Mathf.Sqrt(valueResult);
            }
        }
        Debug.Log("Final 1");
        PrintMatrix(newMatrixIsomorfEnd);
        _distanceMatrix2.SetInput(newMatrixIsomorfEnd, false);

        float[] minValuesInRow = new float[newMatrixIsomorfEnd.GetLength(0)];
        for (int row = 0; row < newMatrixIsomorfEnd.GetLength(0); row++)
        {
            float valueResult = float.MaxValue;
            for (int col = 0; col < newMatrixIsomorfEnd.GetLength(0); col++)
            {
                if (newMatrixIsomorfEnd[row, col] < valueResult && newMatrixIsomorfEnd[row, col] != 0)
                {
                    valueResult = newMatrixIsomorfEnd[row, col];
                }
            }
            minValuesInRow[row] = valueResult;
        }

        float maxValueIso = float.MinValue;
        for (int row = 0; row < minValuesInRow.Length; row++)
        {
            if (minValuesInRow[row] > maxValueIso)
            {
                maxValueIso = minValuesInRow[row];
            }
        }
        Debug.Log("maxValueIso " + maxValueIso);
        _critDistance2.text = "Критична відстань = " + maxValueIso;

        float[,] endMatrixIso = new float[newMatrixIsomorfEnd.GetLength(0), newMatrixIsomorfEnd.GetLength(1) + 1];
        for (int row = 0; row < newMatrixIsomorfEnd.GetLength(0); row++)
        {
            int counter = 0;
            for (int col = 0; col < newMatrixIsomorfEnd.GetLength(1); col++)
            {
                if (newMatrixIsomorfEnd[row, col] < maxValueIso)
                {
                    endMatrixIso[row, col] = 1;
                    counter++;
                }
            }
            endMatrixIso[row, newMatrix.GetLength(1) - 1] = counter;
        }
        Debug.Log("endMatrix: ");
        PrintMatrix(endMatrixIso);

        Debug.Log("Кластери");
        int moRowIndexIso = FindMoRow(endMatrixIso);
        _claster2.text = "";

        while (moRowIndexIso >= 0)
        {
            DrawClaster(ref endMatrixIso, moRowIndexIso, _claster2);
            moRowIndexIso = FindMoRow(endMatrixIso);
        }
    }

    private int FindMoRow(float[,] endMatrix)
    {
        int number = -1;
        float max = 0;

        for (int row = 0; row < endMatrix.GetLength(0); row++)
        {
            float valueInRow = endMatrix[row, endMatrix.GetLength(1) - 1];
            if (valueInRow >= max && valueInRow != 0)
            {
                number = row;
                max = endMatrix[row, endMatrix.GetLength(1) - 1];
            }
        }
        return number;
    }

    private void DrawClaster(ref float[,] endMatrix, int row, TMP_Text text)
    {
        string claster = "";
        for (int col = 0; col < endMatrix.GetLength(1) - 1; col++)
        {
            if (endMatrix[row, col] == 1)
            {
                int value = col + 1;
                claster += ("P" + value + " ");
                for (int rowOther = 0; rowOther < endMatrix.GetLength(0); rowOther++)
                {
                    endMatrix[rowOther, col] = 0;
                }
            }
        }
        Debug.Log(claster);
        text.text += "Кластер " + claster + " <br> ";
        RedrawMatrix(ref endMatrix);
    }

    private void RedrawMatrix(ref float[,] endMatrix)
    {
        for (int row = 0; row < endMatrix.GetLength(0); row++)
        {
            int counter = 0;
            for (int col = 0; col < endMatrix.GetLength(1) - 1; col++)
            {
                if (endMatrix[row, col] == 1)
                {
                    counter++;
                }
            }
            endMatrix[row, endMatrix.GetLength(1) - 1] = counter;
        }
    }

    public void PrintMatrix(float[,] matrix)
    {
        int numRows = matrix.GetLength(0);
        int numCols = matrix.GetLength(1);

        for (int row = 0; row < numRows; row++)
        {
            string rowTekst = "";
            for (int col = 0; col < numCols; col++)
            {
                rowTekst += (matrix[row, col] + " ");
            }
            Debug.Log(rowTekst);

        }
    }


    public void Start()
    {
        float[,] matrix = {
            { 30f, 0.6f, 2f,5f },
            { 33f, 0.6f, 2.5f,5f },
            { 50f, 1f, 2f,15f },
            { 45f, 0.8f, 2f,10f },
            { 20f, 0.2f, 1f,5f },
            { 25f, 0.6f, 1f,20f }
        };

        //IsotonicSplit(matrix);
    }
}