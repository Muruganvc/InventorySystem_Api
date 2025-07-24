namespace InventorySystem_Application.Common;
public interface IResult<T>
{
    bool IsSuccess { get; }
    string Error { get; }
    T Value { get; }
}