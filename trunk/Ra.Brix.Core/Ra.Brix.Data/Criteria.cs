/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

/**
 * Namespace for all commonly used API parts of Ra-Brix. Contains most
 * data abstractions in Ra-Brix.
 */
namespace Ra.Brix.Data
{
    /**
     * Abstract base class for all data storage retrieval criterias.
     * Also contains several handy static constructors for easy
     * creation of data storage retrieval criterias.
     */
    public abstract class Criteria
    {
        private readonly string _propertyName;
        private readonly object _value;

        protected Criteria(string propertyName, object value)
        {
            _propertyName = propertyName;
            _value = value;
        }

        /**
         * Name of property associated with criteria.
         */
        public string PropertyName
        {
            get { return _propertyName; }
        }

        /**
         * Value that criteria associates with the given property
         * of your criteria.
         */
        public object Value
        {
            get { return _value; }
        }

        /**
         * Static constructor to create a criteria of type Equals.
         */
        public static Criteria Eq(string propertyName, object value)
        {
            return new Equals(propertyName, value);
        }

        /**
         * Static constructor to create a criteria of type NotEquals.
         */
        public static Criteria Ne(string propertyName, object value)
        {
            return new NotEquals(propertyName, value);
        }

        /**
         * Static constructor to create a criteria of type LikeEquals.
         */
        public static Criteria Like(string propertyName, string value)
        {
            return new LikeEquals(propertyName, value);
        }

        /**
         * Static constructor to create a criteria of type LikeNotEquals.
         */
        public static Criteria NotLike(string propertyName, string value)
        {
            return new LikeNotEquals(propertyName, value);
        }

        /**
         * Static constructor to create a criteria of type LessThen.
         */
        public static Criteria Lt(string propertyName, object value)
        {
            return new LessThen(propertyName, value);
        }

        /**
         * Static constructor to create a criteria of type MoreThen.
         */
        public static Criteria Mt(string propertyName, object value)
        {
            return new MoreThen(propertyName, value);
        }

        /**
         * Static constructor to create a criteria of type ParentIdEquals.
         */
        public static Criteria ParentId(int id)
        {
            return new ParentIdEquals(id);
        }

        /**
         * Static constructor to create a criteria of type ExistsInEquals.
         */
        public static Criteria ExistsIn(int id)
        {
            return new ExistsInEquals(id);
        }

        /**
         * Static constructor to create a criteria of type ExistsInEquals.
         */
        public static Criteria HasChild(int id)
        {
            return new HasChildId(id);
        }
    }

    /**
     * A criteria that makes sure your object is the parent of the object with
     * the given id
     */
    public class HasChildId : Criteria
    {
        public HasChildId(int id)
            : base(null, id)
        { }
    }

    /**
     * A criteria that makes sure your object must exist within the object with the
     * given ID in order to be true.
     */
    public class ExistsInEquals : Criteria
    {
        public ExistsInEquals(int id)
            : base(null, id)
        { }
    }

    /**
     * A criteria that only returns true if the object is a child (IsOwner=true)
     * of the object with the given ID.
     */
    public class ParentIdEquals : Criteria
    {
        public ParentIdEquals(int id)
            : base(null, id)
        { }
    }

    /**
     * Returns only true if the property with the given name has the given value.
     */
    public class Equals : Criteria
    {
        public Equals(string propertyName, object value)
            : base(propertyName, value)
        { }
    }

    /**
     * Returns only true if the property with the given name does NOT have the given value.
     */
    public class NotEquals : Criteria
    {
        public NotEquals(string propertyName, object value)
            : base(propertyName, value)
        { }
    }

    /**
     * Returns only true if the property with the given name contains the given value
     * string. Only usable for string types.
     */
    public class LikeEquals : Criteria
    {
        public LikeEquals(string propertyName, string value)
            : base(propertyName, value)
        { }
    }

    /**
     * Returns only true if the property with the given name does NOT contain the given value
     * string. Only usable for string types.
     */
    public class LikeNotEquals : Criteria
    {
        public LikeNotEquals(string propertyName, string value)
            : base(propertyName, value)
        { }
    }

    /**
     * Returns only true if the property with the given name have a value which is
     * "less" then the given value.
     */
    public class LessThen : Criteria
    {
        public LessThen(string propertyName, object value)
            : base(propertyName, value)
        { }
    }

    /**
     * Returns only true if the property with the given name have a value which is
     * "more" then the given value.
     */
    public class MoreThen : Criteria
    {
        public MoreThen(string propertyName, object value)
            : base(propertyName, value)
        { }
    }
}
