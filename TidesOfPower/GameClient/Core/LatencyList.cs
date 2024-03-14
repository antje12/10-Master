using System.Collections.Generic;
using System.Linq;

namespace GameClient.Core;

public class LatencyList
{
    private readonly List<long> _values = new List<long>();
    private readonly int _maxSize;

    public LatencyList(int maxSize)
    {
        _maxSize = maxSize;
    }

    public void Add(long value)
    {
        if (_values.Count >= _maxSize)
        {
            _values.RemoveAt(0);
        }
        _values.Add(value);
    }

    public int GetAverage()
    {
        if (_values.Count == 0)
        {
            return 0;
        }
        return (int) _values.Average();
    }
}