namespace DroneSport.Input
{
    public interface IDroneInputSource
    {
        DroneInputChannels ReadChannels();
    }
}
