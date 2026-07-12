namespace DroneSport.Input
{
    public readonly struct DroneInputChannels
    {
        public readonly float Throttle;
        public readonly float Roll;
        public readonly float Pitch;
        public readonly float Yaw;
        public readonly bool IsArmed;

        public DroneInputChannels(float throttle, float roll, float pitch, float yaw, bool isArmed)
        {
            Throttle = throttle;
            Roll = roll;
            Pitch = pitch;
            Yaw = yaw;
            IsArmed = isArmed;
        }
    }
}
