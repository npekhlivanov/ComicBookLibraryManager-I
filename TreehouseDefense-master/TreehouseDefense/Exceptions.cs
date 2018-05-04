namespace TreehouseDefense
{
    public class TreehouseDefenseException : System.Exception
    {
        public TreehouseDefenseException()
        {
        }
        
        public TreehouseDefenseException(string message) : base(message)
        {
        }
    }
    
    public class OutOfBoundsException : TreehouseDefenseException
    {
        public OutOfBoundsException()
        {
        }
        
        public OutOfBoundsException(string message) : base(message)
        {
        }
    }
}