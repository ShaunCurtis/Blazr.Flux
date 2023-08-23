namespace Blazr.Flux.Web;

public class SaveToDraftProcess : OwsWorkflowProcessRequest, IOwsProcessRequest, IVisitorProcessRequest
{
    public SaveToDraftProcess() {
        this.DisplayName = "Save As Draft";
        this.Description = "Save the Visit as a draft for later submission.";
        this.StartingStates = new() { VisitorStateCodes.New, VisitorStateCodes.Draft };
        this.FinalState = VisitorStateCodes.Draft;
    }
}

public class SubmittedProcess : OwsWorkflowProcessRequest, IOwsProcessRequest, IVisitorProcessRequest
{
    public SubmittedProcess()
    {
        this.DisplayName = "Submit";
        this.Description = "Submit the Visit for approval.";
        this.StartingStates = new() { VisitorStateCodes.New, VisitorStateCodes.Draft };
        this.FinalState = VisitorStateCodes.Submitted;
    }
}

public class ApprovedProcess : OwsWorkflowProcessRequest, IOwsProcessRequest, IVisitorProcessRequest
{
    public ApprovedProcess()
    {
        this.DisplayName = "Approve";
        this.Description = "Approve the Visit.";
        this.StartingStates = new() { VisitorStateCodes.Submitted };
        this.FinalState = VisitorStateCodes.Approved;
    }
}

public class OnSiteProcess : OwsWorkflowProcessRequest, IOwsProcessRequest, IVisitorProcessRequest
{
    public OnSiteProcess()
    {
        this.DisplayName = "On Site";
        this.Description = "Book the visitor on site.";
        this.StartingStates = new() { VisitorStateCodes.Approved, VisitorStateCodes.OffSite };
        this.FinalState = VisitorStateCodes.OnSite;
    }
}

public class OffSiteProcess : OwsWorkflowProcessRequest, IOwsProcessRequest, IVisitorProcessRequest
{
    public OffSiteProcess()
    {
        this.DisplayName = "Off Site";
        this.Description = "Book the visitor off site i.e. they are temporarily leaving but will return today.";
        this.StartingStates = new() { VisitorStateCodes.OnSite};
        this.FinalState = VisitorStateCodes.OffSite;
    }
}

public class CompletedProcess : OwsWorkflowProcessRequest, IOwsProcessRequest, IVisitorProcessRequest
{
    public CompletedProcess()
    {
        this.DisplayName = "Complete";
        this.Description = "The visit is complete.";
        this.StartingStates = new() { VisitorStateCodes.OnSite, VisitorStateCodes.OffSite };
        this.FinalState = VisitorStateCodes.Completed;
    }
}

public class ClosedProcess : OwsWorkflowProcessRequest, IOwsProcessRequest, IVisitorProcessRequest
{
    public ClosedProcess()
    {
        this.DisplayName = "Close";
        this.Description = "The visit should be closed.  The visitor isn't coming.";
        this.StartingStates = new() { VisitorStateCodes.Draft, VisitorStateCodes.Submitted, VisitorStateCodes.Approved };
        this.FinalState = VisitorStateCodes.Closed;
    }
}
