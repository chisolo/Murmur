public interface ITimeProgressBarData
{
    long EndTime { get; }

    long Duration { get ; }

    int AdCount { get; }

    long AdCooldownTime { get; }

    void Finish();
    void ReduceTimeByAd(long time, long adCooldown);

    string GetAdUnitName();
}