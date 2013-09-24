using System;

namespace Taga.Core
{
    public class SetOnce<T>
    {
        private readonly object _lockObj = new Object();

        private readonly string _name;

        private T _value;
        private bool _hasValue;

        public SetOnce(string name)
        {
            _name = name;
        }

        public T Value
        {
            get
            {
                lock (_lockObj)
                {
                    if (!_hasValue)
                        throw new InvalidOperationException(_name + " has not been initialized");
                    return _value;
                }
            }
            set
            {
                lock (_lockObj)
                {
                    if (_hasValue)
                        throw new InvalidOperationException(_name + " has already been initialized");
                    _value = value;
                    _hasValue = true;
                }
            }
        }
    }
}