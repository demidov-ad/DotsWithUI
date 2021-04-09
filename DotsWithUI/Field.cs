using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DotsWithUI
{
    /// <summary>
    /// поле игры 
    /// </summary>
    public class Field
    {
        public const int SIZE = 100;
        public CellState[,] cells = new CellState[SIZE, SIZE];
        public List<Tuple<CellState, HashSet<Point>>> TakenAreas = new List<Tuple<CellState, HashSet<Point>>>();

        
        /// <summary>
        /// получение cell у point
        /// </summary>
        /// <param name="p"></param>
        public CellState this[Point p]
        {
            get
            {
                if (p.X < 0 || p.X >= SIZE || p.Y < 0 || p.Y >= SIZE)
                    return CellState.OutOfField;
                return cells[p.X, p.Y];
            }

            set { cells[p.X, p.Y] = value; }
        }

        
        /// <summary>
        /// интерфейс для получения 4х соседей
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public IEnumerable<Point> GetNeighbors4(Point p)
        {
            yield return new Point(p.X - 1, p.Y);
            yield return new Point(p.X, p.Y - 1);
            yield return new Point(p.X + 1, p.Y);
            yield return new Point(p.X, p.Y + 1);
        }

        
        /// <summary>
        /// интерфейс для получения 8ми соседей
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public IEnumerable<Point> GetNeighbors8(Point p)
        {
            yield return new Point(p.X - 1, p.Y);
            yield return new Point(p.X - 1, p.Y - 1);
            yield return new Point(p.X, p.Y - 1);
            yield return new Point(p.X + 1, p.Y - 1);
            yield return new Point(p.X + 1, p.Y);
            yield return new Point(p.X + 1, p.Y + 1);
            yield return new Point(p.X, p.Y + 1);
            yield return new Point(p.X - 1, p.Y + 1);
        }

        /// <summary>
        /// ищем замкнутые области
        /// </summary>
        /// <param name="lastPoint"></param>
        /// <returns></returns>
        private IEnumerable<HashSet<Point>> GetClosedArea(Point lastPoint)
        {
            var myState = this[lastPoint];
            //перебираем пустые точки и пытаемся пробиться из них к краю поля
            foreach (var n in GetNeighbors4(lastPoint))
                if (this[n] != myState)
                {
                    //ищем замкнутую область
                    var list = GetClosedArea(n, myState);
                    if (list != null)//нашли?
                        yield return list;//возвращаем занятые точки
                }
        }

        /// <summary>
        /// заливаем область, начиная с точки затравки, если не вышли к краю поля - возвращаем набор залитых точек
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="myState"></param>
        /// <returns></returns>
        private HashSet<Point> GetClosedArea(Point pos, CellState myState)
        {
            //ищем рекурсивным алгоритмом заливки
            var stack = new Stack<Point>();
            var visited = new HashSet<Point>();
            stack.Push(pos);
            visited.Add(pos);
            while (stack.Count > 0)
            {
                var p = stack.Pop();
                var state = this[p];
                //если вышли за пределы поля - значит область не замкнута
                if (state == CellState.OutOfField)
                    return null;
                //рекурсивно перебираем соседей
                foreach (var n in GetNeighbors4(p))
                    if (this[n] != myState)
                        if (!visited.Contains(n))
                        {
                            visited.Add(n);
                            stack.Push(n);
                        }
            }

            return visited;
        }

        /// <summary>
        /// метод для игры на двоих, проверка работы филда 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static CellState Inverse(CellState state)
        {
            return state == CellState.Blue ? CellState.Red : CellState.Blue;
        }
        
        /// <summary>
        /// добавление точки на поле
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="state"></param>
        public void SetPoint(Point pos, CellState state)
        {
            this[pos] = state;

            foreach (var taken in GetClosedArea(pos))
                TakenAreas.Add(new Tuple<CellState, HashSet<Point>>(state, taken));
        }

        /// <summary>
        /// получаем контур залитой области
        /// </summary>
        /// <param name="taken"></param>
        /// <returns></returns>
        public IEnumerable<Point> GetContour(HashSet<Point> taken)
        {
            //ищем любую точку из контура
            var start = new Point();
            foreach (var p in taken)
            foreach (var n in GetNeighbors4(p))
                if (!taken.Contains(n))
                {
                    start = n;
                    goto next;
                }
            
            next:

            //делаем обход по часовой стрелке вдоль области
            yield return start;
            var pp = GetNext(start, taken);
            while (pp != start)
            {
                yield return pp;
                pp = GetNext(pp, taken);
            }
        }

        Point GetNext(Point p, HashSet<Point> taken)
        {
            var temp = GetNeighbors8(p).ToList();
            var list = new List<Point>(temp);
            list.AddRange(temp);
            for (int i = 0; i < list.Count - 1; i++)
                if (!taken.Contains(list[i]) && taken.Contains(list[i + 1]))
                    return list[i];

            throw new Exception("hmm...");
        }
    }

    public enum CellState
    {
        Empty, Red, Blue, OutOfField
    }
}
