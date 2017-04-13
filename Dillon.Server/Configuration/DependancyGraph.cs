
namespace Dillon.Server
{
    using Autofac;
    using Autofac.Integration.WebApi;
    using Common;
    using Input;
    using Mappings;
    using System.Reflection;
    using System.Windows.Forms;
    using WindowsInput;

    public static class DependancyRegistrar {

        public static IContainer RegisterDependancies(string[] args) {
            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.Register(c => new InputSimulator()).As<IInputSimulator>();
            builder.Register(c => new KeyboardSimulatorAdapter(c.Resolve<IInputSimulator>().Keyboard)).As<IKeyboardSimulatorAdapter>();
            builder.Register(c => new MouseSimulatorAdapter(c.Resolve<IInputSimulator>().Mouse)).As<IMouseSimulatorAdapter>();

            builder.Register(c => new CoreMappingFactory(c.Resolve<IKeyboardSimulatorAdapter>(), c.Resolve<IMouseSimulatorAdapter>())).As<ICoreMappingFactory>();
            builder.Register(c => new Configurator(c.Resolve<ICoreMappingFactory>())).As<IConfigurator>();
            builder.Register(c => c.Resolve<IConfigurator>().Configure(args)).As<IConfiguration>();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            return builder.Build();
        }
    }
}