

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;

namespace DotsWithUI
{ 
  public class Artificial_Intelligence
  {
    private const int CELL_SIZE = 20;
    Random rnd = new Random();

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

    public void ClearPriority(List<PointWithPriority> points)
    {
      foreach (var elem in points)
      {
        elem.Priority = 0;
      }
    }
    
    public void SetPoint(Field field)
    {
      var points = SetPointWithPriority(field);
      ClearPriority(points);
      


    }
  }

  public class PointWithPriority
  {
    public int X;
    public int Y;
    public int Priority;
    
    public PointWithPriority(int xSet, int ySet)
    {
      this.X = xSet;
      this.Y = ySet;
    }
  }
}