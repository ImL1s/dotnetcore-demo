using dotnetcore_demo.Model;

namespace dotnetcore_demo.Services
{
    public class SampleService
    {
        public ISample Transient { get; private set; }
        public ISample Scoped { get; private set; }
        public ISample Singleton { get; private set; }

        public SampleService(ISampleTransient transient, ISampleScoped scoped, ISampleSingleton singleton)
        {
            Transient = transient;
            Scoped = scoped;
            Singleton = singleton;
        }
    }
}