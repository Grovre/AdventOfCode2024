﻿using BenchmarkDotNet.Attributes;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days;

public class Day6 : Day<int, int>
{
    private Position _startingPosition;
    private Direction _startingDirection;
    private char[][] _gridMap = [];

    [GlobalSetup]
    public override async Task Setup()
    {
        _gridMap = (await AdventOfCodeInput.For(2024, 6, SessionId))
            .Select(line => line.ToCharArray())
            .ToArray();

        for (var i = 0; i < _gridMap.Length; i++)
        {
            var j = _gridMap[i]
                .Select((c, idx) => (c, idx))
                .FirstOrDefault(t => t.c is '^' or '>' or 'v' or '<')
                .idx;

            _startingPosition = new(i, j);

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

    private static Position GetDirectionDelta(Direction dir)
    {
        return dir switch
        {
            Direction.Up => new(-1, 0),
            Direction.Down => new(1, 0),
            Direction.Left => new(0, -1),
            Direction.Right => new(0, 1),
            _ => throw new InvalidOperationException()
        };
    }

    private static Position ApplyDirectionDelta(Position pos, Direction dir, int factor)
    {
        var pDelta = GetDirectionDelta(dir);
        return pos + pDelta * factor;
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

    private static bool InBounds(char[][] gridMap, Position p)
    {
        return p.I >= 0 && p.I < gridMap.Length && p.J >= 0 && p.J < gridMap[p.I].Length;
    }

    [Benchmark]
    public override int Solve1()
    {
        var visited = new HashSet<Position>();
        var pathEnumer = new PathEnumerator(_gridMap, _startingPosition, _startingDirection);

        while (pathEnumer.MoveNext())
            visited.Add(pathEnumer.Current.Pos);

        return visited.Count;
    }

    [Benchmark]
    public override int Solve2()
    {
        var possibleSpots = 0;
        var unusedThreadObstacles = new BlockingCollection<char>();
        for (var c = 'A'; c <= 'Z'; c++)
            unusedThreadObstacles.Add(c);
        for (var c = 'a'; c <= 'z'; c++)
            unusedThreadObstacles.Add(c);

        Parallel.For(0, _gridMap.Length, i =>
        {
            for (var j = 0; j < _gridMap[i].Length; j++)
            {
                if (_gridMap[i][j] == (char)GridSquareType.Obstacle || new Position(i, j) == _startingPosition)
                    continue;

                // Temporarily place the obstruction
                var c = unusedThreadObstacles.Take();
                _gridMap[i][j] = c;

                var guard = _startingPosition;
                var dir = _startingDirection;
                var positions = new HashSet<(Position Pos, Direction Dir)> { (guard, dir) };

                while (true)
                {
                    var delta = GetDirectionDelta(dir);
                    var newPos = guard + delta;

                    if (!InBounds(_gridMap, newPos))
                        break;
                    else if (_gridMap[newPos.I][newPos.J] == (char)GridSquareType.Obstacle ||
                             _gridMap[newPos.I][newPos.J] == c)
                        dir = TurnRight(dir);
                    else
                    {
                        if (!positions.Add((newPos, dir)))
                        {
                            Interlocked.Increment(ref possibleSpots);
                            break;
                        }
                        guard = newPos;
                    }
                }

                // Restore the grid map
                _gridMap[i][j] = '.';
                unusedThreadObstacles.Add(c);
            }
        });

        return possibleSpots;
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

    private readonly record struct Position(int I, int J)
    {
        public static Position operator +(Position p1, Position p2) => new(p1.I + p2.I, p1.J + p2.J);
        public static Position operator -(Position p1, Position p2) => new(p1.I - p2.I, p1.J - p2.J);
        public static Position operator *(Position p1, Position p2) => new(p1.I * p2.I, p1.J * p2.J);
        public static Position operator *(Position p1, int scalar) => new(p1.I * scalar, p1.J * scalar);
    }

    private sealed record PathState(Position Pos, Direction Direction, Position? HitObstaclePos);

    private sealed class PathEnumerator(char[][] gridMap, Position startingPosition, Direction startingDirection) : IEnumerator<PathState>
    {
        private Position _currentPos;
        private Direction _currentDirection;
        private Position? _hitObstaclePos;
        private bool _started = false;

        public PathState Current => new(_currentPos, _currentDirection, _hitObstaclePos);

        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {
            // Nothing to do
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

            var nextPos = ApplyDirectionDelta(_currentPos, _currentDirection, 1);

            if (!InBounds(gridMap, nextPos))
                return false;

            _hitObstaclePos = null;
            if (gridMap[nextPos.I][nextPos.J] == (char)GridSquareType.Obstacle)
            {
                _hitObstaclePos = nextPos;
                _currentDirection = TurnRight(_currentDirection);
                nextPos = ApplyDirectionDelta(_currentPos, _currentDirection, 1);
            }

            _currentPos = nextPos;
            return true;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
