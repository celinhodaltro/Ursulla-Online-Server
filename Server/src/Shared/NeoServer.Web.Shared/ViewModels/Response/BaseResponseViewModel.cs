﻿namespace NeoServer.Web.Shared.ViewModels.Response;

[Serializable]
public class BaseResponseViewModel
{
    public bool Success { get; set; }
    public object Data { get; set; }
    public List<string> Errors { get; set; }
}