namespace TreehouseDefense
{
    public class Calculator
    {
        private double _val;
        public Calculator(double value)
        {
            _val = value;
        }

        public void Add(double value)
        {
            _val += value; 
        }

        public double Result
        {
            get => _val;
        }

        public void Subtract(double value)
        {
            _val -= value;
        }
    }
}
