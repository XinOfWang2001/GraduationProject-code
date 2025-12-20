namespace Leap.ApplicationServices.Interfaces.Strategies
{
    // Use chain of command pattern.
    public interface IInputValidatorStrategy<TEntity>
    {
        bool Validate(TEntity input);
        string GetErrorMessage();
    }
}
