namespace NeoServer.Web.Shared.ViewModels.Request;

public class PlayerPostRequest
{
    public string Name { get; set; }
    public int Sex { get; set; }
    public int Vocation { get; set; }
    public int Town { get; set; }
    public int AccountId { get; set; }
    public int WorldId { get; set; }
    public int PosX { get; set; }
    public int PosY { get; set; }
    public int PosZ { get; set; }
}