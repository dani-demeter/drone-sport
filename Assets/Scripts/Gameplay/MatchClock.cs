namespace DroneSport.Gameplay
{
    public class MatchClock
    {
        public MatchClock(float durationSeconds)
        {
            RemainingSeconds = durationSeconds;
            IsActive = true;
        }

        public float RemainingSeconds { get; private set; }
        public bool IsActive { get; private set; }
        public bool JustExpired { get; private set; }

        public void Tick(float deltaSeconds)
        {
            JustExpired = false;
            if (!IsActive)
            {
                return;
            }

            RemainingSeconds -= deltaSeconds;
            if (RemainingSeconds <= 0f)
            {
                RemainingSeconds = 0f;
                IsActive = false;
                JustExpired = true;
            }
        }
    }
}
