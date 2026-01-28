namespace Applet.Nat.Ux.Models
{
    public enum EDcOper
    {
        None,
        Get,
        Add,
        Update,
        Remove,
    }
    public enum EAuthType
    {
        UserAndPass,
        Token,
        OIDC,
    }
    public enum eTask : short
    {
        Print = 1,
        Share = 2,
        Tracking=3,
        Original = 4,
        PdfGen=5,
        ETsts = 7,
        Auth = 8,
        Delete = 9,
        Save = 10,
        Pass = 11,
        SendResponse=12
    }
    public enum eDialogType : short
    {
        Text = 1,
        FileUpload = 2,
        Tracking = 3,
        Me = 4,
        User = 5,
        Cuit=6,
        List = 7,
        Oidc = 8,
        CuitCuit = 9
    }
    public enum eDialogButtoms : short
    {
        Ok = 1,
        OkCancel = 2,
        YesNo = 3,
        X=4,
    }
    public enum eDialogResponse : short
    {
        Ok = 1,
        Cancel = 2,
        Yes = 3,
        No = 4,
    }
}

