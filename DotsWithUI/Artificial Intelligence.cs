

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;

namespace DotsWithUI
{ 
  public class Artificial_Intelligence
  {

    /// <summary>
    /// Считываем из поля занятые точки и добавляем их к новому классу
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    public List<PointWithPriority> SetPointWithPriority(Field field)
    {
      List<PointWithPriority> points = new List<PointWithPriority>();
      var xyCor = field.TakenAreas;
      foreach (var elem in xyCor)
      {
        foreach (var childElem in elem.Item2)
        {
          var pwp = new PointWithPriority(childElem.X, childElem.Y);
          points.Add(pwp);
        }
      }
      return points;
    }

    
    /// <summary>
    /// Ставим приоритет на ноль
    /// </summary>
    /// <param name="points"></param>
    public void ClearPriority(List<PointWithPriority> points)
    {
      foreach (var elem in points)
      {
        elem.Priority = 0;
      }
    }
    
    /// <summary>
    /// Интерфейс для получения соседей
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public IEnumerable<PointWithPriority> GetNeighbors4(PointWithPriority p)
    {
      yield return new PointWithPriority(p.X - 1, p.Y);
      yield return new PointWithPriority(p.X, p.Y - 1);
      yield return new PointWithPriority(p.X + 1, p.Y);
      yield return new PointWithPriority(p.X, p.Y + 1);
    }

    /// <summary>
    /// Считаем количество соседей
    /// </summary>
    /// <param name="point"></param>
    /// <param name="field"></param>
    public void UpdateVerge(PointWithPriority point, Field field)
    {
      foreach (var nPoint in GetNeighbors4(point))
      {
        foreach (var takenAreas in field.TakenAreas)
        {
          foreach (var tA in takenAreas.Item2)
          {
            if (nPoint.X == tA.X & nPoint.X == tA.Y)
              point.Neighbors += 1;
          }
        }
      }
    }

    /// <summary>
    /// Проверяем есть ли точка на поле (для приоритета)
    /// </summary>
    /// <param name="point"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    public bool isInField(PointWithPriority point, Field field)
    {
      foreach (var elemArea in field.TakenAreas)
      {
        foreach (var area in elemArea.Item2)
        {
          if (point.X == area.X & point.Y == area.Y)
            return true;
        }
      }

      return false;
    }
    
    /// <summary>
    /// Ход кудахтера
    /// </summary>
    /// <param name="field"></param>
    public void SetPoint(Field field)
    {
      var points = SetPointWithPriority(field);
      ClearPriority(points);
      foreach (var point in points)
      {
        UpdateVerge(point, field);
        
        //самый низкий приоритет уже использованным точкам
        foreach (var neighbor in GetNeighbors4(point))
        {
          if (isInField(neighbor, field))
            neighbor.Priority = -1000;
        }
        
        //если кол-во соседей равно четырем выходим
        if (point.Neighbors == 4)
          continue;

        //если кол-во соседей меньше двух
        if (point.Neighbors < 2)
        {
          foreach (var neighbor in GetNeighbors4(point))
          {
            if (!isInField(neighbor, field))
            {
              point.Priority += 2 - point.Neighbors;
            }
          }
        }
        else if (point.Neighbors == 2)
        {
          foreach (var neighbor in GetNeighbors4(point))
          {
            if (!isInField(neighbor, field))
            {
              point.Priority -= 50;
            }
          }
        }
        else if (point.Neighbors == 3)
        {
          foreach (var neighbor in GetNeighbors4(point))
          {
            if (!isInField(neighbor, field))
            {
              point.Priority += 1000;
            }
          }
        }
      }

      int x, y, p;
      x = y = p = -1;
      foreach (var elem in points)
      {
        if (x == -1)
        {
          x = elem.X;
          y = elem.Y;
          p = elem.Priority;
        }

        if (elem.Priority > p)
        {
          x = elem.X;
          y = elem.Y;
          p = elem.Priority;
        }
      }

      var pToSet = new Point((int) Math.Round(1f * x / 20), (int) Math.Round(1f * y / 20));
      if (field[pToSet] == CellState.Empty)
      {
        field.SetPoint(pToSet, CellState.Red);
      }
    }
    
    
    
  }

  public class PointWithPriority
  {
    public int X;
    public int Y;
    public int Priority;
    public int Neighbors;
    
    public PointWithPriority(int xSet, int ySet)
    {
      this.X = xSet;
      this.Y = ySet;
    }
  }
}