namespace ITech.Security.Business
{
    public abstract class BaseCommandController : BaseController
    {
        protected DbKeeper command;

        protected BaseCommandController()
        {
            command = new DbKeeper(ask.db, ask);            
        }
    }
}