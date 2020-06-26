namespace MyFunction.ServiceBus
{
    public class MyServiceBusMessage
    {
        public int nSecondsWait { get; set; } = 30;
        public int nSecondsShutdownDuration { get; set; } = 2;
    }
}
