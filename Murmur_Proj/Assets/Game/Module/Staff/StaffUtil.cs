
using System.Collections.Generic;

#region staff
public static class StaffRarity
{
    public const string NORMAL = "NORMAL";
    public const string RARE = "RARE";
    public const string SR = "SR";
}

public static class StaffText
{
    public const string AlmostZero = "ALMOST_ZERO";
    public const string Low = "LOW";
    public const string Medium = "MEDIUM";
    public const string High = "HIGH";
    public const string VeryHigh = "VERY_HIGH";

    public const string MakeOffer = "MAKE_OFFER";
    public const string OfferAccepted = "OFFER_ACCEPTED";
    public const string OfferRejected = "OFFER_REJECTED";

    public const string CautionText = "CAUTION";
    public const string FireStaffBodyMsg = "FIRE_STAFF_BODY_MSG";
    public const string FireStaffBodyRareMsg = "FIRE_STAFF_BODY_RARE_MSG";
    public const string Back = "BACK";
    public const string Fire = "FIRE";

    public const string StaffFullTitleText = "STAF_FFULL_TITLE";
    public const string StaffFullBodyText = "STAF_FFULL_BODY_MSG";
    public const string Accept = "ACCEPT";
}

public enum CandidateOfferStatus
{
    None = 0,
    OfferAccepted,
    OfferRejected
}

public static class HireProbability
{
    public const float ProbabilityZero = 0.6f;
    public const float ProbabilityLow = 0.9f;
    public const float ProbabilityMedium = 1.0f;
    public const float ProbabilityHigh = 1.1f;
}

public static class StaffDefine
{
    public const int CouponRare = 5;
    public const int CouponSR = 10;
}
#endregion