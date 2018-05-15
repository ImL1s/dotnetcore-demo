namespace dotnetcore_demo.Model
{
    public class Sample : ISampleTransient, ISampleScoped, ISampleSingleton
    {
        private static int _counter = default(int);
        private int _id;

        public Sample()
        {
            _id = ++_counter;
        }

        public int Id => _id;
    }
}