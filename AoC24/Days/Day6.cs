using BenchmarkDotNet.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days;

public class Day6 : Day<(int VisitedCount, HashSet<(int I, int J)> Visited), int>
{
    private (int I, int J) _startingPosition;
    private Direction _startingDirection;
    private char[][] _gridMap = [];

    public override async Task Setup()
    {
        _gridMap = (await AdventOfCodeInput.For(2024, 6, SessionId))
            .Select(line => line.ToCharArray())
            .ToArray();

        for (var i = 0; i < _gridMap.Length; i++)
        {
            _startingPosition.I = i;
            _startingPosition.J = _gridMap[i]
                .Select((c, idx) => (c, idx))
                .FirstOrDefault(t => t.c is '^' or '>' or 'v' or '<')
                .idx;

            if (_startingPosition.J != default)
                break;
        }

        _startingDirection = _gridMap[_startingPosition.I][_startingPosition.J] switch
        {
            '^' => Direction.Up,
            'v' => Direction.Down,
            '<' => Direction.Left,
            '>' => Direction.Right,
            _ => throw new InvalidOperationException()
        };
    }

    private bool InBounds(int i, int j)
    {
        return i >= 0 && i < _gridMap.Length && j >= 0 && j < _gridMap[i].Length;
    }

    private static (int I, int J) GetDirectionDelta(Direction dir)
    {
        return dir switch
        {
            Direction.Up => (-1, 0),
            Direction.Down => (1, 0),
            Direction.Left => (0, -1),
            Direction.Right => (0, 1),
            _ => throw new InvalidOperationException()
        };
    }

    private static Direction TurnRight(Direction dir)
    {
        return dir switch
        {
            Direction.Up => Direction.Right,
            Direction.Right => Direction.Down,
            Direction.Down => Direction.Left,
            Direction.Left => Direction.Up,
            _ => throw new InvalidOperationException()
        };
    }

    [Benchmark]
    public override (int VisitedCount, HashSet<(int I, int J)> Visited) Solve1()
    {
        var visited = new HashSet<(int I, int J)>();
        var pathEnumer = new PathEnumerator(_gridMap, _startingPosition, _startingDirection);

        while (pathEnumer.MoveNext())
            visited.Add(pathEnumer.Current.Pos);

        return (visited.Count, visited);
    }

    [Benchmark]
    public override int Solve2()
    {
        var obstructionCount = 0;
        for (var y = 0; y < _gridMap.Length; y++)
        {
            for (var x = 0; x < _gridMap[y].Length; x++)
            {
                if (_gridMap[y][x] == (char)GridSquareType.Obstacle || (y, x) == _startingPosition)
                    continue;

                var originalChar = _gridMap[y][x];
                _gridMap[y][x] = (char)GridSquareType.Obstacle;

                if (IsObstructionLoop(_gridMap, _startingPosition, _startingDirection))
                    obstructionCount++;

                _gridMap[y][x] = originalChar;
            }
        }
        return obstructionCount;
    }

    private static bool IsObstructionLoop(char[][] map, (int I, int J) guard, Direction startingDirection)
    {
        var dir = startingDirection;
        var positions = new HashSet<((int I, int J) Pos, Direction Dir)> { (guard, dir) };

        while (true)
        {
            var (di, dj) = GetDirectionDelta(dir);
            (int I, int J) newPos = (guard.I + di, guard.J + dj);

            if (newPos.I < 0 || newPos.I >= map.Length || newPos.J < 0 || newPos.J >= map[0].Length)
            {
                return false;
            }
            else if (map[newPos.I][newPos.J] == (char)GridSquareType.Obstacle)
            {
                dir = TurnRight(dir);
            }
            else
            {
                if (!positions.Add((newPos, dir)))
                    return true;
                guard = newPos;
            }
        }
    }

    private enum Direction
    {
        Up = '^',
        Down = 'v',
        Left = '<',
        Right = '>'
    }

    private enum GridSquareType
    {
        Obstacle = '#'
    }

    private sealed class PathEnumerator(char[][] gridMap, (int I, int J) startingPosition, Direction startingDirection) : IEnumerator<((int I, int J) Pos, Direction Direction, (int I, int J)? HitObstaclePos)>
    {
        private (int I, int J) _currentPos;
        private Direction _currentDirection;
        private (int I, int J)? _hitObstaclePos;
        private bool _started = false;

        public ((int I, int J) Pos, Direction Direction, (int I, int J)? HitObstaclePos) Current => (_currentPos, _currentDirection, _hitObstaclePos);

        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {
            // Nothing to do
        }

        private bool InBounds(int i, int j)
        {
            return i >= 0 && i < gridMap.Length && j >= 0 && j < gridMap[i].Length;
        }

        public bool MoveNext()
        {
            if (!_started)
            {
                _currentPos = startingPosition;
                _currentDirection = startingDirection;
                _started = true;
                return true;
            }

            var (di, dj) = GetDirectionDelta(_currentDirection);
            var (pi, pj) = (_currentPos.I + di, _currentPos.J + dj);

            if (!InBounds(pi, pj))
                return false;

            _hitObstaclePos = null;
            if (gridMap[pi][pj] == (char)GridSquareType.Obstacle)
            {
                _hitObstaclePos = (pi, pj);
                _currentDirection = TurnRight(_currentDirection);
                (di, dj) = GetDirectionDelta(_currentDirection);
                (pi, pj) = (_currentPos.I + di, _currentPos.J + dj);
            }

            _currentPos = (pi, pj);
            return true;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
