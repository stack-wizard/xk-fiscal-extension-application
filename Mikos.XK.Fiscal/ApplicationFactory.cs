using Micros.PosCore.Extensibility;

namespace Mikos.XK.Fiscal
{
    public class ApplicationFactory : IExtensibilityAssemblyFactory
    {
        public ExtensibilityAssemblyBase Create(IExecutionContext context)
        {
            return new Application(context);
        }

        public void Destroy(ExtensibilityAssemblyBase app)
        {
            app.Destroy();
        }
    }
}
