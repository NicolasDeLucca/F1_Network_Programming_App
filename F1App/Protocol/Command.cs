namespace Protocol
{
    public enum Command
    {
        CreateMechanic = 1,
        SendMessage = 2,
        ShowAllMessages = 3,
        ReadNewMessages = 4,
        CreateReplacement = 5,
        CreateReplacementCategory = 6,
        AddPhotoToReplacement = 7,
        ShowReplacement = 8,
        ShowReplacementCategories = 9,
        DownloadReplacementPhoto = 10,
        ShowAllReplacements = 11,
        ShowReplacementsByKeywords = 12,
        LogOut = 0,
        LogIn = 99,
        Error = -1,
        // API only
        UpdateReplacement = 98,
        RemoveReplacement = 97,
        RemoveReplacementPhoto = 96
    }
 
}
