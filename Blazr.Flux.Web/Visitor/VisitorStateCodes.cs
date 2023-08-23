/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux.Web;

public class VisitorStateCodes : StateCodes
{
    public static StateCode Draft = new(StateCodes.Existing.Value, "Draft");
    public static StateCode Submitted = new(20, "Submitted");
    public static StateCode Approved = new(30, "Approved");
    public static StateCode OnSite = new(40, "On Site");
    public static StateCode OffSite = new(50, "Off Site");
    public static StateCode Completed = new(1001, "Completed");
    public static StateCode Closed = new(1002, "Closed");


    public static List<StateCode> VisitorStateCodeList = new()
    {
        New, Draft, Submitted, Approved, OnSite, OffSite, Completed, Closed, Null
    };

    public static StateCode GetVisitorCode(int code)
        => VisitorStateCodeList.FirstOrDefault(item => item.Value == code);
}
