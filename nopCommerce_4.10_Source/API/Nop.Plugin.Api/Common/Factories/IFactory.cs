namespace Nop.Plugin.Api.Common.Factories
{
    public interface IFactory<T>
    {
        T Initialize();
    }
}