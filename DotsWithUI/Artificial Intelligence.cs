

using System;
using System.Drawing;
using System.Net;

namespace DotsWithUI
{ 
  public class Artificial_Intelligence
  {
    private const int CELL_SIZE = 20;
    Random rnd = new Random();
    
    
    public void SetPoint(Field field)
    {
      //var xCor = field.TakenAreas[1].Item2;
      var p = new Point((int)Math.Round(1f * rnd.Next(100, 500) / CELL_SIZE), 
        (int)Math.Round(1f * rnd.Next(100,500) / CELL_SIZE));
      if (field[p] == CellState.Empty)
      {
        field.SetPoint(p, CellState.Red);
      }
      else
      {
        SetPoint(field);
      }
    }
  }
}