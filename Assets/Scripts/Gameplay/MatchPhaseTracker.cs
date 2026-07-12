namespace DroneSport.Gameplay
{
    public class MatchPhaseTracker
    {
        private int _openGateCount;

        public MatchPhase Phase { get; private set; } = MatchPhase.InProgress;

        public void NotifyGateOpened()
        {
            if (Phase == MatchPhase.Ended)
            {
                return;
            }

            _openGateCount++;
        }

        public bool NotifyGateClosed()
        {
            if (_openGateCount > 0)
            {
                _openGateCount--;
            }

            if (Phase == MatchPhase.Overtime && _openGateCount == 0)
            {
                Phase = MatchPhase.Ended;
                return true;
            }

            return false;
        }

        public bool NotifyTimeExpired()
        {
            if (Phase != MatchPhase.InProgress)
            {
                return false;
            }

            if (_openGateCount > 0)
            {
                Phase = MatchPhase.Overtime;
                return false;
            }

            Phase = MatchPhase.Ended;
            return true;
        }
    }
}
