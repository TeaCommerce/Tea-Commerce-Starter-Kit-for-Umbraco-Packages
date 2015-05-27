using System.Web.Http;
using FollowUpMail.Services;
using Umbraco.Web.Mvc;

namespace FollowUpMail.Controllers {
  public class AutomatedMaillingController : SurfaceController {
    [HttpGet]
    public void SendFollowUpMails() {
      AutomatedMaillingService.Instance.SendFollowupMails();
    }
  }
}