using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ItemReader : MonoBehaviour
{
    private string _filePath;
    private List<int> _stats;
    private int[,] _damageGrid;

    public void PrepareReader(string filePath)
    {
        _filePath = filePath;
        _stats = new List<int>();
    }

    public bool ReadItemFile()
    {
        try
        {
            string[] lines = File.ReadAllLines(_filePath);

            int index = 0;
            while (index < lines.Length && !string.IsNullOrWhiteSpace(lines[index]))
            {
                if (int.TryParse(lines[index], out int value))
                {
                    _stats.Add(value);
                }
                else
                {
                    throw new FormatException($"Invalid integer found in stats section: {lines[index]}");
                }
                index++;
            }

            if (_stats.Count != 6)
                throw new FormatException("Expected exactly 6 integer values before the grid.");

            index++; // Skip blank line

            int attackWidth = _stats[0];
            int attackHeight = _stats[1];

            _damageGrid = new int[attackHeight, attackWidth];

            for (int i = 0; i < attackHeight; i++, index++)
            {
                string[] gridValues = lines[index].Split(' ');
                if (gridValues.Length != attackWidth)
                    throw new FormatException($"Grid row {i} does not match expected width ({attackWidth}).");

                for (int j = 0; j < attackWidth; j++)
                {
                    if (int.TryParse(gridValues[j], out int gridValue))
                    {
                        _damageGrid[i, j] = gridValue;
                    }
                    else
                    {
                        throw new FormatException($"Invalid integer found in grid at ({i}, {j}): {gridValues[j]}");
                    }
                }
            }

            return ValidateGrid();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading item file: {ex.Message}");
            return false;
        }
    }

    private bool ValidateGrid()
    {
        if (_damageGrid[_stats[2] - 1, _stats[3] -1] != -1)
        {
            Console.WriteLine("Invalid item file: The holder position (-1) was not found.");
            return false;
        }

        return true;
    }

    public List<int> GetStats()
    {
        return new List<int>(_stats);
    }

    public int[,] GetDamageGrid()
    {
        return _damageGrid;
    }
}