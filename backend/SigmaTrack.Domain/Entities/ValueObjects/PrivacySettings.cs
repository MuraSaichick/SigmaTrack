using SigmaTrack.Domain.Enums;

namespace SigmaTrack.Domain.Entities.ValueObjects;

public class PrivacySettings
{
    public ContactVisibility ShowContacts { get; set; } = ContactVisibility.TeamOnly;
    public BirthDateVisibility ShowBirthDate { get; set; } = BirthDateVisibility.MonthAndDayOnly;
    public bool ShowOnlineStatus { get; set; } = true;
    public InvitationRestriction WhoCanInviteMe { get; set; } = InvitationRestriction.Everyone;
    public bool Searchable { get; set; } = true;
    public bool ShowStatusMessage { get; set; } = true;
}