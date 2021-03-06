using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;

namespace DotsWithUI
{ 
  public class Artificial_Intelligence
  {
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

    public int GetIndexOfNeighbor(List<PointWithPriority> neighbors, PointWithPriority point)
    {
      int index = neighbors.Select((item, i) => new { Item = item, Index = i })
        .First(x => x.Item.X == point.X & x.Item.Y == point.Y).Index;

      return index;
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
    
    public IEnumerable<PointWithPriority> GetNeighbors8(PointWithPriority p)
    {
      yield return new PointWithPriority(p.X - 1, p.Y);
      yield return new PointWithPriority(p.X - 1, p.Y - 1);
      yield return new PointWithPriority(p.X, p.Y - 1);
      yield return new PointWithPriority(p.X + 1, p.Y - 1);
      yield return new PointWithPriority(p.X + 1, p.Y);
      yield return new PointWithPriority(p.X + 1, p.Y + 1);
      yield return new PointWithPriority(p.X, p.Y + 1);
      yield return new PointWithPriority(p.X - 1, p.Y + 1);
    }

    /// <summary>
    /// Считаем количество соседей
    /// </summary>
    /// <param name="point"></param>
    /// <param name="field"></param>
    public void UpdateVerge(PointWithPriority point, List<PointWithPriority> points)
    {
      foreach (var elemN in GetNeighbors4(point))
      {
        foreach (var elemP in points)
        {
          if (elemN.X == elemP.X & elemN.Y == elemP.Y)
            point.Neighbors += 1;
        }
      }
    }

    /// <summary>
    /// Проверяем есть ли точка на границе (для приоритета)
    /// </summary>
    /// <param name="point"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    public bool isInBorder(PointWithPriority point, Field field)
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
    /// ход кудахтера
    /// </summary>
    /// <param name="field"></param>
    public void SetPoint(Field field, List<PointWithPriority> points)
    {
      ClearPriority(points);

      //добавляем соседей в лист 
      List<PointWithPriority> neighborsList = new List<PointWithPriority>();
      foreach (var point in points)
      {
        foreach (var elemP in GetNeighbors4(p: point))
        {
          neighborsList.Add(elemP);
        }
      }
      ClearPriority(neighborsList);

      foreach (var point in points)
      {
        //обновляет количество соседей
        UpdateVerge(point, points);

        //самый низкий приоритет уже использованным точкам
        foreach (var neighbor in GetNeighbors4(point))
        {
          if (points.Contains(new PointWithPriority(xSet: neighbor.X, ySet: neighbor.Y)))
          {
            var indexOfNeighbor = GetIndexOfNeighbor(neighborsList, neighbor);
            neighborsList[indexOfNeighbor].Priority = -1000000;
          }
            
        }
        
        //если кол-во соседей равно четырем выходим
        if (point.Neighbors == 4)
          continue;

        //если кол-во соседей меньше двух
        if (point.Neighbors < 2)
        {
          foreach (var neighbor in GetNeighbors4(point))
          {
            if (!points.Contains(new PointWithPriority(xSet: neighbor.X, ySet: neighbor.Y)))
            {
              var indexOfNeighbor = GetIndexOfNeighbor(neighborsList, neighbor);
              neighborsList[indexOfNeighbor].Priority += 2 - point.Neighbors;
            }
          }
        }
        else if (point.Neighbors == 2)
        {
          foreach (var neighbor in GetNeighbors4(point))
          {
            if (!points.Contains(new PointWithPriority(xSet: neighbor.X, ySet: neighbor.Y)))
            {
              var indexOfNeighbor = GetIndexOfNeighbor(neighborsList, neighbor);
              neighborsList[indexOfNeighbor].Priority -= 50;
            }
          }
        }
        else if (point.Neighbors == 3)
        {
          foreach (var neighbor in GetNeighbors4(point))
          {
            if (!points.Contains(new PointWithPriority(xSet: neighbor.X, ySet: neighbor.Y)))
            {
              var indexOfNeighbor = GetIndexOfNeighbor(neighborsList, neighbor);
              neighborsList[indexOfNeighbor].Priority = 10000;
            }
          }
        }
      }

      int x, y, p;
      x = y = p = -1;
      foreach (var elem in neighborsList)
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
      
      var pToSet = new Point((int) Math.Round(1f * x), (int) Math.Round(1f * y));
      if (field[pToSet] == CellState.Empty)
      {
        field.SetPoint(pToSet, CellState.Red);
        var addRedPoint = new PointWithPriority(pToSet.X, pToSet.Y);
        points.Add(addRedPoint);
      }
      else
      {
        foreach (var elem in GetNeighbors8(new PointWithPriority(xSet: x, ySet: y)))
        {
          if (field[new Point(elem.X, elem.Y)] == CellState.Empty)
          {
            var pToSetN = new Point((int) Math.Round(1f * elem.X), (int) Math.Round(1f * elem.Y)); 
            field.SetPoint(pToSetN, CellState.Red);
            var addRedPoint = new PointWithPriority(pToSetN.X, pToSetN.Y);
            points.Add(addRedPoint);
            return;
          }
        }
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