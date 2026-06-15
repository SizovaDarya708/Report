using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reporter.Entities;

public class IssueEstimateEntity
{
    public string WorkLogId { get; set; }

    public WorkEstimateTypeEnum WorkEstimateType { get; set; }

    public WorklogEntity Worklog { get; set; }
}
