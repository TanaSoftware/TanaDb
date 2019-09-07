
namespace Tor
{
    public class Initiator
    {
        public static void InitSystemInfra()
        {
            RouteConfig.RegisterRoutes();
            HebrewCalendarManager.init();
            MessageObserver message = new MessageObserver();
            message.Run();
        }
    }
}
