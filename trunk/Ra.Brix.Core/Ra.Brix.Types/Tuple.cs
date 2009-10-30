/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

namespace Ra.Brix.Types
{
    /**
     * A helper structure to make it possible to create multiple return values or "couple"
     * objects together. Type is a natural immutable type and hence also 100% thread safe.
     * Though objects contained within the type might not be.
     */
    public struct Tuple<TLeft, TRight>
    {
        private readonly TLeft _left;
        private readonly TRight _right;

        /**
         * CTOR taking left/first and right/second arguments.
         */
        public Tuple(TLeft left, TRight right)
        {
            _left = left;
            _right = right;
        }

        /**
         * Returns the left or first object
         */
        public TLeft Left
        {
            get { return _left; }
        }

        /**
         * Returns the right or second argument
         */
        public TRight Right
        {
            get { return _right; }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (Tuple<TLeft, TRight>)) return false;
            return Equals((Tuple<TLeft, TRight>) obj);
        }

        public static bool operator ==(Tuple<TLeft, TRight> left, Tuple<TRight, TLeft> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Tuple<TLeft, TRight> left, Tuple<TRight, TLeft> right)
        {
            return !left.Equals(right);
        }

        public bool Equals(Tuple<TLeft, TRight> other)
        {
            return Equals(other._left, _left) && Equals(other._right, _right);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_left.GetHashCode()*397) ^ _right.GetHashCode();
            }
        }
    }
}
