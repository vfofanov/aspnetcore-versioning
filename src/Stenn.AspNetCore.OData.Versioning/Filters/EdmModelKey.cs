using System;

namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    public abstract class EdmModelKey : IEquatable<EdmModelKey>
    {
        public static EdmModelKey Default => EdmModelKeyDefault.Instance;

        /// <inheritdoc />
        public virtual bool Equals(EdmModelKey? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            return EqualsInternal(other);
        }

        public static EdmModelKey Aggregate(EdmModelKey[] keys)
        {
            return keys.Length switch
            {
                0 => Default,
                1 => keys[0],
                2 => new Aggregate2EdmModelKey(keys[0], keys[1]),
                3 => new Aggregate3EdmModelKey(keys[0], keys[1], keys[2]),
                4 => new Aggregate4EdmModelKey(keys[0], keys[1], keys[2], keys[3]),
                5 => new Aggregate5EdmModelKey(keys[0], keys[1], keys[2], keys[3], keys[4]),
                6 => new Aggregate6EdmModelKey(keys[0], keys[1], keys[2], keys[3], keys[4], keys[5]),
                7 => new Aggregate7EdmModelKey(keys[0], keys[1], keys[2], keys[3], keys[4], keys[5], keys[6]),
                8 => new Aggregate8EdmModelKey(keys[0], keys[1], keys[2], keys[3], keys[4], keys[5], keys[6], keys[7]),
                _ => new AggregateEdmModelKey(keys)
            };
        }

        public static EdmModelKey Get<T>(T value)
            where T : IEquatable<T>
        {
            return new ValueEdmModelKey<T>(value);
        }

        protected abstract bool EqualsInternal(EdmModelKey other);

        protected abstract int GetHashCodeInternal();

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((EdmModelKey)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetHashCodeInternal();
        }

        #region Nested types
        internal sealed class EdmModelKeyDefault : EdmModelKey
        {
            internal static readonly EdmModelKey Instance = new EdmModelKeyDefault();

            /// <inheritdoc />
            private EdmModelKeyDefault()
            {
            }

            /// <inheritdoc />
            public override bool Equals(EdmModelKey? obj)
            {
                return ReferenceEquals(this, obj);
            }

            /// <inheritdoc />
            public override bool Equals(object? obj)
            {
                return ReferenceEquals(this, obj);
            }

            /// <inheritdoc />
            protected override bool EqualsInternal(EdmModelKey other)
            {
                return ReferenceEquals(this, other);
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                return 0;
            }

            /// <inheritdoc />
            protected override int GetHashCodeInternal()
            {
                return 0;
            }
        }

        internal sealed class ValueEdmModelKey<T> : EdmModelKey, IEquatable<ValueEdmModelKey<T>>
            where T : IEquatable<T>
        {
            private readonly T _value;

            /// <inheritdoc />
            public ValueEdmModelKey(T value)
            {
                _value = value ?? throw new ArgumentNullException(nameof(value));
            }

            /// <inheritdoc />
            public bool Equals(ValueEdmModelKey<T>? other)
            {
                return !ReferenceEquals(null, other) && EqualsFinal(other);
            }

            /// <inheritdoc />
            public override bool Equals(object? obj)
            {
                return ReferenceEquals(this, obj) || obj is ValueEdmModelKey<T> other && EqualsFinal(other);
            }

            /// <inheritdoc />
            protected override bool EqualsInternal(EdmModelKey other)
            {
                return other is ValueEdmModelKey<T> obj && EqualsFinal(obj);
            }

            private bool EqualsFinal(ValueEdmModelKey<T> other)
            {
                return ReferenceEquals(this, other) || _value.Equals(other._value);
            }

            /// <inheritdoc />
            protected override int GetHashCodeInternal()
            {
                return _value.GetHashCode();
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                return _value.GetHashCode();
            }
        }

        internal sealed class AggregateEdmModelKey : EdmModelKey
        {
            private readonly EdmModelKey[] _keys;

            public AggregateEdmModelKey(EdmModelKey[] keys)
            {
                _keys = keys;
            }

            /// <inheritdoc />
            protected override bool EqualsInternal(EdmModelKey other)
            {
                return other is AggregateEdmModelKey obj && EqualsInternal(obj);
            }

            private bool EqualsInternal(AggregateEdmModelKey other)
            {
                if (ReferenceEquals(this, other))
                {
                    return true;
                }
                if (_keys.Length != other._keys.Length)
                {
                    return false;
                }
                for (var i = 0; i < _keys.Length; i++)
                {
                    if (!_keys[i].EqualsInternal(other._keys[i]))
                    {
                        return false;
                    }
                }
                return true;
            }

            /// <inheritdoc />
            protected override int GetHashCodeInternal()
            {
                var hashCode = new HashCode();
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < _keys.Length; i++)
                {
                    hashCode.Add(_keys[i]);
                }
                return hashCode.ToHashCode();
            }
        }

        internal sealed class Aggregate2EdmModelKey : EdmModelKey
        {
            private readonly EdmModelKey _k1;
            private readonly EdmModelKey _k2;

            public Aggregate2EdmModelKey(EdmModelKey k1, EdmModelKey k2)
            {
                _k1 = k1;
                _k2 = k2;
            }

            /// <inheritdoc />
            protected override bool EqualsInternal(EdmModelKey other)
            {
                return other is Aggregate2EdmModelKey obj && EqualsFinal(obj);
            }

            private bool EqualsFinal(Aggregate2EdmModelKey other)
            {
                if (ReferenceEquals(this, other))
                {
                    return true;
                }
                return _k1.Equals(other._k1) &&
                       _k2.Equals(other._k2);
            }

            /// <inheritdoc />
            protected override int GetHashCodeInternal()
            {
                return HashCode.Combine(_k1, _k2);
            }
        }

        internal sealed class Aggregate3EdmModelKey : EdmModelKey
        {
            private readonly EdmModelKey _k1;
            private readonly EdmModelKey _k2;
            private readonly EdmModelKey _k3;

            public Aggregate3EdmModelKey(EdmModelKey k1, EdmModelKey k2, EdmModelKey k3)
            {
                _k1 = k1;
                _k2 = k2;
                _k3 = k3;
            }

            /// <inheritdoc />
            protected override bool EqualsInternal(EdmModelKey other)
            {
                return other is Aggregate3EdmModelKey obj && EqualsFinal(obj);
            }

            private bool EqualsFinal(Aggregate3EdmModelKey other)
            {
                if (ReferenceEquals(this, other))
                {
                    return true;
                }
                return _k1.Equals(other._k1) &&
                       _k2.Equals(other._k2) &&
                       _k3.Equals(other._k3);
            }

            /// <inheritdoc />
            protected override int GetHashCodeInternal()
            {
                return HashCode.Combine(_k1, _k2, _k3);
            }
        }

        internal sealed class Aggregate4EdmModelKey : EdmModelKey
        {
            private readonly EdmModelKey _k1;
            private readonly EdmModelKey _k2;
            private readonly EdmModelKey _k3;
            private readonly EdmModelKey _k4;

            public Aggregate4EdmModelKey(EdmModelKey k1, EdmModelKey k2, EdmModelKey k3, EdmModelKey k4)
            {
                _k1 = k1;
                _k2 = k2;
                _k3 = k3;
                _k4 = k4;
            }

            /// <inheritdoc />
            protected override bool EqualsInternal(EdmModelKey other)
            {
                return other is Aggregate4EdmModelKey obj && EqualsFinal(obj);
            }

            private bool EqualsFinal(Aggregate4EdmModelKey other)
            {
                if (ReferenceEquals(this, other))
                {
                    return true;
                }
                return _k1.Equals(other._k1) &&
                       _k2.Equals(other._k2) &&
                       _k3.Equals(other._k3) &&
                       _k4.Equals(other._k4);
            }

            /// <inheritdoc />
            protected override int GetHashCodeInternal()
            {
                return HashCode.Combine(_k1, _k2, _k3, _k4);
            }
        }

        internal sealed class Aggregate5EdmModelKey : EdmModelKey
        {
            private readonly EdmModelKey _k1;
            private readonly EdmModelKey _k2;
            private readonly EdmModelKey _k3;
            private readonly EdmModelKey _k4;
            private readonly EdmModelKey _k5;

            public Aggregate5EdmModelKey(EdmModelKey k1, EdmModelKey k2, EdmModelKey k3, EdmModelKey k4, EdmModelKey k5)
            {
                _k1 = k1;
                _k2 = k2;
                _k3 = k3;
                _k4 = k4;
                _k5 = k5;
            }

            /// <inheritdoc />
            protected override bool EqualsInternal(EdmModelKey other)
            {
                return other is Aggregate5EdmModelKey obj && EqualsFinal(obj);
            }

            private bool EqualsFinal(Aggregate5EdmModelKey other)
            {
                if (ReferenceEquals(this, other))
                {
                    return true;
                }
                return _k1.Equals(other._k1) &&
                       _k2.Equals(other._k2) &&
                       _k3.Equals(other._k3) &&
                       _k4.Equals(other._k4) &&
                       _k5.Equals(other._k5);
            }

            /// <inheritdoc />
            protected override int GetHashCodeInternal()
            {
                return HashCode.Combine(_k1, _k2, _k3, _k4, _k5);
            }
        }

        internal sealed class Aggregate6EdmModelKey : EdmModelKey
        {
            private readonly EdmModelKey _k1;
            private readonly EdmModelKey _k2;
            private readonly EdmModelKey _k3;
            private readonly EdmModelKey _k4;
            private readonly EdmModelKey _k5;
            private readonly EdmModelKey _k6;


            public Aggregate6EdmModelKey(EdmModelKey k1, EdmModelKey k2, EdmModelKey k3, EdmModelKey k4, EdmModelKey k5, EdmModelKey k6)
            {
                _k1 = k1;
                _k2 = k2;
                _k3 = k3;
                _k4 = k4;
                _k5 = k5;
                _k6 = k6;
            }

            /// <inheritdoc />
            protected override bool EqualsInternal(EdmModelKey other)
            {
                return other is Aggregate6EdmModelKey obj && EqualsFinal(obj);
            }

            private bool EqualsFinal(Aggregate6EdmModelKey other)
            {
                if (ReferenceEquals(this, other))
                {
                    return true;
                }
                return _k1.Equals(other._k1) &&
                       _k2.Equals(other._k2) &&
                       _k3.Equals(other._k3) &&
                       _k4.Equals(other._k4) &&
                       _k5.Equals(other._k5) &&
                       _k6.Equals(other._k6);
            }

            /// <inheritdoc />
            protected override int GetHashCodeInternal()
            {
                return HashCode.Combine(_k1, _k2, _k3, _k4, _k5, _k6);
            }
        }

        internal sealed class Aggregate7EdmModelKey : EdmModelKey
        {
            private readonly EdmModelKey _k1;
            private readonly EdmModelKey _k2;
            private readonly EdmModelKey _k3;
            private readonly EdmModelKey _k4;
            private readonly EdmModelKey _k5;
            private readonly EdmModelKey _k6;
            private readonly EdmModelKey _k7;


            public Aggregate7EdmModelKey(EdmModelKey k1, EdmModelKey k2, EdmModelKey k3, EdmModelKey k4, EdmModelKey k5, EdmModelKey k6, EdmModelKey k7)
            {
                _k1 = k1;
                _k2 = k2;
                _k3 = k3;
                _k4 = k4;
                _k5 = k5;
                _k6 = k6;
                _k7 = k7;
            }

            /// <inheritdoc />
            protected override bool EqualsInternal(EdmModelKey other)
            {
                return other is Aggregate7EdmModelKey obj && EqualsFinal(obj);
            }

            private bool EqualsFinal(Aggregate7EdmModelKey other)
            {
                if (ReferenceEquals(this, other))
                {
                    return true;
                }
                return _k1.Equals(other._k1) &&
                       _k2.Equals(other._k2) &&
                       _k3.Equals(other._k3) &&
                       _k4.Equals(other._k4) &&
                       _k5.Equals(other._k5) &&
                       _k6.Equals(other._k6) &&
                       _k7.Equals(other._k7);
            }

            /// <inheritdoc />
            protected override int GetHashCodeInternal()
            {
                return HashCode.Combine(_k1, _k2, _k3, _k4, _k5, _k6, _k7);
            }
        }

        internal sealed class Aggregate8EdmModelKey : EdmModelKey
        {
            private readonly EdmModelKey _k1;
            private readonly EdmModelKey _k2;
            private readonly EdmModelKey _k3;
            private readonly EdmModelKey _k4;
            private readonly EdmModelKey _k5;
            private readonly EdmModelKey _k6;
            private readonly EdmModelKey _k7;
            private readonly EdmModelKey _k8;

            public Aggregate8EdmModelKey(EdmModelKey k1, EdmModelKey k2, EdmModelKey k3, EdmModelKey k4, EdmModelKey k5, EdmModelKey k6, EdmModelKey k7,
                EdmModelKey k8)
            {
                _k1 = k1;
                _k2 = k2;
                _k3 = k3;
                _k4 = k4;
                _k5 = k5;
                _k6 = k6;
                _k7 = k7;
                _k8 = k8;
            }

            /// <inheritdoc />
            protected override bool EqualsInternal(EdmModelKey other)
            {
                return other is Aggregate8EdmModelKey obj && EqualsFinal(obj);
            }

            private bool EqualsFinal(Aggregate8EdmModelKey other)
            {
                if (ReferenceEquals(this, other))
                {
                    return true;
                }
                return _k1.Equals(other._k1) &&
                       _k2.Equals(other._k2) &&
                       _k3.Equals(other._k3) &&
                       _k4.Equals(other._k4) &&
                       _k5.Equals(other._k5) &&
                       _k6.Equals(other._k6) &&
                       _k7.Equals(other._k7) &&
                       _k8.Equals(other._k8);
            }

            /// <inheritdoc />
            protected override int GetHashCodeInternal()
            {
                return HashCode.Combine(_k1, _k2, _k3, _k4, _k5, _k6, _k7, _k8);
            }
        }
        #endregion
    }
}