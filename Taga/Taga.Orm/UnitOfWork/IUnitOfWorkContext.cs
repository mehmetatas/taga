namespace Taga.Orm.UnitOfWork
{
    public interface IUnitOfWorkStack
    {
        void Push(IUnitOfWork unitOfWork);

        IUnitOfWork Pop();

        IUnitOfWork Current { get; }
    }
}
