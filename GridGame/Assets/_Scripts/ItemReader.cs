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

            index++;

            int attackWidth = _stats[0];
            int attackHeight = _stats[1];

            _damageGrid = new int[attackWidth, attackHeight];

            for (int y = 0; y < attackHeight; y++, index++)
            {
                string[] gridValues = lines[index].Split(' ');
                if (gridValues.Length != attackWidth)
                    throw new FormatException($"Grid row {y} does not match expected width ({attackWidth}).");

                for (int x = 0; x < attackWidth; x++)
                {
                    if (int.TryParse(gridValues[x], out int gridValue))
                    {
                        _damageGrid[x, attackHeight - y - 1] = gridValue;
                    }
                    else
                    {
                        throw new FormatException($"Invalid integer found in grid at ({y}, {x}): {gridValues[x]}");
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
        Debug.Log(_filePath);
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